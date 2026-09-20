using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ResumeEnhancer.Infrastructure.Caching.Atomic;
using ResumeEnhancer.Infrastructure.Persistence.Limiting;
using StackExchange.Redis;

namespace ResumeEnhancer.Infrastructure.Caching;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationCaching(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection(CacheOptions.SectionName);
        var cacheOptions = section.Get<CacheOptions>() ?? new CacheOptions();
        CacheOptionsValidator.Validate(cacheOptions);

        services.Configure<CacheOptions>(section);
        services.TryAddSingleton<ICacheSerializer, SystemTextJsonCacheSerializer>();
        services.TryAddSingleton<ICacheKeyFormatter, Sha256CacheKeyFormatter>();
        services.TryAddSingleton<ICacheProvider, ApplicationCacheProvider>();

        AddProviderStrategy(services, cacheOptions);

        return services;
    }

    private static void AddProviderStrategy(
        IServiceCollection services,
        CacheOptions options)
    {
        switch (options.Provider)
        {
            case CacheProviderType.InMemory:
                services.AddMemoryCache();
                services.TryAddSingleton<ICacheStrategy, InMemoryCacheStrategy>();
                break;

            case CacheProviderType.Redis:
                services.TryAddSingleton<IConnectionMultiplexer>(_ =>
                    ConnectionMultiplexer.Connect(options.Redis.Configuration));
                services.AddStackExchangeRedisCache(redisOptions =>
                {
                    redisOptions.Configuration = options.Redis.Configuration;
                    redisOptions.InstanceName = options.Redis.InstanceName;
                });
                services.TryAddSingleton<ICacheStrategy, DistributedCacheStrategy>();
                services.Replace(ServiceDescriptor.Scoped<IAtomicLimiterStore, RedisAtomicLimiterStore>());
                break;

            case CacheProviderType.MemCache:
                services.TryAddSingleton<ICacheStrategy, MemCacheStrategy>();
                services.Replace(ServiceDescriptor.Scoped<IAtomicLimiterStore, MemCacheAtomicLimiterStore>());
                break;

            default:
                throw new InvalidOperationException($"Unsupported cache provider '{options.Provider}'.");
        }
    }
}

