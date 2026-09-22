using System.Text.Json;
using ResumeEnhancer.AuthModule.DM.Entities;
using ResumeEnhancer.AuthModule.SL.Abstractions;

namespace ResumeEnhancer.AuthModule.Web.Outbox;

public sealed class AuthOutboxDispatcher(
    IServiceScopeFactory scopeFactory,
    ILogger<AuthOutboxDispatcher> logger,
    TimeProvider timeProvider
) : BackgroundService
{
    private const int BatchSize = 25;
    private const int MaxAttempts = 5;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOnceAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(
                    exception,
                    "Auth side-effect outbox processing failed; retrying on the next cycle.");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), timeProvider, stoppingToken);
        }
    }

    public async Task<int> ProcessOnceAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<IAuthRepository>();
        var handler = scope.ServiceProvider.GetRequiredService<IAuthSideEffectHandler>();
        var recorder = scope.ServiceProvider.GetRequiredService<IAuthAuditRecorder>();
        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;
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
                if (message.Type == "auth-audit-retry")
                {
                    var audit = JsonSerializer.Deserialize<AuthAuditRetryPayload>(message.PayloadJson)
                        ?? throw new JsonException("audit_payload_invalid");
                    await repository.TryRecordAuditRetryAsync(audit, cancellationToken);

                    var acknowledged = await repository.MarkOutboxProcessedAsync(
                        message.Id,
                        message.LeaseId!.Value,
                        timeProvider.GetUtcNow().UtcDateTime,
                        cancellationToken);
                    if (!acknowledged)
                        continue;
                    await repository.SaveAsync(cancellationToken);
                }
                else
                {
                var payload =
                    JsonSerializer.Deserialize<AuthSideEffectPayload>(message.PayloadJson)
                    ?? throw new JsonException("outbox_payload_invalid");
                var challenge = payload.Challenge;
                var expectedPurpose = ExpectedPurpose(message.Type);
                if ((expectedPurpose is not null && challenge is null)
                    || (challenge is not null
                    && (challenge.Version != 1
                        || string.IsNullOrWhiteSpace(challenge.Purpose)
                        || !string.Equals(expectedPurpose, challenge.Purpose, StringComparison.Ordinal)
                        || challenge.UserId != payload.UserId
                        || !string.Equals(challenge.Email, payload.Email, StringComparison.OrdinalIgnoreCase)
                        || string.IsNullOrWhiteSpace(challenge.ProtectedValue))))
                    throw new JsonException("challenge_envelope_invalid");
                await handler.HandleAsync(
                    message.Type,
                    payload.UserId,
                    payload.Email,
                    challenge is null
                        ? null
                        : scope.ServiceProvider.GetRequiredService<IAuthChallengeDeliveryProtector>()
                            .Unprotect(challenge.ProtectedValue, challenge.Purpose, challenge.UserId, challenge.Email),
                    cancellationToken
                );
                var acknowledged = await repository.MarkOutboxProcessedAsync(
                    message.Id,
                    message.LeaseId!.Value,
                    timeProvider.GetUtcNow().UtcDateTime,
                    cancellationToken
                );
                if (!acknowledged)
                    continue;
                await repository.SaveAsync(cancellationToken);
                }
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
                try
                {
                    await repository.MarkOutboxFailedAsync(
                        message.Id,
                        message.LeaseId!.Value,
                        attempts,
                        exhausted ? DateTime.MaxValue : timeProvider.GetUtcNow().UtcDateTime.Add(delay),
                        safeError,
                        cancellationToken);
                    await repository.SaveAsync(cancellationToken);
                }
                catch (Exception failure) when (failure is not OperationCanceledException)
                {
                    logger.LogWarning("Auth side effect retry state could not be persisted for message {MessageId}.", message.Id);
                }

                try
                {
                    await recorder.RecordAsync(
                        "side_effect_failed",
                        TryGetUserId(message.PayloadJson),
                        null,
                        JsonSerializer.Serialize(new { message.Type, attempts, retryable = !exhausted }),
                        cancellationToken,
                        $"outbox:{message.Id}");
                }
                catch (Exception failure) when (failure is not OperationCanceledException)
                {
                    logger.LogWarning("Auth side effect failure audit could not be persisted for message {MessageId}.", message.Id);
                }
                logger.LogWarning(
                    "Auth side effect {SideEffectType} failed on attempt {Attempt}; retry scheduled.",
                    message.Type,
                    attempts
                );
            }
        }

        return processed;
    }

    private static string? ExpectedPurpose(string messageType) => messageType switch
    {
        "verification-email" => "email-verification",
        "password-reset-email" => "password-reset",
        _ => null,
    };

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
        string? challenge,
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
