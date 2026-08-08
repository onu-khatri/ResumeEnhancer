using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Caching;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationCaching(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection(CacheOptions.SectionName);
        var cacheOptions = section.Get<CacheOptions>() ?? new CacheOptions();

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
                services.AddStackExchangeRedisCache(redisOptions =>
                {
                    redisOptions.Configuration = options.Redis.Configuration;
                    redisOptions.InstanceName = options.Redis.InstanceName;
                });
                services.TryAddSingleton<ICacheStrategy, DistributedCacheStrategy>();
                break;

            case CacheProviderType.MemCache:
                services.TryAddSingleton<ICacheStrategy, MemCacheStrategy>();
                break;

            default:
                throw new InvalidOperationException($"Unsupported cache provider '{options.Provider}'.");
        }
    }
}
