using System.Globalization;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Options;
using ResumeEnhancer.Core.CommonLibrary.Resilience;
using ResumeEnhancer.Infrastructure.Persistence.Limiting;

namespace ResumeEnhancer.Infrastructure.Caching.Atomic;

internal sealed class MemCacheAtomicLimiterStore(
    IOptions<CacheOptions> options,
    IResilienceExecutor resilienceExecutor,
    TimeProvider timeProvider) : IAtomicLimiterStore
{
    // MemCache exposes add/incr as separate atomic commands. This bounded loop
    // only repairs the provider-native expiry race between those two commands;
    // policy retries for provider failures are owned by IResilienceExecutor.
    private const int MaxExpiryRaceRecoveries = 2;

    public async Task<AtomicLimiterDecision> TryConsumeAsync(
        string keyHash,
        int limit,
        TimeSpan window,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var key = $"resume-enhancer:limiter:{keyHash}";
        var expiration = GetExpirationSeconds(window);

        return await resilienceExecutor.ExecuteAsync(
            "LimiterProvider",
            ResilienceOperation.Provider,
            token => TryConsumeProviderAsync(key, limit, window, expiration, token),
            cancellationToken: cancellationToken);
    }

    private async Task<AtomicLimiterDecision> TryConsumeProviderAsync(
        string key,
        int limit,
        TimeSpan window,
        long expiration,
        CancellationToken cancellationToken)
    {
        for (var recovery = 0; recovery <= MaxExpiryRaceRecoveries; recovery++)
        {
            using var client = await CreateClientAsync(key, cancellationToken);
            var stream = client.GetStream();
            await WriteAsciiAsync(stream, $"add {key} 0 {expiration} 1\r\n1\r\n", cancellationToken);
            var addResponse = await ReadLineAsync(stream, cancellationToken);

            long count;
            if (addResponse == "STORED")
            {
                count = 1;
            }
            else if (addResponse == "NOT_STORED")
            {
                await WriteAsciiAsync(stream, $"incr {key} 1\r\n", cancellationToken);
                var incrementResponse = await ReadLineAsync(stream, cancellationToken);
                if (incrementResponse == "NOT_FOUND" && recovery < MaxExpiryRaceRecoveries)
                    continue;
                if (!long.TryParse(incrementResponse, NumberStyles.None, CultureInfo.InvariantCulture, out count))
                    throw new InvalidOperationException("MemCache atomic increment failed.");
            }
            else
            {
                throw new InvalidOperationException("MemCache atomic add failed.");
            }

            return new AtomicLimiterDecision(
                count <= limit,
                checked((int)Math.Min(count, int.MaxValue)),
                limit,
                timeProvider.GetUtcNow().UtcDateTime.Add(window));
        }

        throw new InvalidOperationException("MemCache limiter expiry race did not recover within the bounded protocol budget.");
    }

    private async Task<TcpClient> CreateClientAsync(string key, CancellationToken cancellationToken)
    {
        var servers = options.Value.MemCache.Servers
            .Where(server => !string.IsNullOrWhiteSpace(server.Host) && server.Port > 0)
            .ToArray();
        if (servers.Length == 0)
            throw new InvalidOperationException("At least one MemCache server must be configured.");

        var server = servers[(int)(GetStableHash(key) % (uint)servers.Length)];
        var client = new TcpClient();
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(options.Value.MemCache.ConnectTimeout);
        await client.ConnectAsync(server.Host, server.Port, timeout.Token);
        client.ReceiveTimeout = ToMilliseconds(options.Value.MemCache.ReceiveTimeout);
        client.SendTimeout = ToMilliseconds(options.Value.MemCache.ConnectTimeout);
        return client;
    }

    private static long GetExpirationSeconds(TimeSpan window) =>
        Math.Max(1, (long)Math.Ceiling(window.TotalSeconds));

    private static uint GetStableHash(string value)
    {
        const uint offsetBasis = 2166136261;
        const uint prime = 16777619;
        var hash = offsetBasis;
        foreach (var character in value)
        {
            hash ^= character;
            hash *= prime;
        }
        return hash;
    }

    private static int ToMilliseconds(TimeSpan value) =>
        value <= TimeSpan.Zero ? Timeout.Infinite : (int)Math.Min(value.TotalMilliseconds, int.MaxValue);

    private static Task WriteAsciiAsync(NetworkStream stream, string value, CancellationToken cancellationToken) =>
        stream.WriteAsync(Encoding.ASCII.GetBytes(value).AsMemory(), cancellationToken).AsTask();

    private static async Task<string> ReadLineAsync(NetworkStream stream, CancellationToken cancellationToken)
    {
        var bytes = new List<byte>();
        var buffer = new byte[1];
        while (true)
        {
            var read = await stream.ReadAsync(buffer, cancellationToken);
            if (read == 0) throw new InvalidOperationException("Connection closed while reading from MemCache.");
            if (buffer[0] == '\n') break;
            if (buffer[0] != '\r') bytes.Add(buffer[0]);
            if (bytes.Count > 4096) throw new InvalidOperationException("MemCache response line exceeded 4096 bytes.");
        }
        return Encoding.ASCII.GetString(bytes.ToArray());
    }
}
