using Microsoft.Extensions.Logging;
using ResumeEnhancer.AuthModule.SL.Abstractions;

namespace ResumeEnhancer.AuthModule.SL.Services;

internal sealed class AuthAuditFailureSignal(ILogger<AuthAuditFailureSignal> logger) : IAuthAuditFailureSignal
{
    public Task SignalAsync(string eventType, bool userIdentifierPresent, CancellationToken cancellationToken = default)
    {
        if (!cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning(
                "Auth audit persistence failed; retry required. EventType={EventType} UserIdentifierPresent={UserIdentifierPresent}",
                eventType,
                userIdentifierPresent);
        }

        return Task.CompletedTask;
    }
}
