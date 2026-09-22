using ResumeEnhancer.AuthModule.DM.Entities;
using ResumeEnhancer.AuthModule.SL.Abstractions;
using ResumeEnhancer.AuthModule.SL.Options;

namespace ResumeEnhancer.AuthModule.SL.Services;

public sealed class AuthSecurityStateService(
    IAuthRepository repository,
    IPasswordHasher passwordHasher,
    IProgressiveLoginDelayPolicy delayPolicy,
    AuthSecurityOptions options) : IAuthSecurityStateService
{
    public Task<IReadOnlyList<PasswordHistoryEntry>> GetRecentPasswordHistoryAsync(
        int authenticationIdentityId,
        CancellationToken cancellationToken = default) =>
        repository.GetPasswordHistoryAsync(authenticationIdentityId, 2, cancellationToken);

    public async Task<bool> IsPasswordReusedAsync(
        int authenticationIdentityId,
        string candidatePassword,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(candidatePassword))
            return false;

        var identity = await repository.FindIdentityByIdAsync(authenticationIdentityId, cancellationToken);
        if (identity is null)
            return false;

        if (passwordHasher.Verify(identity.PasswordHash, candidatePassword))
            return true;

        var history = await GetRecentPasswordHistoryAsync(authenticationIdentityId, cancellationToken);
        return history.Any(entry => passwordHasher.Verify(entry.PasswordHash, candidatePassword));
    }

    public async Task<FailedLoginDecision> RecordFailedLoginAsync(
        int authenticationIdentityId,
        DateTime nowUtc,
        CancellationToken cancellationToken = default)
    {
        var result = await repository.RecordFailedLoginAsync(
            authenticationIdentityId,
            nowUtc,
            lockoutThreshold: options.LockoutThreshold,
            lockoutWindow: options.LockoutWindow,
            cancellationToken);
        return new FailedLoginDecision(
            result.FailedAttempts,
            result.LockedUntilUtc,
            delayPolicy.Calculate(result.FailedAttempts));
    }
}
