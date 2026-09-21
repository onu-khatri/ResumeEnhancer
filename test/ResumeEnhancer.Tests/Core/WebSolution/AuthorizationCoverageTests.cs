using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using ResumeEnhancer.AuthModule.SL.Abstractions;
using ResumeEnhancer.Core.WebLibrary.Authorization;
using ResumeEnhancer.ProfilingModule.SL.Integrations;
using ResumeEnhancer.WebSolution.ModulesComposition.Authorization;

namespace ResumeEnhancer.Tests.Unit.Core.WebSolution;

public sealed class AuthorizationCoverageTests
{
    [Fact]
    public void Startup_validator_rejects_unclassified_conflicting_and_invalid_api_metadata()
    {
        var validator = new EndpointAuthorizationStartupValidator();

        Assert.Throws<InvalidOperationException>(() => validator.Validate(Source(Api("/api/unclassified"))));
        Assert.Throws<InvalidOperationException>(() => validator.Validate(Source(Api(
            "/api/conflict",
            new GuestAccessMetadata("Guest", new HashSet<string> { "View" }),
            new ProtectedAccessMetadata(new HashSet<string>(), new HashSet<string>(), new HashSet<string>())))));
        Assert.Throws<InvalidOperationException>(() => validator.Validate(Source(Api(
            "/api/anonymous-protected",
            new ProtectedAccessMetadata(new HashSet<string>(), new HashSet<string>(), new HashSet<string>()),
            new AllowAnonymousAttribute()))));
        Assert.Throws<InvalidOperationException>(() => validator.Validate(Source(Api(
            "/api/guest-without-role",
            new GuestAccessMetadata("Guest", new HashSet<string>())))));

        validator.Validate(Source(Api(
            "/api/guest",
            new GuestAccessMetadata("Guest", new HashSet<string> { "View" }))));
        validator.Validate(Source(Api(
            "/api/protected",
            new ProtectedAccessMetadata(new HashSet<string>(), new HashSet<string>(), new HashSet<string>()))));
        validator.Validate(Source(Api("/public/not-an-api")));
    }

    [Fact]
    public async Task Middleware_allows_guest_and_protected_requests_when_profile_requirements_match()
    {
        var profiling = Substitute.For<IProfilingAuthorizationService>();
        profiling.GetGuestAuthorizationAsync(Arg.Any<CancellationToken>()).Returns(
            new ProfilingAuthorizationSnapshot(
                0,
                new HashSet<string> { "Guest" },
                new HashSet<string> { "View" },
                new HashSet<string>()));
        profiling.GetUserAuthorizationAsync(42, Arg.Any<CancellationToken>()).Returns(
            new ProfilingAuthorizationSnapshot(
                42,
                new HashSet<string>(),
                new HashSet<string> { "Editor" },
                new HashSet<string> { "ResumeRead" }));
        var entitlements = Substitute.For<IEntitlementResolver>();
        entitlements.ResolveAsync(42, Arg.Any<CancellationToken>()).Returns(
            new HashSet<string> { "Pro" });
        var audit = Substitute.For<IAuthAuditRecorder>();
        var nextCalls = 0;
        var middleware = new EndpointAuthorizationMiddleware(_ =>
        {
            nextCalls++;
            return Task.CompletedTask;
        });

        var guest = CreateContext();
        guest.SetEndpoint(new Endpoint(
            _ => Task.CompletedTask,
            new EndpointMetadataCollection(new GuestAccessMetadata("Guest", new HashSet<string> { "View" })),
            "guest"));
        await middleware.InvokeAsync(guest, profiling, entitlements, audit);

        var protectedContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity([new Claim("sub", "42")], "Bearer")),
        };
        protectedContext.SetEndpoint(new Endpoint(
            _ => Task.CompletedTask,
            new EndpointMetadataCollection(new ProtectedAccessMetadata(
                new HashSet<string> { "Editor" },
                new HashSet<string> { "ResumeRead" },
                new HashSet<string> { "Pro" })),
            "protected"));
        await middleware.InvokeAsync(protectedContext, profiling, entitlements, audit);

        Assert.Equal(2, nextCalls);
        await audit.DidNotReceiveWithAnyArgs().RecordAsync(default!, default, default, default!, default, default);
    }

    [Fact]
    public async Task Middleware_fails_closed_for_invalid_subject_and_dependency_errors_but_skips_unauthenticated_protected_requests()
    {
        var profiling = Substitute.For<IProfilingAuthorizationService>();
        profiling.GetGuestAuthorizationAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromException<ProfilingAuthorizationSnapshot?>(new InvalidOperationException("dependency")));
        profiling.GetUserAuthorizationAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<ProfilingAuthorizationSnapshot?>(new InvalidOperationException("dependency")));
        var entitlements = Substitute.For<IEntitlementResolver>();
        var audit = Substitute.For<IAuthAuditRecorder>();
        var nextCalls = 0;
        var middleware = new EndpointAuthorizationMiddleware(_ =>
        {
            nextCalls++;
            return Task.CompletedTask;
        });

        var guest = CreateContext();
        guest.SetEndpoint(new Endpoint(
            _ => Task.CompletedTask,
            new EndpointMetadataCollection(new GuestAccessMetadata("Guest", new HashSet<string> { "View" })),
            "guest"));
        await middleware.InvokeAsync(guest, profiling, entitlements, audit);
        Assert.Equal(StatusCodes.Status403Forbidden, guest.Response.StatusCode);

        var invalidSubject = CreateContext();
        invalidSubject.User = new ClaimsPrincipal(
            new ClaimsIdentity([new Claim("sub", "not-an-id")], "Bearer"));
        invalidSubject.SetEndpoint(new Endpoint(
            _ => Task.CompletedTask,
            new EndpointMetadataCollection(new ProtectedAccessMetadata(
                new HashSet<string>(), new HashSet<string>(), new HashSet<string>())),
            "protected"));
        await middleware.InvokeAsync(invalidSubject, profiling, entitlements, audit);
        Assert.Equal(StatusCodes.Status403Forbidden, invalidSubject.Response.StatusCode);

        var anonymous = new DefaultHttpContext();
        anonymous.SetEndpoint(new Endpoint(
            _ => Task.CompletedTask,
            new EndpointMetadataCollection(new ProtectedAccessMetadata(
                new HashSet<string>(), new HashSet<string>(), new HashSet<string>())),
            "protected-anonymous"));
        await middleware.InvokeAsync(anonymous, profiling, entitlements, audit);

        Assert.Equal(1, nextCalls);
    }

    private static IEnumerable<EndpointDataSource> Source(params Endpoint[] endpoints) =>
        [new StaticEndpointDataSource(endpoints)];

    private static DefaultHttpContext CreateContext() => new()
    {
        RequestServices = new ServiceCollection()
            .AddLogging()
            .AddProblemDetails()
            .BuildServiceProvider(),
    };

    private static RouteEndpoint Api(string route, params object[] metadata) => new(
        _ => Task.CompletedTask,
        RoutePatternFactory.Parse(route),
        0,
        new EndpointMetadataCollection(metadata),
        route);

    private sealed class StaticEndpointDataSource(IReadOnlyList<Endpoint> endpoints) : EndpointDataSource
    {
        public override IReadOnlyList<Endpoint> Endpoints { get; } = endpoints;

        public override IChangeToken GetChangeToken() => new CancellationChangeToken(CancellationToken.None);
    }
}
