namespace ResumeEnhancer.Infrastructure.Caching;

public sealed class MemCacheOptions
{
    public IList<MemCacheServerOptions> Servers { get; set; } =
    [
        new MemCacheServerOptions()
    ];

    public TimeSpan ConnectTimeout { get; set; } = TimeSpan.FromSeconds(2);

    public TimeSpan ReceiveTimeout { get; set; } = TimeSpan.FromSeconds(2);
}

