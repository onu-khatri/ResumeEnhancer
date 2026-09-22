using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using ResumeEnhancer.AuthModule.SL.Abstractions;
using ResumeEnhancer.Infrastructure.Persistence.Limiting;

namespace ResumeEnhancer.AuthModule.PL.Repositories;

public sealed class DbCacheRegistrationThrottle(
    IAtomicLimiterStore? store = null,
    ILogger<DbCacheRegistrationThrottle>? logger = null,
    TimeProvider? timeProvider = null) : IRegistrationThrottle
{
    private readonly TimeProvider timeProvider = timeProvider ?? TimeProvider.System;
    private static readonly IReadOnlyDictionary<LimiterOperation, (int Limit, TimeSpan Window)> Policies =
        new Dictionary<LimiterOperation, (int, TimeSpan)>
        {
            [LimiterOperation.Login] = (10, TimeSpan.FromMinutes(15)),
            [LimiterOperation.Registration] = (5, TimeSpan.FromMinutes(15)),
            [LimiterOperation.Refresh] = (20, TimeSpan.FromMinutes(15)),
            [LimiterOperation.Verification] = (5, TimeSpan.FromHours(1)),
            [LimiterOperation.Recovery] = (5, TimeSpan.FromHours(1)),
            [LimiterOperation.PasswordChange] = (10, TimeSpan.FromHours(1)),
            [LimiterOperation.Logout] = (20, TimeSpan.FromMinutes(15)),
        };

    public async Task<bool> IsAllowedAsync(string normalizedEmail, string? ipAddress, CancellationToken cancellationToken = default) =>
        (await TryConsumeAsync(LimiterOperation.Registration, normalizedEmail, null, ipAddress, cancellationToken)).Allowed;

    public async Task<LimiterDecision> TryConsumeAsync(
        LimiterOperation operation,
        string? normalizedEmail,
        string? subject,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        var policy = Policies[operation];
        var material = $"auth-limiter:v1:{operation}:{normalizedEmail ?? string.Empty}:{subject ?? string.Empty}:{ipAddress ?? "unknown"}";
        var keyHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(material))).ToLowerInvariant();
        try
        {
            if (store is null)
                return new LimiterDecision(true, 0, policy.Limit, this.timeProvider.GetUtcNow().UtcDateTime.Add(policy.Window), true);
            var result = await store.TryConsumeAsync(keyHash, policy.Limit, policy.Window, cancellationToken);
            if (result.Degraded)
                logger?.LogWarning("Auth limiter degraded; fail-open decision used. Operation={Operation}", operation);
            return new LimiterDecision(result.Allowed, result.Count, result.Limit, result.ExpiresAtUtc, result.Degraded);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            // The approved outage policy is fail-open. The caller emits a redacted
            // operational signal; provider details never cross the API boundary.
            logger?.LogWarning("Auth limiter unavailable; fail-open decision used. Operation={Operation}", operation);
            return new LimiterDecision(true, 0, policy.Limit, this.timeProvider.GetUtcNow().UtcDateTime.Add(policy.Window), true);
        }
    }
}
