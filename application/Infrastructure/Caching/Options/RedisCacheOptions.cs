namespace ResumeEnhancer.Infrastructure.Caching;

public sealed class RedisCacheOptions
{
    public string Configuration { get; set; } = "localhost:6379";

    public string InstanceName { get; set; } = "ResumeEnhancer:";
}

