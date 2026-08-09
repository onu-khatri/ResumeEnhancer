using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using Caching;

namespace ResumeEnhancer.Tests.Infrastructure.Caching;

public sealed class ApplicationCacheProviderTests
{
    [Fact]
    public async Task GetAsync_StrategyMiss_ReturnsCacheMiss()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var cacheStrategy = new RecordingCacheStrategy();
        var provider = CreateProvider(cacheStrategy: cacheStrategy);

        var result = await provider.GetAsync<CachedItem>("key", cancellationToken);

        result.Found.ShouldBeFalse();
        result.Value.ShouldBeNull();
        cacheStrategy.GetKeys.ShouldBe(["formatted:key"]);
    }

    [Fact]
    public async Task GetAsync_StrategyHit_DeserializesValue()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var provider = CreateProvider();
        await provider.SetAsync("key", new CachedItem("hit"), cancellationToken: cancellationToken);

        var result = await provider.GetAsync<CachedItem>("key", cancellationToken);

        result.Found.ShouldBeTrue();
        result.Value.ShouldBe(new CachedItem("hit"));
    }

    [Fact]
    public async Task GetOrSetAsync_CachedValueExists_DoesNotCallFactory()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var provider = CreateProvider();
        await provider.SetAsync("key", new CachedItem("cached"), cancellationToken: cancellationToken);

        var value = await provider.GetOrSetAsync(
            "key",
            _ => Task.FromResult(new CachedItem("factory")),
            cancellationToken: cancellationToken);

        value.ShouldBe(new CachedItem("cached"));
    }

    [Fact]
    public async Task GetOrSetAsync_CachedValueMissing_CallsFactoryAndStoresValue()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var provider = CreateProvider();

        var value = await provider.GetOrSetAsync(
            "key",
            _ => Task.FromResult(new CachedItem("created")),
            cancellationToken: cancellationToken);
        var cached = await provider.GetAsync<CachedItem>("key", cancellationToken);

        value.ShouldBe(new CachedItem("created"));
        cached.Value.ShouldBe(new CachedItem("created"));
    }

    [Fact]
    public async Task GetOrSetAsync_FactoryIsNull_ThrowsArgumentNullException()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var provider = CreateProvider();

        await Should.ThrowAsync<ArgumentNullException>(
            () => provider.GetOrSetAsync<CachedItem>("key", null!, cancellationToken: cancellationToken));
    }

    [Fact]
    public async Task SetAsync_EntryNeverExpires_PassesNullExpiration()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var cacheStrategy = new RecordingCacheStrategy();
        var provider = CreateProvider(cacheStrategy: cacheStrategy);

        await provider.SetAsync(
            "key",
            new CachedItem("value"),
            CacheEntryOptions.NoExpiration,
            cancellationToken);

        cacheStrategy.SetCalls.ShouldHaveSingleItem();
        cacheStrategy.SetCalls[0].Key.ShouldBe("formatted:key");
        cacheStrategy.SetCalls[0].Expiration.ShouldBeNull();
    }

    [Fact]
    public async Task SetAsync_DefaultExpirationIsZero_PassesNullExpiration()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var cacheStrategy = new RecordingCacheStrategy();
        var provider = CreateProvider(
            cacheStrategy,
            new CacheOptions { DefaultExpiration = TimeSpan.Zero });

        await provider.SetAsync("key", new CachedItem("value"), cancellationToken: cancellationToken);

        cacheStrategy.SetCalls.ShouldHaveSingleItem();
        cacheStrategy.SetCalls[0].Key.ShouldBe("formatted:key");
        cacheStrategy.SetCalls[0].Expiration.ShouldBeNull();
    }

    [Fact]
    public async Task RemoveAsync_FormatsKeyBeforeRemoving()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var cacheStrategy = new RecordingCacheStrategy();
        var provider = CreateProvider(cacheStrategy: cacheStrategy);

        await provider.RemoveAsync("key", cancellationToken);

        cacheStrategy.RemoveKeys.ShouldBe(["formatted:key"]);
    }

    private static ApplicationCacheProvider CreateProvider(
        ICacheStrategy? cacheStrategy = null,
        CacheOptions? options = null) =>
        new(
            cacheStrategy ?? new DictionaryCacheStrategy(),
            new SystemTextJsonCacheSerializer(),
            new PrefixKeyFormatter(),
            Options.Create(options ?? new CacheOptions { DefaultExpiration = TimeSpan.FromMinutes(10) }));

    private sealed record CachedItem(string Value);

    private sealed class PrefixKeyFormatter : ICacheKeyFormatter
    {
        public string Format(string key) => $"formatted:{key}";
    }

    private sealed class DictionaryCacheStrategy : ICacheStrategy
    {
        private readonly Dictionary<string, byte[]> _values = [];

        public string ProviderName => "Test";

        public Task<byte[]?> GetAsync(string key, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(_values.TryGetValue(key, out var value) ? value : null);
        }

        public Task SetAsync(
            string key,
            byte[] value,
            TimeSpan? absoluteExpirationRelativeToNow,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _values[key] = value;
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _values.Remove(key);
            return Task.CompletedTask;
        }
    }

    private sealed class RecordingCacheStrategy : ICacheStrategy
    {
        public List<string> GetKeys { get; } = [];

        public List<SetCall> SetCalls { get; } = [];

        public List<string> RemoveKeys { get; } = [];

        public string ProviderName => "Recording";

        public Task<byte[]?> GetAsync(string key, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            GetKeys.Add(key);

            return Task.FromResult<byte[]?>(null);
        }

        public Task SetAsync(
            string key,
            byte[] value,
            TimeSpan? absoluteExpirationRelativeToNow,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            SetCalls.Add(new SetCall(key, value, absoluteExpirationRelativeToNow));

            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            RemoveKeys.Add(key);

            return Task.CompletedTask;
        }
    }

    private sealed record SetCall(string Key, byte[] Value, TimeSpan? Expiration);
}
