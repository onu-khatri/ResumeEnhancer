using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Options;
using Shouldly;
using ResumeEnhancer.Infrastructure.Caching;

namespace ResumeEnhancer.Tests.Unit.Infrastructure.Caching;

public sealed class MemCacheStrategyTests
{
    [Fact]
    public async Task GetAsync_NoConfiguredServers_ThrowsInvalidOperationException()
    {
        var strategy = new MemCacheStrategy(Options.Create(new CacheOptions
        {
            MemCache = new MemCacheOptions { Servers = [] }
        }));

        await Should.ThrowAsync<InvalidOperationException>(
            () => strategy.GetAsync("key", TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task GetAsync_MissingKey_ReturnsNullAndSendsGetCommand()
    {
        string? command = null;
        byte[]? result = null;

        await RunExchangeAsync(
            async stream =>
            {
                command = await ReadLineAsync(stream);
                await WriteAsciiAsync(stream, "END\r\n");
            },
            async strategy => result = await strategy.GetAsync("cache-key", TestContext.Current.CancellationToken));

        result.ShouldBeNull();
        command.ShouldBe("get cache-key");
    }

    [Fact]
    public async Task GetAsync_ValueResponse_ReturnsPayload()
    {
        byte[]? result = null;

        await RunExchangeAsync(
            async stream =>
            {
                (await ReadLineAsync(stream)).ShouldBe("get cache-key");
                await WriteAsciiAsync(stream, "VALUE cache-key 0 3\r\nabc\r\nEND\r\n");
            },
            async strategy => result = await strategy.GetAsync("cache-key", TestContext.Current.CancellationToken));

        result.ShouldBe(Encoding.ASCII.GetBytes("abc"));
    }

    [Fact]
    public async Task GetAsync_UnexpectedResponse_ThrowsInvalidOperationException()
    {
        await RunExchangeAsync(
            async stream =>
            {
                await ReadLineAsync(stream);
                await WriteAsciiAsync(stream, "ERROR\r\n");
            },
            async strategy => await Should.ThrowAsync<InvalidOperationException>(
                () => strategy.GetAsync("cache-key", TestContext.Current.CancellationToken)));
    }

    [Fact]
    public async Task GetAsync_ValueResponseWithoutEnd_ThrowsInvalidOperationException()
    {
        await RunExchangeAsync(
            async stream =>
            {
                await ReadLineAsync(stream);
                await WriteAsciiAsync(stream, "VALUE cache-key 0 1\r\nx\r\nBROKEN\r\n");
            },
            async strategy => await Should.ThrowAsync<InvalidOperationException>(
                () => strategy.GetAsync("cache-key", TestContext.Current.CancellationToken)));
    }

    [Fact]
    public async Task SetAsync_ExpirationBranches_SendExpectedExpirationSeconds()
    {
        await SetCommandShouldContainExpirationAsync(null, " 0 ");
        await SetCommandShouldContainExpirationAsync(TimeSpan.Zero, " 1 ");
        await SetCommandShouldContainExpirationAsync(TimeSpan.FromMilliseconds(1200), " 2 ");
        await SetCommandShouldContainUnixExpirationAsync(TimeSpan.FromDays(31));
    }

    [Fact]
    public async Task SetAsync_StoreFailure_ThrowsInvalidOperationException()
    {
        await RunExchangeAsync(
            async stream =>
            {
                await ReadSetCommandAndPayloadAsync(stream);
                await WriteAsciiAsync(stream, "NOT_STORED\r\n");
            },
            async strategy => await Should.ThrowAsync<InvalidOperationException>(
                () => strategy.SetAsync("cache-key", [1], TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken)));
    }

    [Fact]
    public async Task RemoveAsync_DeletedAndNotFoundResponses_AreAccepted()
    {
        await DeleteResponseShouldCompleteAsync("DELETED");
        await DeleteResponseShouldCompleteAsync("NOT_FOUND");
    }

    [Fact]
    public async Task RemoveAsync_UnexpectedResponse_ThrowsInvalidOperationException()
    {
        await RunExchangeAsync(
            async stream =>
            {
                (await ReadLineAsync(stream)).ShouldBe("delete cache-key");
                await WriteAsciiAsync(stream, "ERROR\r\n");
            },
            async strategy => await Should.ThrowAsync<InvalidOperationException>(
                () => strategy.RemoveAsync("cache-key", TestContext.Current.CancellationToken)));
    }

    private static async Task SetCommandShouldContainExpirationAsync(
        TimeSpan? expiration,
        string expectedFragment)
    {
        string? command = null;

        await RunExchangeAsync(
            async stream =>
            {
                command = await ReadSetCommandAndPayloadAsync(stream);
                await WriteAsciiAsync(stream, "STORED\r\n");
            },
            async strategy => await strategy.SetAsync(
                "cache-key",
                Encoding.ASCII.GetBytes("value"),
                expiration,
                TestContext.Current.CancellationToken));

        command!.ShouldContain(expectedFragment);
    }

    private static async Task SetCommandShouldContainUnixExpirationAsync(TimeSpan expiration)
    {
        string? command = null;

        await RunExchangeAsync(
            async stream =>
            {
                command = await ReadSetCommandAndPayloadAsync(stream);
                await WriteAsciiAsync(stream, "STORED\r\n");
            },
            async strategy => await strategy.SetAsync(
                "cache-key",
                [1],
                expiration,
                TestContext.Current.CancellationToken));

        var parts = command!.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        long.Parse(parts[3]).ShouldBeGreaterThan(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
    }

    private static async Task DeleteResponseShouldCompleteAsync(string response)
    {
        string? command = null;

        await RunExchangeAsync(
            async stream =>
            {
                command = await ReadLineAsync(stream);
                await WriteAsciiAsync(stream, $"{response}\r\n");
            },
            async strategy => await strategy.RemoveAsync("cache-key", TestContext.Current.CancellationToken));

        command.ShouldBe("delete cache-key");
    }

    private static async Task RunExchangeAsync(
        Func<NetworkStream, Task> handleConnection,
        Func<MemCacheStrategy, Task> exerciseStrategy)
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        var serverTask = Task.Run(async () =>
        {
            using var client = await listener.AcceptTcpClientAsync();
            using var stream = client.GetStream();
            await handleConnection(stream);
        });
        var strategy = new MemCacheStrategy(Options.Create(new CacheOptions
        {
            MemCache = new MemCacheOptions
            {
                ConnectTimeout = TimeSpan.FromSeconds(2),
                ReceiveTimeout = TimeSpan.FromSeconds(2),
                Servers =
                [
                    new MemCacheServerOptions
                    {
                        Host = IPAddress.Loopback.ToString(),
                        Port = port
                    }
                ]
            }
        }));

        try
        {
            strategy.ProviderName.ShouldBe("MemCache");
            await exerciseStrategy(strategy);
            await serverTask;
        }
        finally
        {
            listener.Stop();
        }
    }

    private static async Task<string> ReadSetCommandAndPayloadAsync(NetworkStream stream)
    {
        var command = await ReadLineAsync(stream);
        var bytes = int.Parse(command.Split(' ', StringSplitOptions.RemoveEmptyEntries)[4]);
        var payload = new byte[bytes];
        await stream.ReadExactlyAsync(payload);
        var crlf = new byte[2];
        await stream.ReadExactlyAsync(crlf);
        crlf.ShouldBe(Encoding.ASCII.GetBytes("\r\n"));

        return command;
    }

    private static async Task<string> ReadLineAsync(NetworkStream stream)
    {
        var bytes = new List<byte>();
        var buffer = new byte[1];

        while (true)
        {
            var read = await stream.ReadAsync(buffer);

            if (read == 0 || buffer[0] == '\n')
            {
                break;
            }

            if (buffer[0] != '\r')
            {
                bytes.Add(buffer[0]);
            }
        }

        return Encoding.ASCII.GetString(bytes.ToArray());
    }

    private static Task WriteAsciiAsync(NetworkStream stream, string value) =>
        stream.WriteAsync(Encoding.ASCII.GetBytes(value)).AsTask();
}


