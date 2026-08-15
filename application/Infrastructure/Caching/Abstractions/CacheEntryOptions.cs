namespace ResumeEnhancer.Infrastructure.Caching;

public sealed class CacheEntryOptions
{
    public static CacheEntryOptions NoExpiration { get; } = new()
    {
        NeverExpire = true
    };

    public TimeSpan? AbsoluteExpirationRelativeToNow { get; init; }

    public bool NeverExpire { get; init; }
}

