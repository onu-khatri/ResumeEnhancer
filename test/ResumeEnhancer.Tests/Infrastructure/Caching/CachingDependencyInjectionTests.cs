using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using ResumeEnhancer.Infrastructure.Caching;

namespace ResumeEnhancer.Tests.Unit.Infrastructure.Caching;

public sealed class CachingDependencyInjectionTests
{
    [Fact]
    public void AddApplicationCaching_DefaultConfiguration_RegistersInMemoryProvider()
    {
        var services = new ServiceCollection();

        services.AddApplicationCaching(new ConfigurationBuilder().Build());
        using var provider = services.BuildServiceProvider();

        provider.GetRequiredService<ICacheProvider>().ShouldBeOfType<ApplicationCacheProvider>();
        provider.GetRequiredService<ICacheStrategy>().ShouldBeOfType<InMemoryCacheStrategy>();
        provider.GetRequiredService<ICacheSerializer>().ShouldBeOfType<SystemTextJsonCacheSerializer>();
        provider.GetRequiredService<ICacheKeyFormatter>().ShouldBeOfType<Sha256CacheKeyFormatter>();
    }

    [Fact]
    public void AddApplicationCaching_RedisProvider_RegistersDistributedStrategy()
    {
        var services = new ServiceCollection();
        var configuration = ConfigurationFrom(
            (CacheOptions.SectionName + ":Provider", "Redis"),
            (CacheOptions.SectionName + ":Redis:Configuration", "localhost:6379"));

        services.AddApplicationCaching(configuration);
        using var provider = services.BuildServiceProvider();

        provider.GetRequiredService<ICacheStrategy>().ShouldBeOfType<DistributedCacheStrategy>();
    }

    [Fact]
    public void AddApplicationCaching_MemCacheProvider_RegistersMemCacheStrategy()
    {
        var services = new ServiceCollection();
        var configuration = ConfigurationFrom((CacheOptions.SectionName + ":Provider", "MemCache"));

        services.AddApplicationCaching(configuration);
        using var provider = services.BuildServiceProvider();

        provider.GetRequiredService<ICacheStrategy>().ShouldBeOfType<MemCacheStrategy>();
    }

    [Fact]
    public void AddApplicationCaching_UnsupportedProvider_ThrowsInvalidOperationException()
    {
        var services = new ServiceCollection();
        var configuration = ConfigurationFrom((CacheOptions.SectionName + ":Provider", "999"));

        Should.Throw<InvalidOperationException>(() => services.AddApplicationCaching(configuration));
    }

    private static IConfiguration ConfigurationFrom(params (string Key, string Value)[] values) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(values.ToDictionary(pair => pair.Key, pair => pair.Value)!)
            .Build();
}


