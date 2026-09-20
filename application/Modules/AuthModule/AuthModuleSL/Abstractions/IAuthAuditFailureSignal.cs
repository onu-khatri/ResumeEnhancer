namespace ResumeEnhancer.AuthModule.SL.Abstractions;

public interface IAuthAuditFailureSignal
{
    Task SignalAsync(string eventType, bool userIdentifierPresent, CancellationToken cancellationToken = default);
}
