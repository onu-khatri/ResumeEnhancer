namespace ResumeEnhancer.Infrastructure.Caching;

public sealed class MemCacheServerOptions
{
    public string Host { get; set; } = "localhost";

    public int Port { get; set; } = 11211;
}

