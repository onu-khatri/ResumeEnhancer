using System.Globalization;
using ResumeEnhancer.Core.CommonLibrary.Resilience;
using ResumeEnhancer.Infrastructure.Persistence.Limiting;
using StackExchange.Redis;

namespace ResumeEnhancer.Infrastructure.Caching.Atomic;

internal sealed class RedisAtomicLimiterStore(
    IConnectionMultiplexer connection,
    IResilienceExecutor resilienceExecutor,
    TimeProvider timeProvider) : IAtomicLimiterStore
{
    private const string Script = """
        local count = redis.call('INCR', KEYS[1])
        local ttl = redis.call('PTTL', KEYS[1])
        if count == 1 or ttl <= 0 then redis.call('PEXPIRE', KEYS[1], ARGV[1]) end
        ttl = redis.call('PTTL', KEYS[1])
        return { count, ttl }
        """;

    public async Task<AtomicLimiterDecision> TryConsumeAsync(
        string keyHash,
        int limit,
        TimeSpan window,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var database = connection.GetDatabase();
        var rawResult = await resilienceExecutor.ExecuteAsync(
            "LimiterProvider",
            ResilienceOperation.Provider,
            token => database.ScriptEvaluateAsync(
                Script,
                [new RedisKey($"resume-enhancer:limiter:{keyHash}")],
                [new RedisValue(Math.Max(1L, (long)window.TotalMilliseconds).ToString(CultureInfo.InvariantCulture))])
                .WaitAsync(token),
            cancellationToken: cancellationToken);
        if (rawResult.IsNull)
            throw new InvalidOperationException("Redis atomic limiter returned no decision.");
        var result = (RedisResult[])rawResult!;
        if (result.Length < 2)
            throw new InvalidOperationException("Redis atomic limiter returned an incomplete decision.");

        var count = (int)result[0];
        var ttlMilliseconds = Math.Max(0, (long)result[1]);
        return new AtomicLimiterDecision(
            count <= limit,
            count,
            limit,
            timeProvider.GetUtcNow().UtcDateTime.AddMilliseconds(ttlMilliseconds));
    }
}
