using Microsoft.Extensions.Caching.Memory;
using Shouldly;
using Caching;

namespace ResumeEnhancer.Tests.Infrastructure.Caching;

public sealed class InMemoryCacheStrategyTests
{
    [Fact]
    public async Task GetAsync_KeyMissing_ReturnsNull()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var strategy = new InMemoryCacheStrategy(memoryCache);

        var value = await strategy.GetAsync("missing", cancellationToken);

        value.ShouldBeNull();
        strategy.ProviderName.ShouldBe("InMemory");
    }

    [Fact]
    public async Task SetAsync_KeyExists_StoresDefensiveCopy()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var strategy = new InMemoryCacheStrategy(memoryCache);
        var bytes = new byte[] { 1, 2, 3 };

        await strategy.SetAsync("key", bytes, TimeSpan.FromMinutes(5), cancellationToken);
        bytes[0] = 9;
        var cached = await strategy.GetAsync("key", cancellationToken);
        cached.ShouldBe([1, 2, 3]);

        cached![1] = 8;
        var cachedAgain = await strategy.GetAsync("key", cancellationToken);

        cachedAgain.ShouldBe([1, 2, 3]);
    }

    [Fact]
    public async Task RemoveAsync_KeyExists_RemovesValue()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var strategy = new InMemoryCacheStrategy(memoryCache);
        await strategy.SetAsync("key", [1], null, cancellationToken);

        await strategy.RemoveAsync("key", cancellationToken);
        var value = await strategy.GetAsync("key", cancellationToken);

        value.ShouldBeNull();
    }

    [Fact]
    public async Task GetAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var strategy = new InMemoryCacheStrategy(memoryCache);
        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        await Should.ThrowAsync<OperationCanceledException>(
            () => strategy.GetAsync("key", cancellationTokenSource.Token));
    }
}
