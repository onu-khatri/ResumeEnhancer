namespace ResumeEnhancer.Infrastructure.Caching;

public sealed class CacheOptions
{
    public const string SectionName = "Caching";

    public CacheProviderType Provider { get; set; } = CacheProviderType.InMemory;

    public string KeyPrefix { get; set; } = "ResumeEnhancer";

    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(30);

    public RedisCacheOptions Redis { get; set; } = new();

    public MemCacheOptions MemCache { get; set; } = new();
}

public static class CacheOptionsValidator
{
    public static void Validate(CacheOptions options, bool requireProductionProvider = false)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (string.IsNullOrWhiteSpace(options.KeyPrefix))
            throw new InvalidOperationException("Caching:KeyPrefix is required.");
        if (options.DefaultExpiration <= TimeSpan.Zero)
            throw new InvalidOperationException("Caching:DefaultExpiration must be positive.");

        if (requireProductionProvider && options.Provider == CacheProviderType.InMemory)
            throw new InvalidOperationException("Caching:Provider cannot be InMemory in production.");

        switch (options.Provider)
        {
            case CacheProviderType.Redis when string.IsNullOrWhiteSpace(options.Redis.Configuration):
                throw new InvalidOperationException("Caching:Redis:Configuration is required.");
            case CacheProviderType.MemCache when options.MemCache.Servers is null || options.MemCache.Servers.Count == 0:
                throw new InvalidOperationException("Caching:MemCache:Servers must contain at least one server.");
            case CacheProviderType.MemCache when options.MemCache.Servers.Any(server =>
                string.IsNullOrWhiteSpace(server.Host) || server.Port is <= 0 or > 65535):
                throw new InvalidOperationException("Caching:MemCache:Servers contains an invalid server.");
            case CacheProviderType.Redis:
            case CacheProviderType.MemCache:
            case CacheProviderType.InMemory:
                break;
            default:
                throw new InvalidOperationException($"Unsupported cache provider '{options.Provider}'.");
        }
    }
}

