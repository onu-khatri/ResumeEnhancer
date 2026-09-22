using ResumeEnhancer.AuthModule.DM.Entities;
using ResumeEnhancer.AuthModule.SL.Options;

namespace ResumeEnhancer.AuthModule.SL.Abstractions;

public interface IProgressiveLoginDelayPolicy
{
    TimeSpan Calculate(int failedAttempts);
}

public sealed class ProgressiveLoginDelayPolicy(AuthSecurityOptions options) : IProgressiveLoginDelayPolicy
{
    public TimeSpan Calculate(int failedAttempts) => options.ProgressiveLoginDelays[
        Math.Clamp(failedAttempts - 1, 0, options.ProgressiveLoginDelays.Length - 1)];
}

public sealed record FailedLoginDecision(
    int FailedAttempts,
    DateTime? LockedUntilUtc,
    TimeSpan Delay)
{
    public bool IsLocked => LockedUntilUtc is not null;
}

public interface IAuthSecurityStateService
{
    Task<IReadOnlyList<PasswordHistoryEntry>> GetRecentPasswordHistoryAsync(
        int authenticationIdentityId,
        CancellationToken cancellationToken = default);
    Task<bool> IsPasswordReusedAsync(
        int authenticationIdentityId,
        string candidatePassword,
        CancellationToken cancellationToken = default);
    Task<FailedLoginDecision> RecordFailedLoginAsync(
        int authenticationIdentityId,
        DateTime nowUtc,
        CancellationToken cancellationToken = default);
}
