using Microsoft.EntityFrameworkCore;
using ResumeEnhancer.AuthModule.DM.Entities;
using ResumeEnhancer.AuthModule.SL.Abstractions;
using ResumeEnhancer.Infrastructure.Persistence;

namespace ResumeEnhancer.AuthModule.PL.Repositories;

public sealed class AuthRepository(IUnitOfWork<AppDbContext> unitOfWork) : IAuthRepository
{
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

    public async Task MarkOutboxProcessedAsync(
        int messageId,
        Guid leaseId,
        DateTime processedAtUtc,
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
                        .SetProperty(x => x.ProcessedAtUtc, processedAtUtc)
                        .SetProperty(x => x.LeaseId, (Guid?)null)
                        .SetProperty(x => x.LeaseExpiresAtUtc, (DateTime?)null),
                ct
            );
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

    public Task SaveAsync(CancellationToken ct = default)
    {
        return unitOfWork.SaveAsync(ct);
    }
}
