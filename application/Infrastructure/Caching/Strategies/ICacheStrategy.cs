namespace ResumeEnhancer.Infrastructure.Caching;

internal interface ICacheStrategy
{
    string ProviderName { get; }

    Task<byte[]?> GetAsync(
        string key,
        CancellationToken cancellationToken = default);

    Task SetAsync(
        string key,
        byte[] value,
        TimeSpan? absoluteExpirationRelativeToNow,
        CancellationToken cancellationToken = default);

    Task RemoveAsync(
        string key,
        CancellationToken cancellationToken = default);
}

