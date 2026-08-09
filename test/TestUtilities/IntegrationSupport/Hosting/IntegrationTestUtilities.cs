using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Persistence;

namespace ResumeEnhancer.TestUtilities.IntegrationSupport;

public sealed class IntegrationTestUtilities<TProgram> : IDisposable
    where TProgram : class
{
    private readonly WebApplicationFactory<TProgram> _factory;
    private readonly TestAuthenticationState _authenticationState;
    private readonly SqliteConnection? _sqliteConnection;

    internal IntegrationTestUtilities(
        WebApplicationFactory<TProgram> factory,
        TestAuthenticationState authenticationState,
        SqliteConnection? sqliteConnection)
    {
        _factory = factory;
        _authenticationState = authenticationState;
        _sqliteConnection = sqliteConnection;
    }

    public IServiceProvider Services => _factory.Services;

    internal TestAuthenticationState AuthenticationState => _authenticationState;

    public HttpClient CreateClient()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });

        if (_authenticationState.AuditUserId is { } auditUserId)
        {
            client.DefaultRequestHeaders.Remove("X-Audit-UserId");
            client.DefaultRequestHeaders.Add("X-Audit-UserId", auditUserId.ToString());
        }

        if (!string.IsNullOrWhiteSpace(_authenticationState.UserId))
        {
            client.DefaultRequestHeaders.Remove("X-User-Id");
            client.DefaultRequestHeaders.Add("X-User-Id", _authenticationState.UserId);
        }

        return client;
    }

    public async Task<HttpResponseMessage> PostJsonAsync<TRequest>(
        string requestUri,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        using var client = CreateClient();

        return await client.PostAsJsonAsync(requestUri, request, cancellationToken);
    }

    public async Task<HttpResponseMessage> PutJsonAsync<TRequest>(
        string requestUri,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        using var client = CreateClient();

        return await client.PutAsJsonAsync(requestUri, request, cancellationToken);
    }

    public async Task<HttpResponseMessage> DeleteAsync(
        string requestUri,
        CancellationToken cancellationToken = default)
    {
        using var client = CreateClient();

        return await client.DeleteAsync(requestUri, cancellationToken);
    }

    public ISetupper CreateSetupper() => new IntegrationTestSetupper<TProgram>(this);

    public void ResetDatabase()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
        dbContext.ChangeTracker.Clear();
    }

    public void Dispose()
    {
        _factory.Dispose();
        _sqliteConnection?.Dispose();
    }
}
