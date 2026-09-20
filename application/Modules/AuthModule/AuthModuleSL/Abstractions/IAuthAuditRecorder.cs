namespace ResumeEnhancer.AuthModule.SL.Abstractions;

public interface IAuthAuditRecorder
{
    Task RecordAsync(
        string eventType,
        int? userId,
        string? ipAddress,
        string metadataJson = "{}",
        CancellationToken cancellationToken = default,
        string? correlationId = null);
}
