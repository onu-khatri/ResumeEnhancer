---
title: Shared ResumeEnhancer.Infrastructure.Caching Project Knowledge
intent: help an AI agent safely implement, review, and plan changes across the shared caching project, its host wiring, and its test-support seams
scope: in scope is `application/Infrastructure/Caching` plus first-class host wiring in `application/WebSolution/WebSolution.Server` and test-support consumers under `test/TestUtilities`; out of scope are business-specific cache usage details except where needed to explain composition or boundaries
audience: AI agent implementer, reviewer, and planner
last_reviewed: 2026-08-15
---

## Intent

This artifact gives an AI agent a first-pass mental model and a safe extension path for the shared caching package in `application/Infrastructure/Caching`, including how the web host composes it and how tests replace it. `Observed`: the package exposes one application-facing cache contract, hides concrete providers behind an internal strategy seam, and is registered once in the web host startup.

```csharp
public interface ICacheProvider
{
    Task<CacheResult<T>> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task<T> GetOrSetAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        CacheEntryOptions? options = null,
        CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, CacheEntryOptions? options = null, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}
```

Reference: `ICacheProvider` in [ICacheProvider](../application/Infrastructure/Caching/Abstractions/ICacheProvider.cs).

## When to use this knowledge

Use this artifact when an AI agent needs to:

1. modify shared caching behavior without leaking provider-specific code into business services
2. review a change touching `ApplicationCacheProvider`, provider strategies, cache key formatting, serialization, or expiration behavior
3. add a new provider or alter how existing providers are selected and configured
4. understand how the host wires caching and how tests mock or replace the cache seam

Do not use this as the primary reference for business-specific invalidation rules inside modules or handlers. `Observed`: repository guidance keeps shared infrastructure in `application/Infrastructure`, and the caching README says application code should depend on `ICacheProvider`, not on concrete cache technology APIs.

## Core concepts

### Public application contract

`Observed`: application code is expected to depend on `ICacheProvider`, `CacheEntryOptions`, and `CacheResult<T>`, not on Redis, `IMemoryCache`, or MemCache APIs.

```csharp
public readonly record struct CacheResult<T>(bool Found, T? Value)
{
    public static CacheResult<T> Miss => new(false, default);
    public static CacheResult<T> Hit(T? value) => new(true, value);
}
```

```csharp
public sealed class CacheEntryOptions
{
    public static CacheEntryOptions NoExpiration { get; } = new() { NeverExpire = true };

    public TimeSpan? AbsoluteExpirationRelativeToNow { get; init; }
    public bool NeverExpire { get; init; }
}
```

These two types matter because cache misses are represented explicitly through `Found`, and "never expire" is a first-class option rather than a magic sentinel value. References: [CacheResult](../application/Infrastructure/Caching/Abstractions/CacheResult.cs), [CacheEntryOptions](../application/Infrastructure/Caching/Abstractions/CacheEntryOptions.cs).

### Provider-neutral orchestration layer

`Observed`: `ApplicationCacheProvider` is the only concrete `ICacheProvider` implementation, and it composes three infrastructure services:

1. `ICacheStrategy` for raw byte storage
2. `ICacheSerializer` for object-to-byte conversion
3. `ICacheKeyFormatter` for stable provider keys

```csharp
internal sealed class ApplicationCacheProvider(
    ICacheStrategy cacheStrategy,
    ICacheSerializer serializer,
    ICacheKeyFormatter keyFormatter,
    IOptions<CacheOptions> options) : ICacheProvider
```

This constructor is the architecture in one line: the public provider is provider-neutral because storage, serialization, and key formatting are all replaceable internal seams. Reference: [ApplicationCacheProvider](../application/Infrastructure/Caching/Providers/ApplicationCacheProvider.cs).

### Configuration surface

`Observed`: provider selection and defaults come from `CacheOptions`, which binds to the `"Caching"` configuration section.

```csharp
public sealed class CacheOptions
{
    public const string SectionName = "Caching";

    public CacheProviderType Provider { get; set; } = CacheProviderType.InMemory;
    public string KeyPrefix { get; set; } = "ResumeEnhancer";
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(30);
    public RedisCacheOptions Redis { get; set; } = new();
    public MemCacheOptions MemCache { get; set; } = new();
}
```

```csharp
public enum CacheProviderType
{
    InMemory,
    Redis,
    MemCache
}
```

References: [CacheOptions](../application/Infrastructure/Caching/Options/CacheOptions.cs), [CacheProviderType](../application/Infrastructure/Caching/Options/CacheProviderType.cs).

## Architectural placement

`Observed`: the shared caching package is infrastructure-level code consumed by the host at startup.

```csharp
builder.Services.AddApplicationCaching(builder.Configuration);
builder.Services.AddAppDbContext((_, options) =>
{
    options.UseSqlServer(GetConnectionString(builder));
});
builder.Services.AddApplicationModules();
```

Reference: [Program](../application/WebSolution/WebSolution.Server/Program.cs).

`Observed`: repo-level guidance says shared infrastructure behavior belongs in `application/Infrastructure`, and the caching README says application code should depend on `ICacheProvider`, not on Redis, MemCache, or in-memory cache APIs directly.

`Inferred`: the layering rule for future changes is:

1. host chooses and configures the provider
2. application code consumes only `ICacheProvider`
3. the caching package owns technology-specific details behind internal strategies

That inference combines the host startup registration, the public abstractions, and the README boundary statement in [README](../application/Infrastructure/Caching/README.md).

## Main workflows

### 1. Startup registration and provider selection

`Observed`: `AddApplicationCaching` binds the configuration section, registers the serializer, key formatter, and public provider, then chooses one internal strategy from the configured provider enum.

```csharp
public static IServiceCollection AddApplicationCaching(
    this IServiceCollection services,
    IConfiguration configuration)
{
    var section = configuration.GetSection(CacheOptions.SectionName);
    var cacheOptions = section.Get<CacheOptions>() ?? new CacheOptions();

    services.Configure<CacheOptions>(section);
    services.TryAddSingleton<ICacheSerializer, SystemTextJsonCacheSerializer>();
    services.TryAddSingleton<ICacheKeyFormatter, Sha256CacheKeyFormatter>();
    services.TryAddSingleton<ICacheProvider, ApplicationCacheProvider>();

    AddProviderStrategy(services, cacheOptions);

    return services;
}
```

```csharp
switch (options.Provider)
{
    case CacheProviderType.InMemory:
        services.AddMemoryCache();
        services.TryAddSingleton<ICacheStrategy, InMemoryCacheStrategy>();
        break;
    case CacheProviderType.Redis:
        services.AddStackExchangeRedisCache(redisOptions =>
        {
            redisOptions.Configuration = options.Redis.Configuration;
            redisOptions.InstanceName = options.Redis.InstanceName;
        });
        services.TryAddSingleton<ICacheStrategy, DistributedCacheStrategy>();
        break;
    case CacheProviderType.MemCache:
        services.TryAddSingleton<ICacheStrategy, MemCacheStrategy>();
        break;
    default:
        throw new InvalidOperationException($"Unsupported cache provider '{options.Provider}'.");
}
```

The important invariant is that exactly one `ICacheStrategy` is registered through this switch. Reference: [DependencyInjection](../application/Infrastructure/Caching/DependencyInjection/DependencyInjection.cs).

### 2. Read-through cache miss workflow

`Observed`: `GetOrSetAsync` checks the cache first, returns the cached value on a hit, otherwise executes the factory, stores the produced value, and returns it.

```csharp
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
```

This is the central behavior another agent will usually change or rely on. Reference: [ApplicationCacheProvider](../application/Infrastructure/Caching/Providers/ApplicationCacheProvider.cs).

### 3. Cache read and write normalization workflow

`Observed`: every public operation formats the logical key before delegating to the strategy, and writes always serialize object values to bytes.

```csharp
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
```

```csharp
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
```

The compatibility implication is simple: changing key formatting or serialization affects every provider. Reference: [ApplicationCacheProvider](../application/Infrastructure/Caching/Providers/ApplicationCacheProvider.cs).

## Rules and invariants

1. `Observed`: business code should inject `ICacheProvider`, not `IMemoryCache`, `IDistributedCache`, Redis clients, or MemCache clients. This rule is stated directly in the caching README.
2. `Observed`: a blank or whitespace cache key is invalid and throws before reaching any provider.
3. `Observed`: `CacheEntryOptions.NoExpiration` and non-positive default expiration both normalize to `null` expiration at the strategy layer.
4. `Observed`: `MemCacheStrategy` requires at least one valid server configuration and throws if none exist.
5. `Observed`: unsupported provider enum values fail fast during service registration.

```csharp
if (string.IsNullOrWhiteSpace(key))
{
    throw new ArgumentException("Cache key cannot be empty.", nameof(key));
}
```

```csharp
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
```

```csharp
if (servers.Length == 0)
{
    throw new InvalidOperationException("At least one MemCache server must be configured.");
}
```

References: [Sha256CacheKeyFormatter](../application/Infrastructure/Caching/Keying/Sha256CacheKeyFormatter.cs), [ApplicationCacheProvider](../application/Infrastructure/Caching/Providers/ApplicationCacheProvider.cs), [MemCacheStrategy](../application/Infrastructure/Caching/Strategies/MemCacheStrategy.cs).

## Key formatting, serialization, and expiration logic

### Key formatting

`Observed`: logical cache keys are never used directly in backing stores. They are SHA-256 hashed, lowercased, and optionally prefixed with a sanitized key prefix.

```csharp
var prefix = SanitizePrefix(options.Value.KeyPrefix);
var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(key))).ToLowerInvariant();

return string.IsNullOrWhiteSpace(prefix)
    ? hash
    : $"{prefix}:{hash}";
```

```csharp
private static bool IsSafeKeyCharacter(char character) =>
    char.IsAsciiLetterOrDigit(character)
    || character is ':' or '_' or '-' or '.';
```

`Observed`: unsafe characters in the prefix are replaced with `_`, and blank prefixes collapse to hash-only keys. This behavior is validated in [Sha256CacheKeyFormatterTests](../test/ResumeEnhancer.Tests/Infrastructure/Caching/Sha256CacheKeyFormatterTests.cs).

### Serialization

`Observed`: all object values are serialized through `System.Text.Json` with `JsonSerializerDefaults.Web`.

```csharp
internal sealed class SystemTextJsonCacheSerializer : ICacheSerializer
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public byte[] Serialize<T>(T value) =>
        JsonSerializer.SerializeToUtf8Bytes(value, SerializerOptions);

    public T? Deserialize<T>(byte[] value) =>
        JsonSerializer.Deserialize<T>(value, SerializerOptions);
}
```

The compatibility risk is high: serializer changes can invalidate existing cache payloads or change deserialization semantics across all providers. Reference: [SystemTextJsonCacheSerializer](../application/Infrastructure/Caching/Serialization/SystemTextJsonCacheSerializer.cs).

### Expiration

`Observed`: expiration is resolved in `ApplicationCacheProvider` before reaching strategies. `null` means "no expiration" for every strategy.

`Observed`: `MemCacheStrategy` converts `TimeSpan?` into MemCache protocol seconds, with special handling for null, zero-or-negative values, and values longer than 30 days.

```csharp
private static long GetExpirationSeconds(TimeSpan? absoluteExpirationRelativeToNow)
{
    if (!absoluteExpirationRelativeToNow.HasValue)
    {
        return 0;
    }

    if (absoluteExpirationRelativeToNow.Value <= TimeSpan.Zero)
    {
        return 1;
    }

    var seconds = (long)Math.Ceiling(absoluteExpirationRelativeToNow.Value.TotalSeconds);

    return seconds <= 2_592_000
        ? seconds
        : DateTimeOffset.UtcNow.Add(absoluteExpirationRelativeToNow.Value).ToUnixTimeSeconds();
}
```

This is the most provider-specific expiration rule in the package. Reference: [MemCacheStrategy](../application/Infrastructure/Caching/Strategies/MemCacheStrategy.cs).

## Provider strategy internals

### InMemory strategy

`Observed`: `InMemoryCacheStrategy` stores defensive byte-array copies on write and returns defensive copies on read so callers cannot mutate shared in-process state accidentally.

```csharp
var value = memoryCache.TryGetValue<byte[]>(key, out var cachedBytes)
    ? cachedBytes?.ToArray()
    : null;
```

```csharp
memoryCache.Set(key, value.ToArray(), options);
```

That copy behavior is a contract worth preserving because tests assert it directly in [InMemoryCacheStrategyTests](../test/ResumeEnhancer.Tests/Infrastructure/Caching/InMemoryCacheStrategyTests.cs). Reference: [InMemoryCacheStrategy](../application/Infrastructure/Caching/Strategies/InMemoryCacheStrategy.cs).

### Distributed strategy

`Observed`: `DistributedCacheStrategy` is intentionally thin. It delegates directly to `IDistributedCache` and only adds translation from `TimeSpan?` into `DistributedCacheEntryOptions`.

```csharp
public Task<byte[]?> GetAsync(
    string key,
    CancellationToken cancellationToken = default) =>
    distributedCache.GetAsync(key, cancellationToken);
```

```csharp
var options = new DistributedCacheEntryOptions();

if (absoluteExpirationRelativeToNow.HasValue)
{
    options.AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
}

return distributedCache.SetAsync(key, value, options, cancellationToken);
```

`Inferred`: if a future change adds provider-specific logic for Redis, this strategy is where it belongs rather than in `ApplicationCacheProvider`, because the provider façade currently remains provider-neutral. Reference: [DistributedCacheStrategy](../application/Infrastructure/Caching/Strategies/DistributedCacheStrategy.cs).

### MemCache strategy

`Observed`: `MemCacheStrategy` talks directly to MemCache over TCP using the text protocol and performs its own request/response parsing.

```csharp
await WriteAsciiAsync(stream, $"get {key}\r\n", cancellationToken);
var firstLine = await ReadLineAsync(stream, cancellationToken);

if (firstLine == "END")
{
    return null;
}
```

```csharp
await WriteAsciiAsync(stream, $"set {key} 0 {expiration} {value.Length}\r\n", cancellationToken);
await stream.WriteAsync(value, cancellationToken);
await stream.WriteAsync(CrLf, cancellationToken);
```

`Observed`: server selection uses a stable FNV-1a-style hash of the formatted key to keep a key on the same configured server.

```csharp
var index = (int)(GetStableHash(key) % (uint)servers.Length);
return servers[index];
```

`Observed`: line parsing is defensive and throws on malformed responses, missing `END`, oversized lines, or missing CRLF terminators. Those behaviors are validated in [MemCacheStrategyTests](../test/ResumeEnhancer.Tests/Infrastructure/Caching/MemCacheStrategyTests.cs). Reference: [MemCacheStrategy](../application/Infrastructure/Caching/Strategies/MemCacheStrategy.cs).

## Host wiring and first-class consumers

### Web host composition

`Observed`: the web host composes caching once, before module registration.

```csharp
builder.Services.AddApplicationCaching(builder.Configuration);
builder.Services.AddAppDbContext((_, options) =>
{
    options.UseSqlServer(GetConnectionString(builder));
});
builder.Services.AddApplicationModules();
```

The ordering matters less than the fact that caching is registered centrally at host startup and exposed through normal DI. Reference: [Program](../application/WebSolution/WebSolution.Server/Program.cs).

### Host configuration defaults

`Observed`: the default host configuration uses `InMemory` with a `ResumeEnhancer` key prefix and a 30-minute default expiration, while still including Redis and MemCache option shapes in `appsettings.json`.

```json
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
```

Reference: [appsettings.json](../application/WebSolution/WebSolution.Server/appsettings.json).

### Test-support consumer seam

`Observed`: integration-test utilities expose a first-class seam for replacing the shared `ICacheProvider` with a strict mock.

```csharp
public IntegrationTestUtilitiesBuilder<TProgram> WithMockedCacheProvider()
{
    return WithConfigureServices(services =>
    {
        var cacheProvider = new Mock<ICacheProvider>(MockBehavior.Strict);

        services.Replace(ServiceDescriptor.Singleton(cacheProvider.Object));
    });
}
```

This is the main reason the public cache contract must remain stable and DI-friendly. Reference: [IntegrationTestUtilitiesBuilder](../test/TestUtilities/IntegrationSupport/Hosting/IntegrationTestUtilitiesBuilder.cs).

## Extension pattern

### Add a new cache provider

1. `Observed`: add a new enum value to `CacheProviderType`.
2. `Observed`: extend `CacheOptions` with provider-specific options if needed.
3. `Observed`: implement `ICacheStrategy` for the new backing technology.
4. `Observed`: register the strategy in the `AddProviderStrategy` switch inside `DependencyInjection`.
5. `Observed`: update `README.md` configuration guidance.
6. `Observed`: add or extend strategy tests plus DI registration tests.

```csharp
internal interface ICacheStrategy
{
    string ProviderName { get; }
    Task<byte[]?> GetAsync(string key, CancellationToken cancellationToken = default);
    Task SetAsync(string key, byte[] value, TimeSpan? absoluteExpirationRelativeToNow, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}
```

The copy shape already exists in the README’s "Adding A New Cache Provider" section in [README](../application/Infrastructure/Caching/README.md).

### Change key formatting

1. `Observed`: update `Sha256CacheKeyFormatter` or replace the registered `ICacheKeyFormatter` implementation.
2. `Observed`: keep blank-key validation unless there is a deliberate contract change.
3. `Observed`: update formatter tests to cover prefix sanitization, determinism, and collision-relevant expectations.
4. `Inferred`: treat this as a compatibility-sensitive change because every stored key across every provider will change shape at once.

```csharp
services.TryAddSingleton<ICacheKeyFormatter, Sha256CacheKeyFormatter>();
```

The failure mode is global cache misses after deployment because new formatted keys will not match old stored keys.

### Change serialization

1. `Observed`: update `SystemTextJsonCacheSerializer` or replace the registered `ICacheSerializer`.
2. `Observed`: keep the public `ICacheProvider` surface unchanged unless the caller contract itself must change.
3. `Observed`: update serializer tests and provider tests that exercise round-tripping.
4. `Inferred`: plan for mixed old/new payload compatibility if the cache is shared across rolling deployments.

```csharp
services.TryAddSingleton<ICacheSerializer, SystemTextJsonCacheSerializer>();
```

The failure mode is deserialization breakage or subtle semantic drift for existing cached payloads.

### Change expiration behavior

1. `Observed`: decide whether the change belongs in provider-neutral expiration resolution in `ApplicationCacheProvider` or in a provider-specific translation layer such as `MemCacheStrategy`.
2. `Observed`: update tests for default expiration, no-expiration handling, and provider-specific conversion edge cases.
3. `Inferred`: keep `null` as the strategy-layer meaning for "never expire" unless all strategies and tests are updated coherently.

```csharp
await provider.SetAsync(
    "key",
    new CachedItem("value"),
    CacheEntryOptions.NoExpiration,
    cancellationToken);

cacheStrategy.SetCalls[0].Expiration.ShouldBeNull();
```

Reference example: [ApplicationCacheProviderTests](../test/ResumeEnhancer.Tests/Infrastructure/Caching/ApplicationCacheProviderTests.cs).

## Verification and testing

The main validation command for this area is:

```powershell
dotnet test test\ResumeEnhancer.Tests\ResumeEnhancer.Tests.Unit.csproj --no-restore --filter "FullyQualifiedName~Infrastructure.Caching"
```

`Recommended`: use that filter first for focused changes, then widen to the full unit suite when cross-cutting infrastructure was touched.

Important test coverage already in the repo:

```csharp
provider.GetRequiredService<ICacheProvider>().ShouldBeOfType<ApplicationCacheProvider>();
provider.GetRequiredService<ICacheStrategy>().ShouldBeOfType<InMemoryCacheStrategy>();
provider.GetRequiredService<ICacheSerializer>().ShouldBeOfType<SystemTextJsonCacheSerializer>();
provider.GetRequiredService<ICacheKeyFormatter>().ShouldBeOfType<Sha256CacheKeyFormatter>();
```

This proves DI composition defaults. Reference: [CachingDependencyInjectionTests](../test/ResumeEnhancer.Tests/Infrastructure/Caching/CachingDependencyInjectionTests.cs).

```csharp
await provider.SetAsync("key", new CachedItem("hit"), cancellationToken: cancellationToken);
var result = await provider.GetAsync<CachedItem>("key", cancellationToken);

result.Found.ShouldBeTrue();
result.Value.ShouldBe(new CachedItem("hit"));
```

This proves end-to-end provider serialization and read behavior through the shared façade. Reference: [ApplicationCacheProviderTests](../test/ResumeEnhancer.Tests/Infrastructure/Caching/ApplicationCacheProviderTests.cs).

```csharp
await strategy.SetAsync("key", bytes, TimeSpan.FromMinutes(5), cancellationToken);
bytes[0] = 9;
var cached = await strategy.GetAsync("key", cancellationToken);
cached.ShouldBe([1, 2, 3]);
```

This proves defensive-copy behavior in the in-memory strategy. Reference: [InMemoryCacheStrategyTests](../test/ResumeEnhancer.Tests/Infrastructure/Caching/InMemoryCacheStrategyTests.cs).

What was verified this session:

- current source files and test files were re-read
- command guidance was checked against repo paths

What was not run this session:

- no `dotnet test` command was executed during this documentation step

## Pitfalls and boundaries

1. Do not inject concrete cache technology APIs into business services. Use `ICacheProvider`.
2. Do not change `ICacheKeyFormatter` or `ICacheSerializer` casually; both are package-wide compatibility seams.
3. Do not put provider-specific branching into business code or into the public `ICacheProvider` surface.
4. Do not treat MemCache protocol parsing as trivial string handling; the current implementation has explicit guards for malformed responses and protocol framing.
5. Do not break the DI replacement seam used by integration tests.

```csharp
if (response is not "DELETED" and not "NOT_FOUND")
{
    throw new InvalidOperationException($"Unable to delete MemCache value. Response: '{response}'.");
}
```

```csharp
services.Replace(ServiceDescriptor.Singleton(cacheProvider.Object));
```

The first snippet shows provider-specific boundary enforcement inside the MemCache strategy. The second shows why the shared public abstraction must remain easy to replace in tests. References: [MemCacheStrategy](../application/Infrastructure/Caching/Strategies/MemCacheStrategy.cs), [IntegrationTestUtilitiesBuilder](../test/TestUtilities/IntegrationSupport/Hosting/IntegrationTestUtilitiesBuilder.cs).

## Clarifications

- Q: Should the knowledge base cover only `application/Infrastructure/Caching`, or also include host wiring and test-support consumers as first-class material?
  - A: also its host wiring and test-support consumers as first-class material
- Q: For AI agents, should the artifact optimize for implementation, review, planning, or all three?
  - A: all three but by AI Agent
- Q: Should extension recipes cover only a new provider or also key formatting, serialization, and expiration behavior changes?
  - A: all options
- Q: Should evidence be inline snippet-first only, or should there be a dedicated evidence map?
  - A: inline snippet-first only
- Q: Should references be a dedicated section or embedded inside each section?
  - A: keep references embedded inside each section
- Q: Should the earlier plan be kept and revised or discarded and recreated cleanly?
  - A: discard it and recreate cleanly from your answers

