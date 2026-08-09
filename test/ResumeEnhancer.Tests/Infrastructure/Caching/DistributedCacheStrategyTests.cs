using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;
using Shouldly;
using Caching;

namespace ResumeEnhancer.Tests.Infrastructure.Caching;

public sealed class DistributedCacheStrategyTests
{
    [Fact]
    public async Task GetAsync_DelegatesToDistributedCache()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var distributedCache = Substitute.For<IDistributedCache>();
        distributedCache.GetAsync("key", cancellationToken).Returns([1, 2, 3]);
        var strategy = new DistributedCacheStrategy(distributedCache);

        var value = await strategy.GetAsync("key", cancellationToken);

        strategy.ProviderName.ShouldBe("Distributed");
        value.ShouldBe([1, 2, 3]);
    }

    [Fact]
    public async Task SetAsync_WithExpiration_PassesAbsoluteExpirationOption()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var distributedCache = Substitute.For<IDistributedCache>();
        var strategy = new DistributedCacheStrategy(distributedCache);

        await strategy.SetAsync("key", [4, 5], TimeSpan.FromMinutes(3), cancellationToken);

        await distributedCache.Received(1).SetAsync(
            "key",
            Arg.Is<byte[]>(bytes => bytes != null && bytes.SequenceEqual(new byte[] { 4, 5 })),
            Arg.Is<DistributedCacheEntryOptions>(options =>
                options != null &&
                options.AbsoluteExpirationRelativeToNow == TimeSpan.FromMinutes(3)),
            cancellationToken);
    }

    [Fact]
    public async Task SetAsync_WithoutExpiration_PassesEmptyOptions()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var distributedCache = Substitute.For<IDistributedCache>();
        var strategy = new DistributedCacheStrategy(distributedCache);

        await strategy.SetAsync("key", [6], null, cancellationToken);

        await distributedCache.Received(1).SetAsync(
            "key",
            Arg.Is<byte[]>(bytes => bytes != null && bytes.SequenceEqual(new byte[] { 6 })),
            Arg.Is<DistributedCacheEntryOptions>(options =>
                options != null &&
                options.AbsoluteExpirationRelativeToNow == null),
            cancellationToken);
    }

    [Fact]
    public async Task RemoveAsync_DelegatesToDistributedCache()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var distributedCache = Substitute.For<IDistributedCache>();
        var strategy = new DistributedCacheStrategy(distributedCache);

        await strategy.RemoveAsync("key", cancellationToken);

        await distributedCache.Received(1).RemoveAsync("key", cancellationToken);
    }
}
