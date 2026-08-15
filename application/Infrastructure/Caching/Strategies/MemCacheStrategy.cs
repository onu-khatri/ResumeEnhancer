using System.Globalization;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Options;

namespace ResumeEnhancer.Infrastructure.Caching;

internal sealed class MemCacheStrategy(IOptions<CacheOptions> options) : ICacheStrategy
{
    private static readonly byte[] CrLf = "\r\n"u8.ToArray();

    public string ProviderName => "MemCache";

    public async Task<byte[]?> GetAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        using var client = await CreateClientAsync(key, cancellationToken);
        var stream = client.GetStream();

        await WriteAsciiAsync(stream, $"get {key}\r\n", cancellationToken);

        var firstLine = await ReadLineAsync(stream, cancellationToken);

        if (firstLine == "END")
        {
            return null;
        }

        var parts = firstLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 4 || parts[0] != "VALUE")
        {
            throw new InvalidOperationException($"Unexpected MemCache response '{firstLine}'.");
        }

        var byteCount = int.Parse(parts[3], CultureInfo.InvariantCulture);
        var value = new byte[byteCount];
        await stream.ReadExactlyAsync(value, cancellationToken);
        await ReadExpectedCrLfAsync(stream, cancellationToken);

        var endLine = await ReadLineAsync(stream, cancellationToken);

        if (endLine != "END")
        {
            throw new InvalidOperationException($"Unexpected MemCache response '{endLine}'.");
        }

        return value;
    }

    public async Task SetAsync(
        string key,
        byte[] value,
        TimeSpan? absoluteExpirationRelativeToNow,
        CancellationToken cancellationToken = default)
    {
        using var client = await CreateClientAsync(key, cancellationToken);
        var stream = client.GetStream();
        var expiration = GetExpirationSeconds(absoluteExpirationRelativeToNow);

        await WriteAsciiAsync(stream, $"set {key} 0 {expiration} {value.Length}\r\n", cancellationToken);
        await stream.WriteAsync(value, cancellationToken);
        await stream.WriteAsync(CrLf, cancellationToken);

        var response = await ReadLineAsync(stream, cancellationToken);

        if (response != "STORED")
        {
            throw new InvalidOperationException($"Unable to store MemCache value. Response: '{response}'.");
        }
    }

    public async Task RemoveAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        using var client = await CreateClientAsync(key, cancellationToken);
        var stream = client.GetStream();

        await WriteAsciiAsync(stream, $"delete {key}\r\n", cancellationToken);

        var response = await ReadLineAsync(stream, cancellationToken);

        if (response is not "DELETED" and not "NOT_FOUND")
        {
            throw new InvalidOperationException($"Unable to delete MemCache value. Response: '{response}'.");
        }
    }

    private async Task<TcpClient> CreateClientAsync(
        string key,
        CancellationToken cancellationToken)
    {
        var server = GetServer(key);
        var cacheOptions = options.Value.MemCache;
        var client = new TcpClient();

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(cacheOptions.ConnectTimeout);

        await client.ConnectAsync(server.Host, server.Port, timeout.Token);

        client.ReceiveTimeout = ToMilliseconds(cacheOptions.ReceiveTimeout);
        client.SendTimeout = ToMilliseconds(cacheOptions.ConnectTimeout);

        return client;
    }

    private MemCacheServerOptions GetServer(string key)
    {
        var servers = options.Value.MemCache.Servers
            .Where(server => !string.IsNullOrWhiteSpace(server.Host) && server.Port > 0)
            .ToArray();

        if (servers.Length == 0)
        {
            throw new InvalidOperationException("At least one MemCache server must be configured.");
        }

        var index = (int)(GetStableHash(key) % (uint)servers.Length);
        return servers[index];
    }

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

    private static int ToMilliseconds(TimeSpan value)
    {
        if (value <= TimeSpan.Zero)
        {
            return Timeout.Infinite;
        }

        return value.TotalMilliseconds > int.MaxValue
            ? int.MaxValue
            : (int)value.TotalMilliseconds;
    }

    private static long GetExpirationSeconds(TimeSpan? absoluteExpirationRelativeToNow)
    {
        if (!absoluteExpirationRelativeToNow.HasValue)
        {
            return 0;
        }

        if (absoluteExpirationRelativeToNow.Value <= TimeSpan.Zero)
        {
            return 1;
        }

        var seconds = (long)Math.Ceiling(absoluteExpirationRelativeToNow.Value.TotalSeconds);

        return seconds <= 2_592_000
            ? seconds
            : DateTimeOffset.UtcNow.Add(absoluteExpirationRelativeToNow.Value).ToUnixTimeSeconds();
    }

    private static Task WriteAsciiAsync(
        NetworkStream stream,
        string value,
        CancellationToken cancellationToken)
    {
        var bytes = Encoding.ASCII.GetBytes(value);
        return stream.WriteAsync(bytes.AsMemory(), cancellationToken).AsTask();
    }

    private static async Task<string> ReadLineAsync(
        NetworkStream stream,
        CancellationToken cancellationToken)
    {
        var bytes = new List<byte>();
        var buffer = new byte[1];

        while (true)
        {
            var read = await stream.ReadAsync(buffer, cancellationToken);

            if (read == 0)
            {
                throw new InvalidOperationException("Connection closed while reading from MemCache.");
            }

            if (buffer[0] == '\n')
            {
                break;
            }

            if (buffer[0] != '\r')
            {
                bytes.Add(buffer[0]);
            }

            if (bytes.Count > 4096)
            {
                throw new InvalidOperationException("MemCache response line exceeded 4096 bytes.");
            }
        }

        return Encoding.ASCII.GetString(bytes.ToArray());
    }

    private static async Task ReadExpectedCrLfAsync(
        NetworkStream stream,
        CancellationToken cancellationToken)
    {
        var bytes = new byte[2];
        await stream.ReadExactlyAsync(bytes, cancellationToken);

        if (bytes[0] != '\r' || bytes[1] != '\n')
        {
            throw new InvalidOperationException("Expected CRLF in MemCache response.");
        }
    }
}

