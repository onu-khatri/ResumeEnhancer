using Microsoft.Extensions.Options;

namespace Caching;

internal sealed class ApplicationCacheProvider(
    ICacheStrategy cacheStrategy,
    ICacheSerializer serializer,
    ICacheKeyFormatter keyFormatter,
    IOptions<CacheOptions> options) : ICacheProvider
{
    public async Task<CacheResult<T>> GetAsync<T>(
        string key,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = keyFormatter.Format(key);
        var value = await cacheStrategy.GetAsync(cacheKey, cancellationToken);

        return value is null
            ? CacheResult<T>.Miss
            : CacheResult<T>.Hit(serializer.Deserialize<T>(value));
    }

    public async Task<T> GetOrSetAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        CacheEntryOptions? entryOptions = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(factory);

        var cachedValue = await GetAsync<T>(key, cancellationToken);

        if (cachedValue.Found)
        {
            return cachedValue.Value!;
        }

        var value = await factory(cancellationToken);
        await SetAsync(key, value, entryOptions, cancellationToken);

        return value;
    }

    public Task SetAsync<T>(
        string key,
        T value,
        CacheEntryOptions? entryOptions = null,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = keyFormatter.Format(key);
        var serializedValue = serializer.Serialize(value);
        var expiration = GetExpiration(entryOptions);

        return cacheStrategy.SetAsync(cacheKey, serializedValue, expiration, cancellationToken);
    }

    public Task RemoveAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = keyFormatter.Format(key);
        return cacheStrategy.RemoveAsync(cacheKey, cancellationToken);
    }

    private TimeSpan? GetExpiration(CacheEntryOptions? entryOptions)
    {
        if (entryOptions?.NeverExpire == true)
        {
            return null;
        }

        var expiration = entryOptions?.AbsoluteExpirationRelativeToNow
            ?? options.Value.DefaultExpiration;

        return expiration > TimeSpan.Zero
            ? expiration
            : null;
    }
}
