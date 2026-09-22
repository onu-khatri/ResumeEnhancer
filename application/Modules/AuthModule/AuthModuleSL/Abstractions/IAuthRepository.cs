using ResumeEnhancer.AuthModule.DM.Entities;

namespace ResumeEnhancer.AuthModule.SL.Abstractions;

public sealed class AuthPersistenceConflictException(Exception innerException)
    : Exception("The authentication record conflicts with an existing record.", innerException) { }

public interface IAuthRepository
{
    public Task<IReadOnlyList<AuthSigningKeyMetadata>> GetSigningKeyMetadataAsync(CancellationToken cancellationToken = default);
    public Task<bool> TryRotateSigningKeyAsync(
        string expectedKeyIdentifier,
        long expectedLifecycleVersion,
        AuthSigningKeyMetadata replacement,
        DateTime retiredAtUtc,
        CancellationToken cancellationToken = default) => Task.FromResult(false);
    public Task InvalidateSigningKeyAsync(string keyIdentifier, DateTime invalidatedAtUtc, CancellationToken cancellationToken = default);
    public Task<AuthenticationIdentity?> FindIdentityByIdAsync(
        int authenticationIdentityId,
        CancellationToken cancellationToken = default);
    public Task<IReadOnlyList<PasswordHistoryEntry>> GetPasswordHistoryAsync(
        int authenticationIdentityId,
        int take = 2,
        CancellationToken cancellationToken = default);
    public Task UpdatePasswordAndRevokeSessionsAsync(
        int authenticationIdentityId,
        int userId,
        string newPasswordHash,
        DateTime changedAtUtc,
        string revocationReason,
        CancellationToken cancellationToken = default);
    public Task AddChallengeAsync(
        AuthChallenge challenge,
        CancellationToken cancellationToken = default);
    public Task<int?> FindActiveChallengePurposeIdAsync(
        string purposeCode,
        CancellationToken cancellationToken = default);
    public Task<ChallengeConsumeResult> TryConsumeChallengeAsync(
        int authenticationIdentityId,
        string purposeCode,
        string tokenHash,
        DateTime nowUtc,
        CancellationToken cancellationToken = default);
    public Task<bool> ConsumeChallengeAndUpdatePasswordAsync(
        int authenticationIdentityId,
        int userId,
        string purposeCode,
        string tokenHash,
        string newPasswordHash,
        DateTime changedAtUtc,
        string revocationReason,
        CancellationToken cancellationToken = default);
    public Task<FailedLoginResult> RecordFailedLoginAsync(
        int authenticationIdentityId,
        DateTime nowUtc,
        int lockoutThreshold,
        TimeSpan lockoutWindow,
        CancellationToken cancellationToken = default);
    public Task ClearLockoutAsync(
        int authenticationIdentityId,
        CancellationToken cancellationToken = default);
    public Task<bool> MarkEmailVerifiedAsync(
        int authenticationIdentityId,
        DateTime verifiedAtUtc,
        CancellationToken cancellationToken = default) => Task.FromResult(false);
    public Task<AuthenticationIdentity?> FindIdentityAsync(
        string normalizedEmail,
        CancellationToken cancellationToken = default
    );
    public Task<AuthenticationIdentity?> FindIdentityForUserIdAsync(
        int userId,
        CancellationToken cancellationToken = default) => Task.FromResult<AuthenticationIdentity?>(null);
    public Task<AuthRegistrationIdempotency?> FindRegistrationIdempotencyAsync(
        string normalizedEmail,
        string idempotencyKey,
        CancellationToken cancellationToken = default
    );
    public Task AddRegistrationIdempotencyAsync(
        AuthRegistrationIdempotency record,
        CancellationToken cancellationToken = default
    );
    public Task AddAsync(
        AuthenticationIdentity identity,
        RefreshSession session,
        IReadOnlyCollection<ConsentRecord> consents,
        IReadOnlyCollection<AuthAuditEvent> audits,
        IReadOnlyCollection<AuthOutboxMessage> outbox,
        CancellationToken cancellationToken = default
    );
    public Task<RefreshSession?> FindSessionAsync(
        string tokenHash,
        CancellationToken cancellationToken = default
    );
    public Task<RefreshSession?> FindSessionBySessionKeyAsync(Guid sessionKey, CancellationToken cancellationToken = default);
    public Task AddSessionAsync(
        RefreshSession session,
        CancellationToken cancellationToken = default
    );
    public Task<bool> TryRotateSessionAsync(
        int sessionId,
        DateTime rotatedAtUtc,
        RefreshSession replacement,
        CancellationToken cancellationToken = default
    );
    public async Task<RefreshRotationResult> TryRotateSessionIfCurrentAsync(
        int sessionId,
        int userId,
        Guid sessionKey,
        DateTime rotatedAtUtc,
        RefreshSession replacement,
        CancellationToken cancellationToken = default)
        => await TryRotateSessionAsync(sessionId, rotatedAtUtc, replacement, cancellationToken)
            ? RefreshRotationResult.Rotated
            : RefreshRotationResult.Rejected;
    public Task RevokeFamilyAsync(
        Guid familyId,
        DateTime revokedAtUtc,
        CancellationToken cancellationToken = default
    );
    public Task AddOutboxAsync(
        AuthOutboxMessage message,
        CancellationToken cancellationToken = default
    );
    public Task<IReadOnlyList<AuthOutboxMessage>> ClaimDueOutboxAsync(
        DateTime nowUtc,
        DateTime leaseExpiresAtUtc,
        int take,
        CancellationToken cancellationToken = default
    );
    public Task<bool> MarkOutboxProcessedAsync(
        int messageId,
        Guid leaseId,
        DateTime processedAtUtc,
        CancellationToken cancellationToken = default
    );
    public Task MarkOutboxFailedAsync(
        int messageId,
        Guid leaseId,
        int attempts,
        DateTime availableAtUtc,
        string safeError,
        CancellationToken cancellationToken = default
    );
    public Task AddAuditAsync(AuthAuditEvent audit, CancellationToken cancellationToken = default);
    public Task<bool> TryRecordAuditRetryAsync(
        AuthAuditRetryPayload payload,
        CancellationToken cancellationToken = default);
    public Task QueueAuditRetryAsync(AuthAuditRetryPayload payload, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task SaveAsync(CancellationToken cancellationToken = default);
}

public enum ChallengeConsumeResult
{
    Consumed,
    InvalidOrExpired,
}

public sealed record FailedLoginResult(
    int FailedAttempts,
    DateTime? LockedUntilUtc,
    DateTime? LastFailedAtUtc)
{
    public bool IsLocked => LockedUntilUtc is not null;
    public bool CrossedLockoutThreshold => FailedAttempts > 0 && LockedUntilUtc is not null;
}

public enum RefreshRotationResult
{
    Rotated,
    Rejected,
    CurrentStateInvalid,
}
