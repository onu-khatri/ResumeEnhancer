using ResumeEnhancer.AuthModule.DM.Entities;

namespace ResumeEnhancer.AuthModule.SL.Abstractions;

public sealed class AuthPersistenceConflictException(Exception innerException)
    : Exception("The authentication record conflicts with an existing record.", innerException) { }

public interface IAuthRepository
{
    public Task<AuthenticationIdentity?> FindIdentityAsync(
        string normalizedEmail,
        CancellationToken cancellationToken = default
    );
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
    public Task MarkOutboxProcessedAsync(
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
    public Task SaveAsync(CancellationToken cancellationToken = default);
}
