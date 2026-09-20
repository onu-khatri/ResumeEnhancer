namespace ResumeEnhancer.AuthModule.DM.Entities;

public sealed record AuthAuditRetryPayload(
    string EventType,
    int? UserId,
    string? IpAddress,
    string MetadataJson,
    string CorrelationId);
