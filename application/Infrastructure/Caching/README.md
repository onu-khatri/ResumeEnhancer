# ResumeEnhancer.Infrastructure.Caching Project

This project provides a provider-neutral cache interface for the application.

Application code should depend on `ICacheProvider`, not on Redis, MemCache, or in-memory cache APIs directly. The selected cache provider is chosen from configuration, so the application can move between providers without changing business code.

## What This Project Does

- Exposes one public cache interface: `ICacheProvider`.
- Uses a strategy pattern internally for cache providers.
- Supports in-memory cache.
- Supports Redis through `IDistributedCache`.
- Supports MemCache through the MemCache TCP text protocol.
- Serializes cached values with `System.Text.Json`.
- Hashes cache keys and applies a configurable prefix.

## Folder Structure

| Folder/File | Purpose |
| --- | --- |
| `Abstractions/` | Public types application code is allowed to use directly. |
| `Abstractions/ICacheProvider.cs` | Public interface used by application services. |
| `Abstractions/CacheEntryOptions.cs` | Per-entry expiration options. |
| `Abstractions/CacheResult.cs` | Result type returned by cache reads. |
| `Options/` | Configuration models for provider selection and provider settings. |
| `Options/CacheOptions.cs` | Configuration model for the `Caching` section. |
| `Options/CacheProviderType.cs` | Supported provider names: `InMemory`, `Redis`, and `MemCache`. |
| `Providers/` | Provider-neutral orchestration layer. |
| `Providers/ApplicationCacheProvider.cs` | Main implementation of `ICacheProvider`. |
| `Strategies/` | Internal provider implementations selected by configuration. |
| `Strategies/ICacheStrategy.cs` | Internal strategy contract for concrete cache providers. |
| `Strategies/InMemoryCacheStrategy.cs` | In-memory cache implementation. |
| `Strategies/DistributedCacheStrategy.cs` | Redis/distributed cache implementation. |
| `Strategies/MemCacheStrategy.cs` | MemCache implementation. |
| `Serialization/` | Serialization contract and JSON implementation. |
| `Keying/` | Cache key formatting and hashing. |
| `DependencyInjection/DependencyInjection.cs` | Registers the correct cache provider from configuration. |

Application code should usually only need the files in `Abstractions/`. Most other folders are infrastructure details.

## Architecture

Application services call:

```csharp
ICacheProvider
```

`ICacheProvider` is implemented by:

```csharp
ApplicationCacheProvider
```

`ApplicationCacheProvider` delegates storage to an internal strategy:

```text
InMemoryCacheStrategy
DistributedCacheStrategy
MemCacheStrategy
```

This is the strategy pattern. The business layer uses one interface, while infrastructure decides which provider strategy is active.

## Register Caching In The Application

Add the caching project reference to the application project:

```xml
<ProjectReference Include="..\..\Infrastructure\Caching\ResumeEnhancer.Infrastructure.Caching.csproj" />
```

Register caching during application startup:

```csharp
using ResumeEnhancer.Infrastructure.Caching;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationCaching(builder.Configuration);
```

After registration, inject `ICacheProvider` anywhere through dependency injection.

## Configuration

Add a `Caching` section in `appsettings.json`.

```json
{
  "Caching": {
    "Provider": "InMemory",
    "KeyPrefix": "ResumeEnhancer",
    "DefaultExpiration": "00:30:00",
    "Redis": {
      "Configuration": "localhost:6379",
      "InstanceName": "ResumeEnhancer:"
    },
    "MemCache": {
      "Servers": [
        {
          "Host": "localhost",
          "Port": 11211
        }
      ],
      "ConnectTimeout": "00:00:02",
      "ReceiveTimeout": "00:00:02"
    }
  }
}
```

## Provider Options

| Provider value | Description | Best for |
| --- | --- | --- |
| `InMemory` | Stores cache entries in the current application process. | Local development, single-instance apps. |
| `Redis` | Stores cache entries in Redis through `IDistributedCache`. | Production and multi-instance apps. |
| `MemCache` | Stores cache entries in MemCache. | Apps using MemCache infrastructure. |

## Switching Providers

Application code does not change when switching providers. Only configuration changes.

Use in-memory:

```json
"Caching": {
  "Provider": "InMemory"
}
```

Use Redis:

```json
"Caching": {
  "Provider": "Redis",
  "Redis": {
    "Configuration": "localhost:6379",
    "InstanceName": "ResumeEnhancer:"
  }
}
```

Use MemCache:

```json
"Caching": {
  "Provider": "MemCache",
  "MemCache": {
    "Servers": [
      {
        "Host": "localhost",
        "Port": 11211
      }
    ]
  }
}
```

## Basic Usage

Inject `ICacheProvider` into your service:

```csharp
using ResumeEnhancer.Infrastructure.Caching;

public sealed class ResumeQueryService(ICacheProvider cacheProvider)
{
    public async Task<ResumeDto> GetResumeAsync(
        int resumeId,
        CancellationToken cancellationToken = default)
    {
        return await cacheProvider.GetOrSetAsync(
            $"resume:{resumeId}",
            token => LoadResumeFromDatabaseAsync(resumeId, token),
            cancellationToken: cancellationToken);
    }

    private static Task<ResumeDto> LoadResumeFromDatabaseAsync(
        int resumeId,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
```

`GetOrSetAsync` means:

1. Try to read the value from cache.
2. If found, return the cached value.
3. If not found, run the factory method.
4. Save the factory result in cache.
5. Return the result.

## Reading From Cache

Use `GetAsync<T>` when you only want to check cache and do not want to load missing data automatically.

```csharp
var result = await cacheProvider.GetAsync<ResumeDto>(
    $"resume:{resumeId}",
    cancellationToken);

if (result.Found)
{
    return result.Value!;
}
```

`CacheResult<T>.Found` tells you whether the cache contained the key. This is better than checking `Value is null`, because a cached value could theoretically be null.

## Writing To Cache

Use `SetAsync<T>` to explicitly store a value:

```csharp
await cacheProvider.SetAsync(
    $"resume:{resume.Id}",
    resume,
    cancellationToken: cancellationToken);
```

Use a custom expiration:

```csharp
await cacheProvider.SetAsync(
    $"resume:{resume.Id}",
    resume,
    new CacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
    },
    cancellationToken);
```

Use no expiration:

```csharp
await cacheProvider.SetAsync(
    "resume-section-setup",
    sectionSetup,
    CacheEntryOptions.NoExpiration,
    cancellationToken);
```

## Removing From Cache

Remove cache entries when data changes:

```csharp
await cacheProvider.RemoveAsync(
    $"resume:{resumeId}",
    cancellationToken);
```

Example after updating a resume:

```csharp
public async Task UpdateResumeAsync(
    ResumeDto resume,
    CancellationToken cancellationToken = default)
{
    await SaveResumeToDatabaseAsync(resume, cancellationToken);

    await cacheProvider.RemoveAsync(
        $"resume:{resume.Id}",
        cancellationToken);
}
```

## Expiration Rules

If no `CacheEntryOptions` are provided, the project uses:

```json
"DefaultExpiration": "00:30:00"
```

That means cached values expire after 30 minutes by default.

To override the expiration for one cache entry, pass `CacheEntryOptions`.

To store without expiration, pass `CacheEntryOptions.NoExpiration`.

## Cache Key Guidelines

Use clear logical keys in application code:

```csharp
$"resume:{resumeId}"
$"user:{userId}:resume-list"
$"resume-section-setup"
```

Good cache keys should:

- Include the entity or use case name.
- Include the id or filter values that make the result unique.
- Include tenant, user, culture, or role when those values change the response.
- Stay stable between read and write operations.

Do not include secrets in cache keys. The formatter hashes keys internally, but keys should still be treated as application identifiers, not a place for passwords or tokens.

## What To Cache

Prefer caching DTOs or simple read models:

```csharp
public sealed record ResumeSummaryDto(
    int Id,
    string Title,
    DateTimeOffset UpdatedAt);
```

Cached values are serialized with `System.Text.Json`, so the type should be JSON serializable. Avoid caching EF Core tracked entity instances directly. Instead, map entities to DTOs and cache the DTO.

Good candidates for caching:

- Data read often but changed rarely.
- Lookup lists.
- User-specific dashboard summaries.
- Resume section setup or other reference data.

Poor candidates for caching:

- Data that changes every request.
- Large file or binary content.
- Sensitive data without a clear expiration and invalidation plan.
- EF Core `DbContext`, tracked entities, service objects, or open streams.

## Provider Details

### InMemory

In-memory cache stores data in the current application process.

Use it for local development or simple single-instance deployments. If the app restarts, the cache is lost. If the app runs on multiple servers, each server has its own separate cache.

### Redis

Redis uses `Microsoft.Extensions.Caching.StackExchangeRedis` and the .NET `IDistributedCache` abstraction.

Use it when multiple app instances should share the same cache.

Required configuration:

```json
"Caching": {
  "Provider": "Redis",
  "Redis": {
    "Configuration": "localhost:6379",
    "InstanceName": "ResumeEnhancer:"
  }
}
```

### MemCache

MemCache uses the configured server list.

Required configuration:

```json
"Caching": {
  "Provider": "MemCache",
  "MemCache": {
    "Servers": [
      {
        "Host": "localhost",
        "Port": 11211
      }
    ]
  }
}
```

When multiple MemCache servers are configured, the strategy chooses the server by a stable hash of the cache key. This keeps reads and writes for the same key on the same server.

## Adding A New Cache Provider

To add another provider:

1. Add a value to `CacheProviderType`.
2. Add provider-specific options to `CacheOptions` if needed.
3. Create a new class that implements `ICacheStrategy`.
4. Register it in `DependencyInjection.AddProviderStrategy`.
5. Add configuration examples to this README.

Example strategy shape:

```csharp
internal sealed class MyCacheStrategy : ICacheStrategy
{
    public string ProviderName => "MyCache";

    public Task<byte[]?> GetAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task SetAsync(
        string key,
        byte[] value,
        TimeSpan? absoluteExpirationRelativeToNow,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
```

## Common Mistakes

Do not inject `IMemoryCache`, `IDistributedCache`, Redis clients, or MemCache clients into business services. Inject `ICacheProvider`.

Do not use different keys for reading and writing the same data.

Do not cache data that changes frequently unless you remove or refresh the cache when the data changes.

Do not cache user-specific or permission-specific data with a shared key. Include the user id, tenant id, role, or permission context in the key.



