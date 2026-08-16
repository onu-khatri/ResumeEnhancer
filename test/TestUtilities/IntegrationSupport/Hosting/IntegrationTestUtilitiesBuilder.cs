using ResumeEnhancer.Infrastructure.Caching;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using ResumeEnhancer.Infrastructure.Persistence;
using System.Collections.Concurrent;

namespace ResumeEnhancer.TestUtilities.IntegrationSupport;

public static class IntegrationTestUtilitiesBuilder
{
    public static IntegrationTestUtilitiesBuilder<TProgram> Get<TProgram>()
        where TProgram : class =>
        new();
}

public sealed class IntegrationTestUtilitiesBuilder<TProgram>
    where TProgram : class
{
    private readonly List<Action<IServiceCollection>> _configureServices = [];
    private readonly TestAuthenticationState _authenticationState = new();
    private SqliteConnection? _sqliteConnection;

    public IntegrationTestUtilitiesBuilder<TProgram> WithInMemoryDbContext()
    {
        _sqliteConnection ??= CreateOpenConnection();

        _configureServices.Add(services =>
        {
            services.RemoveAll<AppDbContext>();
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.AddSingleton(_sqliteConnection);
            services.AddScoped<AppDbContext>(serviceProvider =>
            {
                var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlite(serviceProvider.GetRequiredService<SqliteConnection>())
                    .Options;

                return new IntegrationTestAppDbContext(
                    options,
                    serviceProvider.GetServices<IAppDbContextModelConfiguration>());
            });
        });

        return this;
    }

    public IntegrationTestUtilitiesBuilder<TProgram> WithFakeAuthentication()
    {
        _configureServices.Add(services =>
        {
            services.AddSingleton(_authenticationState);
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = FakeAuthenticationHandler.SchemeName;
                    options.DefaultChallengeScheme = FakeAuthenticationHandler.SchemeName;
                    options.DefaultScheme = FakeAuthenticationHandler.SchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions, FakeAuthenticationHandler>(
                    FakeAuthenticationHandler.SchemeName,
                    _ => { });
            services.AddAuthorization();
            services.TryAddEnumerable(
                ServiceDescriptor.Singleton<IStartupFilter, FakeAuthenticationStartupFilter>());
        });

        return this;
    }

    public IntegrationTestUtilitiesBuilder<TProgram> WithConfigureServices(
        Action<IServiceCollection> configureServices)
    {
        ArgumentNullException.ThrowIfNull(configureServices);

        _configureServices.Add(configureServices);

        return this;
    }

    public IntegrationTestUtilities<TProgram> Build()
    {
        var factory = new IntegrationTestWebApplicationFactory<TProgram>(_configureServices);
        var utilities = new IntegrationTestUtilities<TProgram>(
            factory,
            _authenticationState,
            _sqliteConnection);

        utilities.ResetDatabase();

        return utilities;
    }

    public IntegrationTestUtilitiesBuilder<TProgram> WithMockedCacheProvider()
    {
        return WithConfigureServices(services =>
        {
            services.Replace(ServiceDescriptor.Singleton<ICacheProvider>(
                new InMemoryIntegrationTestCacheProvider()));
        });
    }

    private static SqliteConnection CreateOpenConnection()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        return connection;
    }

    private sealed class IntegrationTestWebApplicationFactory<TEntryPoint>(
        IReadOnlyList<Action<IServiceCollection>> configureServices)
        : WebApplicationFactory<TEntryPoint>
        where TEntryPoint : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("IntegrationTest");
            builder.ConfigureLogging(logging => logging.ClearProviders());
            builder.ConfigureServices(services =>
            {
                foreach (var configureService in configureServices)
                {
                    configureService(services);
                }
            });
        }
    }

    private sealed class InMemoryIntegrationTestCacheProvider : ICacheProvider
    {
        private readonly ConcurrentDictionary<string, object?> _cache = new();

        public Task<CacheResult<T>> GetAsync<T>(
            string key,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(
                _cache.TryGetValue(key, out var value)
                    ? CacheResult<T>.Hit((T?)value)
                    : CacheResult<T>.Miss);
        }

        public async Task<T> GetOrSetAsync<T>(
            string key,
            Func<CancellationToken, Task<T>> factory,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_cache.TryGetValue(key, out var cachedValue))
            {
                return (T)cachedValue!;
            }

            var value = await factory(cancellationToken);
            _cache[key] = value;

            return value;
        }

        public Task SetAsync<T>(
            string key,
            T value,
            CacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _cache[key] = value;

            return Task.CompletedTask;
        }

        public Task RemoveAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _cache.TryRemove(key, out _);

            return Task.CompletedTask;
        }
    }
}

