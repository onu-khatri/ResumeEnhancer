namespace ResumeEnhancer.Infrastructure.Caching;

internal interface ICacheKeyFormatter
{
    string Format(string key);
}

