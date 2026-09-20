using ResumeEnhancer.Core.DomainLibrary.DomainModel;

namespace ResumeEnhancer.Infrastructure.Persistence.Limiting;

public sealed class DbCacheLimiterEntry : BusinessEntity
{
    public string KeyHash { get; set; } = string.Empty;
    public int Count { get; set; }
    public DateTime WindowStartedUtc { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
}

public sealed record AtomicLimiterDecision(
    bool Allowed,
    int Count,
    int Limit,
    DateTime ExpiresAtUtc,
    bool Degraded = false)
{
    public TimeSpan RetryAfter(DateTime nowUtc) =>
        ExpiresAtUtc > nowUtc ? ExpiresAtUtc - nowUtc : TimeSpan.Zero;
}

public interface IAtomicLimiterStore
{
    Task<AtomicLimiterDecision> TryConsumeAsync(
        string keyHash,
        int limit,
        TimeSpan window,
        CancellationToken cancellationToken = default);
}
