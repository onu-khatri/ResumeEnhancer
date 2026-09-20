using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text.Json;
using ResumeEnhancer.AuthModule.DM.Entities;
using ResumeEnhancer.AuthModule.SL.Abstractions;
using ResumeEnhancer.Infrastructure.Persistence;

namespace ResumeEnhancer.AuthModule.PL.Repositories;

public sealed class AuthRepository(IUnitOfWork<AppDbContext> unitOfWork, TimeProvider? timeProvider = null) : IAuthRepository
{
    public async Task<IReadOnlyList<AuthSigningKeyMetadata>> GetSigningKeyMetadataAsync(CancellationToken cancellationToken = default) =>
        await unitOfWork.GetRepo<AuthSigningKeyMetadata>().Query().AsNoTracking()
            .OrderByDescending(x => x.ActivatedAtUtc).ThenBy(x => x.KeyIdentifier).ToListAsync(cancellationToken);
    public async Task InvalidateSigningKeyAsync(string keyIdentifier, DateTime invalidatedAtUtc, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(keyIdentifier)) throw new ArgumentException("A key identifier is required.", nameof(keyIdentifier));
        var updated = await unitOfWork.GetRepo<AuthSigningKeyMetadata>().Query().Where(x => x.KeyIdentifier == keyIdentifier && x.InvalidatedAtUtc == null)
            .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.InvalidatedAtUtc, invalidatedAtUtc), cancellationToken);
        if (updated == 0)
            return;
        await SaveAsync(cancellationToken);
    }

    public async Task<bool> TryRotateSigningKeyAsync(
        string expectedKeyIdentifier,
        long expectedLifecycleVersion,
        AuthSigningKeyMetadata replacement,
        DateTime retiredAtUtc,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await unitOfWork.CreateTransactionAsync(cancellationToken);
        try
        {
            var keys = unitOfWork.GetRepo<AuthSigningKeyMetadata>();
            var retired = await keys.Query()
                .Where(x => x.IsActive && x.KeyIdentifier == expectedKeyIdentifier
                    && x.LifecycleVersion == expectedLifecycleVersion
                    && x.RetiredAtUtc == null && x.InvalidatedAtUtc == null)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.IsActive, false)
                    .SetProperty(x => x.RetiredAtUtc, retiredAtUtc), cancellationToken);
            if (retired != 1)
                return false;

            replacement.IsActive = true;
            replacement.LifecycleVersion = expectedLifecycleVersion + 1;
            await keys.AddAsync(replacement, cancellationToken);
            await SaveAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            return false;
        }
        catch (DbUpdateException)
        {
            // A competing writer can win the filtered active-key uniqueness race.
            // The caller must reload committed state rather than publish this candidate.
            return false;
        }
    }
    public async Task<IReadOnlyList<PasswordHistoryEntry>> GetPasswordHistoryAsync(
        int authenticationIdentityId,
        int take = 3,
        CancellationToken cancellationToken = default) =>
        await unitOfWork.GetRepo<PasswordHistoryEntry>().Query()
            .Where(x => x.AuthenticationIdentityId == authenticationIdentityId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ThenByDescending(x => x.Id)
            .Take(Math.Clamp(take, 0, 2))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public Task<AuthenticationIdentity?> FindIdentityByIdAsync(
        int authenticationIdentityId,
        CancellationToken cancellationToken = default) =>
        unitOfWork.GetRepo<AuthenticationIdentity>().Query()
            .SingleOrDefaultAsync(x => x.Id == authenticationIdentityId, cancellationToken);

    public async Task UpdatePasswordAndRevokeSessionsAsync(
        int authenticationIdentityId,
        int userId,
        string newPasswordHash,
        DateTime changedAtUtc,
        string revocationReason,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(newPasswordHash);
        ArgumentException.ThrowIfNullOrWhiteSpace(revocationReason);

        await using var transaction = await unitOfWork.CreateTransactionAsync(cancellationToken);
        var identity = await unitOfWork.GetRepo<AuthenticationIdentity>().Query()
            .SingleOrDefaultAsync(x => x.Id == authenticationIdentityId, cancellationToken)
            ?? throw new AuthPersistenceConflictException(
                new InvalidOperationException("Authentication identity was not found."));

        await unitOfWork.GetRepo<PasswordHistoryEntry>().AddAsync(new PasswordHistoryEntry
        {
            AuthenticationIdentityId = identity.Id,
            PasswordHash = identity.PasswordHash,
            CreatedAtUtc = changedAtUtc,
        }, cancellationToken);
        identity.PasswordHash = newPasswordHash;
        identity.FailedLoginAttempts = 0;
        identity.LastFailedLoginAtUtc = null;
        identity.LockedUntilUtc = null;

        var historyRepository = unitOfWork.GetRepo<PasswordHistoryEntry>();
        var staleHistory = await historyRepository.Query()
            .Where(x => x.AuthenticationIdentityId == identity.Id)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ThenByDescending(x => x.Id)
            .Skip(2)
            .ToListAsync(cancellationToken);
        historyRepository.Delete(staleHistory);

        await unitOfWork.GetRepo<RefreshSession>().Query()
            .Where(x => x.UserId == userId && x.RevokedAtUtc == null && x.ExpiresAtUtc > changedAtUtc)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.RevokedAtUtc, changedAtUtc)
                .SetProperty(x => x.RevocationReason, revocationReason), cancellationToken);

        await SaveAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task AddChallengeAsync(AuthChallenge challenge, CancellationToken cancellationToken = default)
    {
        var isActive = await unitOfWork.GetRepo<AuthChallengePurpose>().Query()
            .AnyAsync(x => x.Id == challenge.AuthChallengePurposeId && !x.ObsoleteFlag, cancellationToken);
        if (!isActive)
            throw new AuthPersistenceConflictException(new InvalidOperationException("The authentication challenge purpose is invalid."));

        await unitOfWork.GetRepo<AuthChallenge>().AddAsync(challenge, cancellationToken);
    }

    public Task<int?> FindActiveChallengePurposeIdAsync(
        string purposeCode,
        CancellationToken cancellationToken = default) =>
        string.IsNullOrWhiteSpace(purposeCode)
            ? Task.FromResult<int?>(null)
            : unitOfWork.GetRepo<AuthChallengePurpose>().Query()
                .Where(x => x.Code == purposeCode && !x.ObsoleteFlag)
                .Select(x => (int?)x.Id)
                .SingleOrDefaultAsync(cancellationToken);

    public async Task<ChallengeConsumeResult> TryConsumeChallengeAsync(
        int authenticationIdentityId,
        string purposeCode,
        string tokenHash,
        DateTime nowUtc,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(purposeCode) || string.IsNullOrWhiteSpace(tokenHash))
            return ChallengeConsumeResult.InvalidOrExpired;

        var updated = await unitOfWork.GetRepo<AuthChallenge>().Query()
            .Where(x => x.AuthenticationIdentityId == authenticationIdentityId
                && x.AuthChallengePurpose!.Code == purposeCode
                && !x.AuthChallengePurpose.ObsoleteFlag
                && x.TokenHash == tokenHash
                && x.ConsumedAtUtc == null
                && x.ExpiresAtUtc > nowUtc)
            .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.ConsumedAtUtc, nowUtc), cancellationToken);
        return updated == 1 ? ChallengeConsumeResult.Consumed : ChallengeConsumeResult.InvalidOrExpired;
    }

    public async Task<bool> ConsumeChallengeAndUpdatePasswordAsync(
        int authenticationIdentityId,
        int userId,
        string purposeCode,
        string tokenHash,
        string newPasswordHash,
        DateTime changedAtUtc,
        string revocationReason,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(purposeCode)
            || string.IsNullOrWhiteSpace(tokenHash)
            || string.IsNullOrWhiteSpace(newPasswordHash)
            || string.IsNullOrWhiteSpace(revocationReason))
            return false;

        await using var transaction = await unitOfWork.CreateTransactionAsync(cancellationToken);
        var consumed = await unitOfWork.GetRepo<AuthChallenge>().Query()
            .Where(x => x.AuthenticationIdentityId == authenticationIdentityId
                && x.AuthChallengePurpose!.Code == purposeCode
                && !x.AuthChallengePurpose.ObsoleteFlag
                && x.TokenHash == tokenHash
                && x.ConsumedAtUtc == null
                && x.ExpiresAtUtc > changedAtUtc)
            .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.ConsumedAtUtc, changedAtUtc), cancellationToken);
        if (consumed != 1)
            return false;

        var identity = await unitOfWork.GetRepo<AuthenticationIdentity>().Query()
            .SingleOrDefaultAsync(x => x.Id == authenticationIdentityId, cancellationToken);
        if (identity is null || identity.UserId != userId)
            return false;

        await unitOfWork.GetRepo<PasswordHistoryEntry>().AddAsync(new PasswordHistoryEntry
        {
            AuthenticationIdentityId = identity.Id,
            PasswordHash = identity.PasswordHash,
            CreatedAtUtc = changedAtUtc,
        }, cancellationToken);
        identity.PasswordHash = newPasswordHash;
        identity.FailedLoginAttempts = 0;
        identity.LastFailedLoginAtUtc = null;
        identity.LockedUntilUtc = null;
        await unitOfWork.GetRepo<RefreshSession>().Query()
            .Where(x => x.UserId == userId && x.RevokedAtUtc == null && x.ExpiresAtUtc > changedAtUtc)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.RevokedAtUtc, changedAtUtc)
                .SetProperty(x => x.RevocationReason, revocationReason), cancellationToken);
        await SaveAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return true;
    }

    public async Task<FailedLoginResult> RecordFailedLoginAsync(
        int authenticationIdentityId,
        DateTime nowUtc,
        int lockoutThreshold,
        TimeSpan lockoutWindow,
        CancellationToken cancellationToken = default)
    {
        if (lockoutThreshold < 1 || lockoutWindow <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(lockoutThreshold));

        var identityRepository = unitOfWork.GetRepo<AuthenticationIdentity>();
        var identityExists = await identityRepository.Query()
            .AnyAsync(x => x.Id == authenticationIdentityId, cancellationToken);
        if (!identityExists)
            throw new AuthPersistenceConflictException(new InvalidOperationException("Authentication identity was not found."));

        var resetBoundaryUtc = nowUtc.Subtract(lockoutWindow);
        var lockoutUntilUtc = nowUtc.Add(lockoutWindow);
        var updated = await identityRepository.Query()
            .Where(x => x.Id == authenticationIdentityId
                && (x.LockedUntilUtc == null || x.LockedUntilUtc <= nowUtc))
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(
                    x => x.FailedLoginAttempts,
                    x => x.LastFailedLoginAtUtc == null || x.LastFailedLoginAtUtc <= resetBoundaryUtc
                        ? 1
                        : x.FailedLoginAttempts + 1)
                .SetProperty(x => x.LastFailedLoginAtUtc, nowUtc)
                .SetProperty(
                    x => x.LockedUntilUtc,
                    x => x.LastFailedLoginAtUtc == null || x.LastFailedLoginAtUtc <= resetBoundaryUtc
                        ? lockoutThreshold == 1 ? lockoutUntilUtc : (DateTime?)null
                        : x.FailedLoginAttempts + 1 >= lockoutThreshold
                            ? lockoutUntilUtc
                            : (DateTime?)null),
                cancellationToken);

        var current = await identityRepository.Query()
            .AsNoTracking()
            .SingleAsync(x => x.Id == authenticationIdentityId, cancellationToken);
        return new FailedLoginResult(current.FailedLoginAttempts, current.LockedUntilUtc, current.LastFailedLoginAtUtc);
    }

    public async Task ClearLockoutAsync(int authenticationIdentityId, CancellationToken cancellationToken = default)
    {
        var updated = await unitOfWork.GetRepo<AuthenticationIdentity>().Query()
            .Where(x => x.Id == authenticationIdentityId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.FailedLoginAttempts, 0)
                .SetProperty(x => x.LastFailedLoginAtUtc, (DateTime?)null)
                .SetProperty(x => x.LockedUntilUtc, (DateTime?)null), cancellationToken);
        if (updated == 0)
            throw new AuthPersistenceConflictException(new InvalidOperationException("Authentication identity was not found."));
    }

    public async Task<bool> MarkEmailVerifiedAsync(
        int authenticationIdentityId,
        DateTime verifiedAtUtc,
        CancellationToken cancellationToken = default)
    {
        var updated = await unitOfWork.GetRepo<AuthenticationIdentity>().Query()
            .Where(x => x.Id == authenticationIdentityId && !x.EmailVerified)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.EmailVerified, true)
                .SetProperty(x => x.EmailVerifiedAtUtc, verifiedAtUtc), cancellationToken);
        return updated == 1;
    }
    public Task<AuthRegistrationIdempotency?> FindRegistrationIdempotencyAsync(
        string normalizedEmail,
        string idempotencyKey,
        CancellationToken ct = default
    ) =>
        unitOfWork
            .GetRepo<AuthRegistrationIdempotency>()
            .Query()
            .SingleOrDefaultAsync(
                x => x.NormalizedEmail == normalizedEmail && x.IdempotencyKey == idempotencyKey,
                ct
            );

    public Task AddRegistrationIdempotencyAsync(
        AuthRegistrationIdempotency record,
        CancellationToken ct = default
    ) => unitOfWork.GetRepo<AuthRegistrationIdempotency>().AddAsync(record, ct);

    public Task<AuthenticationIdentity?> FindIdentityAsync(
        string email,
        CancellationToken ct = default
    )
    {
        return unitOfWork
            .GetRepo<AuthenticationIdentity>()
            .Query()
            .SingleOrDefaultAsync(x => x.NormalizedEmail == email, ct);
    }

    public Task<AuthenticationIdentity?> FindIdentityForUserIdAsync(int userId, CancellationToken ct = default) =>
        unitOfWork.GetRepo<AuthenticationIdentity>().Query().SingleOrDefaultAsync(x => x.UserId == userId, ct);

    public async Task AddAsync(
        AuthenticationIdentity identity,
        RefreshSession session,
        IReadOnlyCollection<ConsentRecord> consents,
        IReadOnlyCollection<AuthAuditEvent> audits,
        IReadOnlyCollection<AuthOutboxMessage> outbox,
        CancellationToken ct = default
    )
    {
        try
        {
            await using var transaction = await unitOfWork.CreateTransactionAsync(ct);
            await unitOfWork.GetRepo<AuthenticationIdentity>().AddAsync(identity, ct);
            await unitOfWork.GetRepo<RefreshSession>().AddAsync(session, ct);
            foreach (var consent in consents)
                await unitOfWork.GetRepo<ConsentRecord>().AddAsync(consent, ct);
            foreach (var audit in audits)
                await unitOfWork.GetRepo<AuthAuditEvent>().AddAsync(audit, ct);
            foreach (var message in outbox)
                await unitOfWork.GetRepo<AuthOutboxMessage>().AddAsync(message, ct);
            await SaveAsync(ct);
            await transaction.CommitAsync(ct);
        }
        catch (DbUpdateException exception)
        {
            throw new AuthPersistenceConflictException(exception);
        }
    }

    public Task<RefreshSession?> FindSessionAsync(string hash, CancellationToken ct = default)
    {
        return unitOfWork
            .GetRepo<RefreshSession>()
            .Query()
            .SingleOrDefaultAsync(x => x.TokenHash == hash, ct);
    }

    public Task AddSessionAsync(RefreshSession session, CancellationToken ct = default)
    {
        return unitOfWork.GetRepo<RefreshSession>().AddAsync(session, ct);
    }

    public async Task<bool> TryRotateSessionAsync(
        int sessionId,
        DateTime rotatedAtUtc,
        RefreshSession replacement,
        CancellationToken ct = default
    )
    {
        await using var transaction = await unitOfWork.CreateTransactionAsync(ct);
        var updated = await unitOfWork
            .GetRepo<RefreshSession>()
            .Query()
            .Where(session =>
                session.Id == sessionId
                && session.RotatedAtUtc == null
                && session.RevokedAtUtc == null
                && session.ExpiresAtUtc > rotatedAtUtc
            )
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(session => session.RotatedAtUtc, rotatedAtUtc),
                ct
            );

        if (updated != 1)
            return false;

        await unitOfWork.GetRepo<RefreshSession>().AddAsync(replacement, ct);
        await SaveAsync(ct);
        await transaction.CommitAsync(ct);
        return true;
    }

    public async Task RevokeFamilyAsync(
        Guid familyId,
        DateTime revokedAtUtc,
        CancellationToken ct = default
    )
    {
        await unitOfWork
            .GetRepo<RefreshSession>()
            .Query()
            .Where(x => x.FamilyId == familyId && x.RevokedAtUtc == null)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(x => x.RevokedAtUtc, revokedAtUtc),
                ct
            );
    }

    public Task AddOutboxAsync(AuthOutboxMessage message, CancellationToken ct = default)
    {
        return unitOfWork.GetRepo<AuthOutboxMessage>().AddAsync(message, ct);
    }

    public async Task<IReadOnlyList<AuthOutboxMessage>> ClaimDueOutboxAsync(
        DateTime nowUtc,
        DateTime leaseExpiresAtUtc,
        int take,
        CancellationToken ct = default
    )
    {
        var repository = unitOfWork.GetRepo<AuthOutboxMessage>();
        var candidateIds = await repository
            .Query()
            .Where(x =>
                x.ProcessedAtUtc == null
                && x.AvailableAtUtc <= nowUtc
                && (x.LeaseExpiresAtUtc == null || x.LeaseExpiresAtUtc <= nowUtc)
            )
            .OrderBy(x => x.AvailableAtUtc)
            .ThenBy(x => x.App_CreateDate)
            .Take(take)
            .Select(x => x.Id)
            .ToListAsync(ct);
        var claimed = new List<AuthOutboxMessage>(candidateIds.Count);

        foreach (var messageId in candidateIds)
        {
            var leaseId = Guid.NewGuid();
            var updated = await repository
                .Query()
                .Where(x =>
                    x.Id == messageId
                    && x.ProcessedAtUtc == null
                    && x.AvailableAtUtc <= nowUtc
                    && (x.LeaseExpiresAtUtc == null || x.LeaseExpiresAtUtc <= nowUtc)
                )
                .ExecuteUpdateAsync(
                    setters =>
                        setters
                            .SetProperty(x => x.LeaseId, leaseId)
                            .SetProperty(x => x.LeaseExpiresAtUtc, leaseExpiresAtUtc),
                    ct
                );
            if (updated != 1)
                continue;

            var message = await repository
                .Query()
                .AsNoTracking()
                .SingleAsync(x => x.Id == messageId && x.LeaseId == leaseId, ct);
            claimed.Add(message);
        }

        return claimed;
    }

    public async Task<bool> MarkOutboxProcessedAsync(
        int messageId,
        Guid leaseId,
        DateTime processedAtUtc,
        CancellationToken ct = default
    )
    {
        var updated = await unitOfWork
            .GetRepo<AuthOutboxMessage>()
            .Query()
            .Where(x => x.Id == messageId && x.ProcessedAtUtc == null && x.LeaseId == leaseId)
            .ExecuteUpdateAsync(
                setters =>
                    setters
                        .SetProperty(x => x.ProcessedAtUtc, processedAtUtc)
                        .SetProperty(x => x.LeaseId, (Guid?)null)
                        .SetProperty(x => x.LeaseExpiresAtUtc, (DateTime?)null),
                ct
            );
        return updated == 1;
    }

    public async Task MarkOutboxFailedAsync(
        int messageId,
        Guid leaseId,
        int attempts,
        DateTime availableAtUtc,
        string safeError,
        CancellationToken ct = default
    )
    {
        await unitOfWork
            .GetRepo<AuthOutboxMessage>()
            .Query()
            .Where(x => x.Id == messageId && x.ProcessedAtUtc == null && x.LeaseId == leaseId)
            .ExecuteUpdateAsync(
                setters =>
                    setters
                        .SetProperty(x => x.Attempts, attempts)
                        .SetProperty(x => x.AvailableAtUtc, availableAtUtc)
                        .SetProperty(x => x.LastError, safeError)
                        .SetProperty(x => x.LeaseId, (Guid?)null)
                        .SetProperty(x => x.LeaseExpiresAtUtc, (DateTime?)null),
                ct
            );
    }

    public Task AddAuditAsync(AuthAuditEvent audit, CancellationToken ct = default)
    {
        return unitOfWork.GetRepo<AuthAuditEvent>().AddAsync(audit, ct);
    }

    public Task<RefreshSession?> FindSessionBySessionKeyAsync(Guid sessionKey, CancellationToken ct = default) =>
        unitOfWork.GetRepo<RefreshSession>().Query().SingleOrDefaultAsync(x => x.SessionKey == sessionKey, ct);

    public async Task<bool> TryRecordAuditRetryAsync(AuthAuditRetryPayload payload, CancellationToken ct = default)
    {
        await using var transaction = await unitOfWork.DbContext.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            ct);

        var auditRepository = unitOfWork.GetRepo<AuthAuditEvent>();
        var exists = await auditRepository
            .Query()
            .AsNoTracking()
            .AnyAsync(audit =>
                audit.EventType == payload.EventType
                && audit.UserId == payload.UserId
                && audit.IpAddress == payload.IpAddress
                && audit.MetadataJson == payload.MetadataJson,
                ct);
        if (exists)
        {
            await transaction.CommitAsync(ct);
            return false;
        }

        await auditRepository.AddAsync(new AuthAuditEvent
        {
            EventType = payload.EventType,
            UserId = payload.UserId,
            IpAddress = payload.IpAddress,
            MetadataJson = payload.MetadataJson,
        }, ct);
        await SaveAsync(ct);
        await transaction.CommitAsync(ct);
        return true;
    }

    public async Task<RefreshRotationResult> TryRotateSessionIfCurrentAsync(
        int sessionId,
        int userId,
        Guid sessionKey,
        DateTime rotatedAtUtc,
        RefreshSession replacement,
        CancellationToken ct = default)
    {
        // Account-state mutations are owned by Profiling, but Auth already has the
        // established identity relationship to that aggregate. Serializable
        // evaluation keeps a deactivation/deletion from committing between the
        // state decision and refresh-session rotation.
        await using var transaction = await unitOfWork.DbContext.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            ct);
        var identity = await unitOfWork.GetRepo<AuthenticationIdentity>().Query()
            .Include(x => x.User)
            .SingleOrDefaultAsync(x => x.UserId == userId, ct);
        if (identity is null
            || !identity.EmailVerified
            || identity.LockedUntilUtc > rotatedAtUtc
            || identity.User is null
            || identity.User.IsDeactivated
            || identity.User.IsDeleted)
            return RefreshRotationResult.CurrentStateInvalid;

        var updated = await unitOfWork
            .GetRepo<RefreshSession>()
            .Query()
            .Where(session =>
                session.Id == sessionId
                && session.UserId == userId
                && session.SessionKey == sessionKey
                && session.RotatedAtUtc == null
                && session.RevokedAtUtc == null
                && session.ExpiresAtUtc > rotatedAtUtc)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(session => session.RotatedAtUtc, rotatedAtUtc),
                ct);

        if (updated != 1)
            return RefreshRotationResult.Rejected;

        await unitOfWork.GetRepo<RefreshSession>().AddAsync(replacement, ct);
        await SaveAsync(ct);
        await transaction.CommitAsync(ct);
        return RefreshRotationResult.Rotated;
    }

    public Task QueueAuditRetryAsync(AuthAuditRetryPayload payload, CancellationToken ct = default)
    {
        foreach (var entry in unitOfWork.DbContext.ChangeTracker.Entries<AuthAuditEvent>())
            entry.State = EntityState.Detached;

        return AddOutboxAsync(new AuthOutboxMessage
        {
            Type = "auth-audit-retry",
            PayloadJson = JsonSerializer.Serialize(payload),
            AvailableAtUtc = (timeProvider ?? TimeProvider.System).GetUtcNow().UtcDateTime,
        }, ct);
    }

    public Task SaveAsync(CancellationToken ct = default)
    {
        return unitOfWork.SaveAsync(ct);
    }
}
