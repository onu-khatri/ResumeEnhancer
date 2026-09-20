using System.Collections.Concurrent;
using System.Security.Cryptography;
using ResumeEnhancer.AuthModule.DM.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Time.Testing;
using ResumeEnhancer.AuthModule.PL;
using ResumeEnhancer.AuthModule.SL.Abstractions;
using ResumeEnhancer.AuthModule.SL.Options;
using ResumeEnhancer.AuthModule.SL.Services;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.TestUtilities.IntegrationSupport;

namespace ResumeEnhancer.Tests.Integration.TestSupport;

[CollectionDefinition("Sequential_Integration", DisableParallelization = true)]
public sealed class IntegrationTestCollection : ICollectionFixture<IntegrationTestAssemblyFixture>
{
}

public sealed class IntegrationTestAssemblyFixture : IDisposable
{
    internal static readonly DateTimeOffset InitialUtcNow = new(2030, 1, 1, 0, 0, 0, TimeSpan.Zero);

    private readonly string keyRingPath = Path.Combine(Path.GetTempPath(), $"ResumeEnhancerAuthKeys-{Guid.NewGuid():N}");
    private readonly string? priorDataProtectionKeyRingPath;
    private readonly ResettableFakeTimeProvider timeProvider = new(InitialUtcNow);

    public IntegrationTestAssemblyFixture()
    {
        priorDataProtectionKeyRingPath = Environment.GetEnvironmentVariable("Auth__Security__DataProtectionKeyRingPath");
        Directory.CreateDirectory(keyRingPath);
        Environment.SetEnvironmentVariable("Auth__Security__DataProtectionKeyRingPath", keyRingPath);
        Utilities = IntegrationTestUtilitiesBuilder
            .Get<global::Program>()
            .WithInMemoryDbContext()
            .WithFakeAuthentication()
            .WithMockedCacheProvider()
            .WithConfigureAppConfiguration(configuration =>
                configuration.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["Auth:Security:DataProtectionKeyRingPath"] = keyRingPath,
                        ["Auth:Security:TrustedOrigins:0"] = "https://localhost",
                    }))
            .WithConfigureServices(services =>
            {
                services.Replace(ServiceDescriptor.Singleton<TimeProvider>(timeProvider));
                services.Replace(ServiceDescriptor.Singleton<IRegistrationThrottle, IntegrationTestRegistrationThrottle>());
                services.RemoveAll<AuthSecurityOptions>();
                services.AddSingleton(new AuthSecurityOptions
                {
                    DataProtectionKeyRingPath = keyRingPath,
                    TrustedOrigins = ["https://localhost"],
                });
            })
            .Build();

        RealUtilities = IntegrationTestUtilitiesBuilder
            .Get<global::Program>()
            .WithInMemoryDbContext()
            .WithRealAuthentication()
            .WithMockedCacheProvider()
            .WithConfigureAppConfiguration(configuration =>
                configuration.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["Auth:Security:DataProtectionKeyRingPath"] = keyRingPath,
                        ["Auth:Security:TrustedOrigins:0"] = "https://localhost",
                    }))
            .WithConfigureServices(services =>
            {
                services.Replace(ServiceDescriptor.Singleton<IRegistrationThrottle, IntegrationTestRegistrationThrottle>());
                services.RemoveAll<AuthSecurityOptions>();
                services.AddSingleton(new AuthSecurityOptions
                {
                    DataProtectionKeyRingPath = keyRingPath,
                    TrustedOrigins = ["https://localhost"],
                });
            })
            .Build();
    }

    internal IntegrationTestUtilities<global::Program> Utilities { get; }

    internal IntegrationTestUtilities<global::Program> RealUtilities { get; }

    internal ResettableFakeTimeProvider TimeProvider => timeProvider;

    internal ISetupper CreateSetupper() => Utilities.CreateSetupper();

    internal async Task ResetAndSeedAsync(CancellationToken cancellationToken)
    {
        timeProvider.SetUtcNow(InitialUtcNow);
        ((IntegrationTestRegistrationThrottle)Utilities.Services.GetRequiredService<IRegistrationThrottle>()).Reset();
        Utilities.ClearAuthentication();
        ((IntegrationTestRegistrationThrottle)RealUtilities.Services.GetRequiredService<IRegistrationThrottle>()).Reset();
        await ResetAndSeedUtilityAsync(Utilities, cancellationToken);
        await ResetAndSeedUtilityAsync(RealUtilities, cancellationToken);
    }

    private async Task ResetAndSeedUtilityAsync(
        IntegrationTestUtilities<global::Program> utilities,
        CancellationToken cancellationToken)
    {
        utilities.ResetDatabase();
        await utilities.Services.SeedAppDbContextAsync(cancellationToken);
        var dbContext = utilities.Services.GetRequiredService<AppDbContext>();
        if (!dbContext.Set<AuthSigningKeyMetadata>().Any())
        {
            using var rsa = RSA.Create(2048);
            dbContext.Add(new AuthSigningKeyMetadata
            {
                KeyIdentifier = "primary",
                ProtectedMaterial = utilities.Services.GetRequiredService<IAuthProtectedKeyMaterialStore>().Protect(rsa),
                IsActive = true,
                LifecycleVersion = 1,
                ActivatedAtUtc = timeProvider.GetUtcNow().UtcDateTime,
            });
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        await utilities.Services.GetRequiredService<AuthSigningKeyStartupValidator>()
            .ValidateAsync(cancellationToken);
    }

    public void Dispose()
    {
        Utilities.Dispose();
        RealUtilities.Dispose();
        Environment.SetEnvironmentVariable("Auth__Security__DataProtectionKeyRingPath", priorDataProtectionKeyRingPath);
        try { Directory.Delete(keyRingPath, recursive: true); } catch { }
    }

    internal sealed class ResettableFakeTimeProvider(DateTimeOffset initialUtcNow) : TimeProvider
    {
        private FakeTimeProvider current = new(initialUtcNow);

        public override DateTimeOffset GetUtcNow() => current.GetUtcNow();

        public override long GetTimestamp() => current.GetTimestamp();

        public override TimeZoneInfo LocalTimeZone => current.LocalTimeZone;

        public override ITimer CreateTimer(
            TimerCallback callback,
            object? state,
            TimeSpan dueTime,
            TimeSpan period) => current.CreateTimer(callback, state, dueTime, period);

        internal void SetUtcNow(DateTimeOffset utcNow) => current = new FakeTimeProvider(utcNow);

        internal void Advance(TimeSpan amount) => current.Advance(amount);
    }
}

internal sealed class IntegrationTestRegistrationThrottle(TimeProvider timeProvider) : IRegistrationThrottle
{
    private static readonly IReadOnlyDictionary<LimiterOperation, (int Limit, TimeSpan Window)> Policies =
        new Dictionary<LimiterOperation, (int, TimeSpan)>
        {
            [LimiterOperation.Login] = (5, TimeSpan.FromMinutes(15)),
            [LimiterOperation.Registration] = (5, TimeSpan.FromMinutes(15)),
            [LimiterOperation.Refresh] = (5, TimeSpan.FromMinutes(15)),
            [LimiterOperation.Verification] = (5, TimeSpan.FromHours(1)),
            [LimiterOperation.Recovery] = (5, TimeSpan.FromHours(1)),
            [LimiterOperation.PasswordChange] = (5, TimeSpan.FromHours(1)),
            [LimiterOperation.Logout] = (5, TimeSpan.FromMinutes(15)),
        };

    private readonly ConcurrentDictionary<string, ConcurrentQueue<DateTime>> attempts = new(StringComparer.Ordinal);

    public async Task<bool> IsAllowedAsync(
        string normalizedEmail,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        return (await TryConsumeAsync(
            LimiterOperation.Registration,
            normalizedEmail,
            null,
            ipAddress,
            cancellationToken)).Allowed;
    }

    public Task<LimiterDecision> TryConsumeAsync(
        LimiterOperation operation,
        string? normalizedEmail,
        string? subject,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var policy = Policies[operation];
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var key = string.Join('|', operation, normalizedEmail ?? string.Empty, subject ?? string.Empty, ipAddress ?? "unknown");
        var queue = attempts.GetOrAdd(key, _ => new ConcurrentQueue<DateTime>());
        while (queue.TryPeek(out var timestamp) && now - timestamp > policy.Window)
            queue.TryDequeue(out _);

        if (queue.Count >= policy.Limit)
        {
            queue.TryPeek(out var first);
            return Task.FromResult(new LimiterDecision(false, queue.Count, policy.Limit, first.Add(policy.Window)));
        }

        queue.Enqueue(now);
        return Task.FromResult(new LimiterDecision(true, queue.Count, policy.Limit, now.Add(policy.Window)));
    }

    public void Reset() => attempts.Clear();
}
