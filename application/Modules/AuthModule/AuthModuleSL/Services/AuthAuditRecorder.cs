using ResumeEnhancer.AuthModule.DM.Entities;
using ResumeEnhancer.AuthModule.SL.Abstractions;
using ResumeEnhancer.Core.CommonLibrary.Resilience;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ResumeEnhancer.AuthModule.SL.Services;

public sealed class AuthAuditRecorder(
    IAuthRepository repository,
    IAuthAuditFailureSignal failureSignal,
    IResilienceExecutor resilienceExecutor) : IAuthAuditRecorder
{
    public async Task RecordAsync(
        string eventType,
        int? userId,
        string? ipAddress,
        string metadataJson = "{}",
        CancellationToken cancellationToken = default,
        string? correlationId = null)
    {
        var logicalCorrelationId = string.IsNullOrWhiteSpace(correlationId)
            ? Activity.Current?.Id ?? Guid.NewGuid().ToString("N")
            : correlationId;
        var redactedMetadata = AddCorrelation(metadataJson, logicalCorrelationId);

        try
        {
            await resilienceExecutor.ExecuteAsync(
                "AuditOutbox",
                ResilienceOperation.AuditDelivery,
                async token =>
                {
                    await repository.AddAuditAsync(new AuthAuditEvent
                    {
                        EventType = eventType,
                        UserId = userId,
                        IpAddress = ipAddress,
                        MetadataJson = redactedMetadata,
                    }, token);
                    await repository.SaveAsync(token);
                },
                cancellationToken: cancellationToken);
        }
        catch (Exception) when (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await repository.QueueAuditRetryAsync(new AuthAuditRetryPayload(
                    eventType,
                    userId,
                    ipAddress,
                    redactedMetadata,
                    logicalCorrelationId), cancellationToken);
                await repository.SaveAsync(cancellationToken);
            }
            catch (Exception) when (!cancellationToken.IsCancellationRequested)
            {
                // The protected request remains denied; operational signaling is safe and redacted.
            }

            try
            {
                await failureSignal.SignalAsync(eventType, userId.HasValue, cancellationToken);
            }
            catch (Exception) when (!cancellationToken.IsCancellationRequested)
            {
                // Operational signaling is secondary to the protected request and must not escape.
            }
        }
    }

    private static string AddCorrelation(string metadataJson, string correlationId)
    {
        try
        {
            var node = JsonNode.Parse(string.IsNullOrWhiteSpace(metadataJson) ? "{}" : metadataJson);
            if (node is JsonObject metadata)
            {
                metadata["correlationId"] = correlationId;
                return metadata.ToJsonString();
            }
        }
        catch (JsonException)
        {
            // Invalid internal metadata is retained as redacted data below.
        }

        return JsonSerializer.Serialize(new { correlationId, metadata = metadataJson });
    }
}
