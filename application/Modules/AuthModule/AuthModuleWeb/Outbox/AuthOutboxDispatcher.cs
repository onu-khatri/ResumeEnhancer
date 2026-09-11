using System.Text.Json;
using ResumeEnhancer.AuthModule.DM.Entities;
using ResumeEnhancer.AuthModule.SL.Abstractions;

namespace ResumeEnhancer.AuthModule.Web.Outbox;

internal sealed record AuthSideEffectPayload(int UserId, string Email);

public sealed class AuthOutboxDispatcher(
    IServiceScopeFactory scopeFactory,
    ILogger<AuthOutboxDispatcher> logger
) : BackgroundService
{
    private const int BatchSize = 25;
    private const int MaxAttempts = 5;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessOnceAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    public async Task<int> ProcessOnceAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<IAuthRepository>();
        var handler = scope.ServiceProvider.GetRequiredService<IAuthSideEffectHandler>();
        var nowUtc = DateTime.UtcNow;
        var messages = await repository.ClaimDueOutboxAsync(
            nowUtc,
            nowUtc.AddMinutes(5),
            BatchSize,
            cancellationToken
        );
        var processed = 0;

        foreach (var message in messages)
        {
            try
            {
                var payload =
                    JsonSerializer.Deserialize<AuthSideEffectPayload>(message.PayloadJson)
                    ?? throw new JsonException("outbox_payload_invalid");
                await handler.HandleAsync(
                    message.Type,
                    payload.UserId,
                    payload.Email,
                    cancellationToken
                );
                await repository.MarkOutboxProcessedAsync(
                    message.Id,
                    message.LeaseId!.Value,
                    DateTime.UtcNow,
                    cancellationToken
                );
                await repository.SaveAsync(cancellationToken);
                processed++;
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                var attempts = message.Attempts + 1;
                var exhausted = attempts >= MaxAttempts;
                var delay = exhausted
                    ? TimeSpan.MaxValue
                    : TimeSpan.FromMinutes(Math.Min(60, Math.Pow(2, Math.Min(attempts, 6))));
                var safeError =
                    exception is JsonException ? "side_effect_payload_invalid"
                    : exhausted ? "side_effect_delivery_exhausted"
                    : "side_effect_delivery_failed";
                await repository.MarkOutboxFailedAsync(
                    message.Id,
                    message.LeaseId!.Value,
                    attempts,
                    exhausted ? DateTime.MaxValue : DateTime.UtcNow.Add(delay),
                    safeError,
                    cancellationToken
                );
                await repository.AddAuditAsync(
                    new AuthAuditEvent
                    {
                        UserId = TryGetUserId(message.PayloadJson),
                        EventType = "side_effect_failed",
                        MetadataJson = JsonSerializer.Serialize(
                            new
                            {
                                message.Type,
                                attempts,
                                retryable = !exhausted,
                            }
                        ),
                    },
                    cancellationToken
                );
                await repository.SaveAsync(cancellationToken);
                logger.LogWarning(
                    "Auth side effect {SideEffectType} failed on attempt {Attempt}; retry scheduled.",
                    message.Type,
                    attempts
                );
            }
        }

        return processed;
    }

    private static int? TryGetUserId(string payload)
    {
        try
        {
            return JsonSerializer.Deserialize<AuthSideEffectPayload>(payload)?.UserId;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}

public sealed class LoggingAuthSideEffectHandler(ILogger<LoggingAuthSideEffectHandler> logger)
    : IAuthSideEffectHandler
{
    public Task HandleAsync(
        string type,
        int userId,
        string email,
        CancellationToken cancellationToken = default
    )
    {
        logger.LogInformation(
            "Auth side effect {SideEffectType} dispatched for user {UserId}.",
            type,
            userId
        );
        return Task.CompletedTask;
    }
}
