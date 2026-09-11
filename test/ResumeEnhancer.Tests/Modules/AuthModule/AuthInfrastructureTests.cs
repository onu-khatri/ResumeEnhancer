using System.Security.Cryptography;
using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using ResumeEnhancer.AuthModule.AM.Requests;
using ResumeEnhancer.AuthModule.AM.Responses;
using ResumeEnhancer.AuthModule.DM.Entities;
using ResumeEnhancer.AuthModule.PL;
using ResumeEnhancer.AuthModule.PL.Repositories;
using ResumeEnhancer.AuthModule.PL.Seeding;
using ResumeEnhancer.AuthModule.SL.Abstractions;
using ResumeEnhancer.AuthModule.SL.Contracts;
using ResumeEnhancer.AuthModule.SL.Handlers;
using ResumeEnhancer.AuthModule.SL.Services;
using ResumeEnhancer.AuthModule.Web;
using ResumeEnhancer.AuthModule.Web.Outbox;
using ResumeEnhancer.BillingModule.SL.Integrations;
using ResumeEnhancer.ProfilingModule.SL.Integrations;
using ResumeEnhancer.Tests.Unit.TestInfrastructure;

namespace ResumeEnhancer.Tests.Unit.Modules.AuthModule;

public sealed class AuthInfrastructureTests
{
    [Fact]
    public void Database_schema_uses_default_and_rooted_names()
    {
        Assert.Equal("auth", AuthModuleDatabase.GetSchema(null));
        Assert.Equal("auth", AuthModuleDatabase.GetSchema(" "));
        Assert.Equal("tenant_auth", AuthModuleDatabase.GetSchema("tenant"));
    }

    [Fact]
    public void Password_hasher_round_trips_and_rejects_invalid_values()
    {
        var hasher = new AuthPasswordHasher();
        var encoded = hasher.Hash("Password!1234");

        Assert.True(hasher.Verify(encoded, "Password!1234"));
        Assert.False(hasher.Verify(encoded, "wrong"));
        Assert.False(hasher.Verify("not-a-hash", "Password!1234"));
        Assert.False(hasher.Verify("pbkdf2-sha512$x$bad$bad", "Password!1234"));
    }

    [Fact]
    public void Password_hasher_rejects_wrong_algorithm_and_malformed_payloads()
    {
        var hasher = new AuthPasswordHasher();

        Assert.False(hasher.Verify("bcrypt$210000$c2FsdA==$aGFzaA==", "Password!1234"));
        Assert.False(
            hasher.Verify("pbkdf2-sha512$not-a-number$c2FsdA==$aGFzaA==", "Password!1234")
        );
        Assert.False(hasher.Verify("pbkdf2-sha512$1$%%%$%%%", "Password!1234"));
    }

    [Fact]
    public void Token_service_hashes_refresh_tokens_and_issues_signed_access_tokens()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["Auth:SigningKey"] = "unit-test-signing-key-with-at-least-32-bytes",
                }
            )
            .Build();
        var service = new AuthTokenService(configuration);

        var token = service.CreateRefreshToken();
        var hash = service.HashRefreshToken(token);
        var issued = service.CreateAccessToken(42, Guid.NewGuid());

        Assert.NotEqual(token, hash);
        Assert.Equal(64, hash.Length);
        Assert.StartsWith("v1.", issued.AccessToken);
        Assert.InRange(
            issued.ExpiresAtUtc,
            DateTime.UtcNow.AddMinutes(14),
            DateTime.UtcNow.AddMinutes(16)
        );
    }

    [Fact]
    public async Task Registration_throttle_limits_email_and_ip_attempts()
    {
        var throttle = new InMemoryRegistrationThrottle();
        for (var attempt = 0; attempt < 5; attempt++)
            Assert.True(await throttle.IsAllowedAsync("ada@example.com", "127.0.0.1"));

        Assert.False(await throttle.IsAllowedAsync("ada@example.com", "127.0.0.1"));
        Assert.True(await throttle.IsAllowedAsync("other@example.com", "127.0.0.2"));
    }

    [Fact]
    public async Task Auth_repository_persists_and_transitions_sessions_and_outbox()
    {
        using var scope = new SqliteAppDbContextScope();
        var repository = new AuthRepository(scope.UnitOfWork);
        var family = Guid.NewGuid();
        var session = new RefreshSession
        {
            UserId = 1,
            FamilyId = family,
            TokenHash = Convert
                .ToHexString(SHA256.HashData("refresh"u8.ToArray()))
                .ToLowerInvariant(),
            ExpiresAtUtc = DateTime.UtcNow.AddDays(1),
        };
        var identity = new AuthenticationIdentity
        {
            UserId = 1,
            NormalizedEmail = "ada@example.com",
            PasswordHash = "hash",
        };
        var outbox = new AuthOutboxMessage
        {
            Type = "verification-email",
            PayloadJson = "{\"UserId\":1,\"Email\":\"ada@example.com\"}",
        };

        await repository.AddAsync(
            identity,
            session,
            [
                new ConsentRecord
                {
                    UserId = 1,
                    Type = "terms",
                    VersionId = "terms-v1",
                    AcceptedAtUtc = DateTime.UtcNow,
                },
            ],
            [new AuthAuditEvent { UserId = 1, EventType = "account_created" }],
            [outbox]
        );

        Assert.NotNull(await repository.FindIdentityAsync("ada@example.com"));
        Assert.Equal(session.Id, (await repository.FindSessionAsync(session.TokenHash))?.Id);
        var claimed = await repository.ClaimDueOutboxAsync(
            DateTime.UtcNow.AddMinutes(1),
            DateTime.UtcNow.AddMinutes(6),
            10
        );
        Assert.Single(claimed);
        Assert.Equal(outbox.Id, claimed[0].Id);
        Assert.NotNull(claimed[0].LeaseId);
        Assert.Empty(
            await repository.ClaimDueOutboxAsync(
                DateTime.UtcNow.AddMinutes(1),
                DateTime.UtcNow.AddMinutes(6),
                10
            )
        );

        var rotated = await repository.TryRotateSessionAsync(
            session.Id,
            DateTime.UtcNow,
            new RefreshSession
            {
                UserId = 1,
                FamilyId = family,
                TokenHash = "replacement-by-rotation",
                ExpiresAtUtc = DateTime.UtcNow.AddDays(1),
            }
        );
        Assert.True(rotated);
        Assert.False(
            await repository.TryRotateSessionAsync(
                session.Id,
                DateTime.UtcNow,
                new RefreshSession
                {
                    UserId = 1,
                    FamilyId = family,
                    TokenHash = "rejected-rotation",
                    ExpiresAtUtc = DateTime.UtcNow.AddDays(1),
                }
            )
        );

        await repository.MarkOutboxFailedAsync(
            outbox.Id,
            claimed[0].LeaseId!.Value,
            1,
            DateTime.UtcNow.AddMinutes(1),
            "safe_error"
        );
        await repository.SaveAsync();
        var reclaimed = await repository.ClaimDueOutboxAsync(
            DateTime.UtcNow.AddMinutes(2),
            DateTime.UtcNow.AddMinutes(7),
            10
        );
        Assert.Single(reclaimed);
        await repository.MarkOutboxProcessedAsync(
            outbox.Id,
            reclaimed[0].LeaseId!.Value,
            DateTime.UtcNow
        );
        await repository.AddSessionAsync(
            new RefreshSession
            {
                UserId = 1,
                FamilyId = family,
                TokenHash = "replacement",
                ExpiresAtUtc = DateTime.UtcNow.AddDays(1),
            }
        );
        await repository.RevokeFamilyAsync(family, DateTime.UtcNow);
        await repository.SaveAsync();

        Assert.Empty(
            await repository.ClaimDueOutboxAsync(
                DateTime.UtcNow.AddDays(1),
                DateTime.UtcNow.AddDays(1).AddMinutes(5),
                10
            )
        );
    }

    [Fact]
    public async Task Entitlement_resolver_unions_profiles_and_falls_back_when_empty()
    {
        var billing = new StubBillingService
        {
            Subscriptions =
            [
                new BillingSubscriptionSnapshot(1, 1, 10, "PRO"),
                new BillingSubscriptionSnapshot(2, 2, null, "FREE"),
            ],
        };
        var profiling = new StubProfilingService
        {
            Shapes = new Dictionary<int, AccessShapeSnapshot>
            {
                [10] = new(new HashSet<string> { "PdfExport" }),
            },
            Starter = new(new HashSet<string> { "ResumeCreate" }),
        };
        var resolver = new SubscriptionEntitlementResolver(billing, profiling);

        var capabilities = await resolver.ResolveAsync(1);
        Assert.Equal(["PdfExport"], capabilities);

        billing.Subscriptions = [];
        capabilities = await resolver.ResolveAsync(1);
        Assert.Equal(["ResumeCreate"], capabilities);
    }

    [Fact]
    public async Task Auth_handlers_delegate_to_registration_service()
    {
        var service = new StubRegistrationService();
        Assert.Same(
            service.Registered,
            await new RegisterCommandHandler(service).Handle(
                new RegisterCommand(new(), null, null),
                CancellationToken.None
            )
        );
        Assert.Same(
            service.Tokens,
            await new RefreshCommandHandler(service).Handle(
                new RefreshCommand(new()),
                CancellationToken.None
            )
        );
        Assert.True(
            await new LogoutCommandHandler(service).Handle(
                new LogoutCommand("refresh"),
                CancellationToken.None
            )
        );
        Assert.True(
            await new ResendVerificationCommandHandler(service).Handle(
                new ResendVerificationCommand(new()),
                CancellationToken.None
            )
        );
    }

    [Fact]
    public void Auth_composition_registers_owned_services()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(
            new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["Auth:SigningKey"] = "unit-test-signing-key-with-at-least-32-bytes",
                    }
                )
                .Build()
        );
        services.AddAuthModulePersistence();
        using var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetRequiredService<IPasswordHasher>());
        Assert.NotNull(provider.GetRequiredService<ITokenService>());
        Assert.NotNull(provider.GetRequiredService<IRegistrationThrottle>());
        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(IAuthRepository));
    }

    [Fact]
    public async Task Auth_seeder_creates_required_consent_versions_idempotently()
    {
        using var scope = new SqliteAppDbContextScope();
        var seeder = new AuthModuleSeeder();

        await seeder.SeedAsync(scope.DbContext, TestContext.Current.CancellationToken);
        scope.DbContext.ChangeTracker.Clear();
        await seeder.SeedAsync(scope.DbContext, TestContext.Current.CancellationToken);

        var versions = await scope
            .DbContext.Set<AuthConsentVersion>()
            .OrderBy(x => x.Type)
            .ToArrayAsync(TestContext.Current.CancellationToken);

        Assert.Equal(["marketing", "privacy", "terms"], versions.Select(x => x.Type));
        Assert.Equal([false, true, true], versions.Select(x => x.Required));
        Assert.Equal(3, versions.Length);
    }

    [Fact]
    public void Auth_model_contains_required_relationships_and_unique_indexes()
    {
        using var scope = new SqliteAppDbContextScope();
        var identity = scope.DbContext.Model.FindEntityType(typeof(AuthenticationIdentity));
        var session = scope.DbContext.Model.FindEntityType(typeof(RefreshSession));
        var consent = scope.DbContext.Model.FindEntityType(typeof(ConsentRecord));

        Assert.NotNull(identity);
        Assert.NotNull(identity!.FindNavigation(nameof(AuthenticationIdentity.User)));
        Assert.Contains(
            identity.GetIndexes(),
            index =>
                index.IsUnique
                && index.Properties.Single().Name == nameof(AuthenticationIdentity.NormalizedEmail)
        );
        Assert.Contains(
            session!.GetIndexes(),
            index =>
                index.IsUnique && index.Properties.Single().Name == nameof(RefreshSession.TokenHash)
        );
        Assert.Contains(
            consent!.GetIndexes(),
            index =>
                index.IsUnique
                && index
                    .Properties.Select(x => x.Name)
                    .SequenceEqual([
                        nameof(ConsentRecord.UserId),
                        nameof(ConsentRecord.Type),
                        nameof(ConsentRecord.VersionId),
                    ])
        );
    }

    [Fact]
    public async Task Auth_endpoint_mapping_registers_all_public_routes()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddSingleton<IValidator<RegisterRequest>, RegisterRequestValidator>();
        builder.Services.AddSingleton(Substitute.For<IMediator>());
        builder.Services.AddSingleton(Substitute.For<IRegistrationService>());
        await using var app = builder.Build();

        IEndpointRouteBuilder routeBuilder = app;
        routeBuilder.MapAuthModuleApis();

        var routes = routeBuilder
            .DataSources.SelectMany(source => source.Endpoints)
            .OfType<RouteEndpoint>()
            .Select(endpoint => endpoint.RoutePattern.RawText)
            .Where(route => route is not null)
            .ToArray();

        Assert.Equal(5, routes.Length);
        Assert.Contains("/api/v1/auth/register", routes);
        Assert.Contains("/api/v1/auth/refresh", routes);
        Assert.Contains("/api/v1/auth/logout", routes);
        Assert.Contains("/api/v1/auth/verify-email/resend", routes);
        Assert.Contains("/api/v1/bootstrap", routes);
    }

    [Fact]
    public async Task Logging_side_effect_handler_accepts_a_dispatch()
    {
        var handler = new LoggingAuthSideEffectHandler(
            NullLogger<LoggingAuthSideEffectHandler>.Instance
        );

        await handler.HandleAsync("welcome-email", 42, "ada@example.com");
    }

    private sealed class StubBillingService : IBillingRegistrationService
    {
        public IReadOnlyList<BillingSubscriptionSnapshot> Subscriptions { get; set; } = [];

        public Task<BillingRegistrationSnapshot?> AddStarterRegistrationBillingAsync(
            int u,
            int p,
            CancellationToken c = default
        ) => Task.FromResult<BillingRegistrationSnapshot?>(new(1, 1, p, "FREE"));

        public Task<
            IReadOnlyList<BillingSubscriptionSnapshot>
        > ListActiveSubscriptionSnapshotsAsync(int u, CancellationToken c = default) =>
            Task.FromResult(Subscriptions);
    }

    private sealed class StubProfilingService : IProfilingRegistrationService
    {
        public Dictionary<int, AccessShapeSnapshot> Shapes { get; set; } = [];
        public AccessShapeSnapshot Starter { get; set; } = new(new HashSet<string>());

        public Task<ProfileRegistrationSnapshot> CreateRegistrationUserAsync(
            ProfileRegistrationInput i,
            CancellationToken c = default
        ) => Task.FromResult(new ProfileRegistrationSnapshot(1));

        public Task AddRegistrationBaselineAsync(
            RegistrationBaselineInput i,
            CancellationToken c = default
        ) => Task.CompletedTask;

        public Task<AccessShapeSnapshot> GetAccessShapeAsync(
            int i,
            CancellationToken c = default
        ) => Task.FromResult(Shapes[i]);

        public Task<AccessShapeSnapshot> GetStarterAccessShapeAsync(
            CancellationToken c = default
        ) => Task.FromResult(Starter);

        public Task<StarterAccessProfileSnapshot?> GetStarterAccessProfileAsync(
            CancellationToken c = default
        ) => Task.FromResult<StarterAccessProfileSnapshot?>(new(1));
    }

    private sealed class StubRegistrationService : IRegistrationService
    {
        public RegisterResponse Registered { get; } =
            new(
                1,
                new("access", "refresh", DateTime.UtcNow, DateTime.UtcNow),
                new(
                    "active",
                    "pending",
                    "FREE",
                    "/app/templates",
                    "homepage",
                    null,
                    "continue",
                    new Dictionary<string, bool>(),
                    []
                )
            );
        public AuthTokens Tokens { get; } =
            new("access", "refresh", DateTime.UtcNow, DateTime.UtcNow);

        public Task<RegisterResponse> RegisterAsync(
            RegisterCommand c,
            CancellationToken ct = default
        ) => Task.FromResult(Registered);

        public Task<AuthTokens> RefreshAsync(RefreshCommand c, CancellationToken ct = default) =>
            Task.FromResult(Tokens);

        public Task<bool> LogoutAsync(LogoutCommand c, CancellationToken ct = default) =>
            Task.FromResult(true);

        public Task<bool> ResendVerificationAsync(
            ResendVerificationCommand c,
            CancellationToken ct = default
        ) => Task.FromResult(true);

        public BootstrapResponse CalculateBootstrap(string source, string? selectedTemplateId) =>
            Registered.Bootstrap;
    }
}
