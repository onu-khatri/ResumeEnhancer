using Microsoft.Extensions.Caching.Distributed;

namespace Caching;

internal sealed class DistributedCacheStrategy(IDistributedCache distributedCache) : ICacheStrategy
{
    public string ProviderName => "Distributed";

    public Task<byte[]?> GetAsync(
        string key,
        CancellationToken cancellationToken = default) =>
        distributedCache.GetAsync(key, cancellationToken);

    public Task SetAsync(
        string key,
        byte[] value,
        TimeSpan? absoluteExpirationRelativeToNow,
        CancellationToken cancellationToken = default)
    {
        var options = new DistributedCacheEntryOptions();

        if (absoluteExpirationRelativeToNow.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
        }

        return distributedCache.SetAsync(key, value, options, cancellationToken);
    }

    public Task RemoveAsync(
        string key,
        CancellationToken cancellationToken = default) =>
        distributedCache.RemoveAsync(key, cancellationToken);
}
