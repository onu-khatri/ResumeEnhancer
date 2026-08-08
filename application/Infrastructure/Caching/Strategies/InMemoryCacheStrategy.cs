using Microsoft.Extensions.Caching.Memory;

namespace Caching;

internal sealed class InMemoryCacheStrategy(IMemoryCache memoryCache) : ICacheStrategy
{
    public string ProviderName => "InMemory";

    public Task<byte[]?> GetAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var value = memoryCache.TryGetValue<byte[]>(key, out var cachedBytes)
            ? cachedBytes?.ToArray()
            : null;

        return Task.FromResult(value);
    }

    public Task SetAsync(
        string key,
        byte[] value,
        TimeSpan? absoluteExpirationRelativeToNow,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var options = new MemoryCacheEntryOptions();

        if (absoluteExpirationRelativeToNow.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
        }

        memoryCache.Set(key, value.ToArray(), options);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        memoryCache.Remove(key);
        return Task.CompletedTask;
    }
}
