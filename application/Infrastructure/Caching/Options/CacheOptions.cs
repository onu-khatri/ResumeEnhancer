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

