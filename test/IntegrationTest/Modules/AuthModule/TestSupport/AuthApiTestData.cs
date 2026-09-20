using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using ResumeEnhancer.AuthModule.DM.Entities;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.ProfilingModule.DM.Entities;
using ResumeEnhancer.TestUtilities.IntegrationSupport;
using Shouldly;

namespace ResumeEnhancer.Tests.Integration.Modules.AuthModule.TestSupport;

public static class AuthApiTestData
{
    public static IEnumerable<object[]> IdempotencySetups()
    {
        yield return [new IdempotencySetup("replay", "idempotent-replay@example.com")];
        yield return [new IdempotencySetup("conflict", "idempotent-conflict@example.com")];
    }

    public static IEnumerable<object[]> BootstrapSetups()
    {
        yield return
        [
            new EndpointSetup(
                "unknown source falls back to the homepage templates route",
                HttpMethod.Get,
                "/api/v1/bootstrap?source=unknown",
                null,
                async (_, response, cancellationToken) =>
                {
                    using var json = await response.ReadJsonAsync(
                        HttpStatusCode.OK,
                        cancellationToken
                    );
                    json.RootElement.GetProperty("source").GetString().ShouldBe("homepage");
                    json.RootElement.GetProperty("workspaceRoute")
                        .GetString()
                        .ShouldBe("/app/templates");
                }
            ),
        ];

        foreach (
            var (source, route) in new[]
            {
                ("pricing", "/app/billing"),
                ("template-selection", "/app/templates?selected=starter%20one"),
                ("premium-lock", "/app/templates"),
            }
        )
        {
            yield return
            [
                new EndpointSetup(
                    $"{source} source maps to its safe workspace route",
                    HttpMethod.Get,
                    $"/api/v1/bootstrap?source={source}&selectedTemplateId=starter%20one",
                    null,
                    async (_, response, cancellationToken) =>
                    {
                        using var json = await response.ReadJsonAsync(
                            HttpStatusCode.OK,
                            cancellationToken
                        );
                        json.RootElement.GetProperty("workspaceRoute").GetString().ShouldBe(route);
                    }
                ),
            ];
        }
    }

    public static IEnumerable<object[]> RegisterSetups()
    {
        yield return
        [
            new EndpointSetup<object>(
                "valid registration creates account and session tokens",
                HttpMethod.Post,
                "/api/v1/auth/register",
                ValidRegistration("created@example.com"),
                null,
                async (_, response, cancellationToken) =>
                {
                    using var json = await response.ReadJsonAsync(
                        HttpStatusCode.Created,
                        cancellationToken
                    );
                    json.RootElement.GetProperty("userId").GetInt32().ShouldBeGreaterThan(0);
                    json.RootElement.GetProperty("tokens")
                        .GetProperty("accessToken")
                        .GetString()
                        .ShouldNotBeNullOrWhiteSpace();
                    json.RootElement.GetProperty("tokens").ToString().ShouldNotContain("refreshToken");
                    json.RootElement.GetProperty("bootstrap")
                        .GetProperty("workspaceRoute")
                        .GetString()
                        .ShouldBe("/app/templates");
                }
            ),
        ];

        yield return
        [
            new EndpointSetup<object>(
                "invalid registration returns field validation errors",
                HttpMethod.Post,
                "/api/v1/auth/register",
                new
                {
                    FirstName = "",
                    LastName = "",
                    Email = "not-an-email",
                    Password = "weak",
                    TermsConsent = false,
                    PrivacyConsent = false,
                    Source = "https://evil.example",
                },
                null,
                async (_, response, cancellationToken) =>
                {
                    using var json = await response.ReadJsonAsync(
                        HttpStatusCode.BadRequest,
                        cancellationToken
                    );
                    var errors = json.RootElement.GetProperty("errors");
                    errors.TryGetProperty("Email", out var emailErrors).ShouldBeTrue();
                    errors.TryGetProperty("TermsConsent", out var termsErrors).ShouldBeTrue();
                    errors.TryGetProperty("PrivacyConsent", out var privacyErrors).ShouldBeTrue();
                    errors.TryGetProperty("Source", out var sourceErrors).ShouldBeTrue();
                }
            ),
        ];
    }

    public static IEnumerable<object[]> RefreshSetups()
    {
        yield return
        [
            new EndpointSetup<object>(
                "invalid refresh token returns unauthorized",
                HttpMethod.Post,
                "/api/v1/auth/refresh",
                new { RefreshToken = "invalid-refresh-token" },
                null,
                async (_, response, cancellationToken) =>
                {
                    using var json = await response.ReadJsonAsync(
                        HttpStatusCode.Unauthorized,
                        cancellationToken
                    );
                    json.RootElement.GetProperty("code")
                        .GetString()
                        .ShouldBe("AUTH_REFRESH_INVALID");
                }
            ),
        ];
    }

    public static IEnumerable<object[]> MalformedPayloadSetups()
    {
        yield return
        [
            new EndpointSetup<string>(
                "malformed JSON returns bad request",
                HttpMethod.Post,
                "/api/v1/auth/register",
                "{\"email\":",
                null,
                async (_, response, cancellationToken) =>
                    response.StatusCode.ShouldBe(HttpStatusCode.BadRequest)
            ),
        ];
    }

    public static IEnumerable<object[]> DuplicateRegistrationSetups()
    {
        yield return [new DuplicateRegistrationSetup("duplicate@example.com")];
    }

    public static IEnumerable<object[]> SessionFlowSetups()
    {
        yield return [new SessionFlowSetup("session@example.com")];
    }

    public static IEnumerable<object[]> AtomicRegistrationSetups()
    {
        yield return
        [
            new EndpointSetup<object>(
                "registration persists the complete atomic baseline",
                HttpMethod.Post,
                "/api/v1/auth/register",
                ValidRegistration("atomic@example.com"),
                null,
                async (setupper, response, cancellationToken) =>
                {
                    using var json = await response.ReadJsonAsync(
                        HttpStatusCode.Created,
                        cancellationToken
                    );
                    var userId = json.RootElement.GetProperty("userId").GetInt32();
                    var dbContext = (AppDbContext)setupper.GetFreshDbContext();

                    (
                        await dbContext.Set<AuthenticationIdentity>().CountAsync(cancellationToken)
                    ).ShouldBe(1);
                    (await dbContext.Set<RefreshSession>().CountAsync(cancellationToken)).ShouldBe(
                        1
                    );
                    (
                        await dbContext
                            .Set<ConsentRecord>()
                            .CountAsync(x => x.UserId == userId, cancellationToken)
                    ).ShouldBe(3);
                    (
                        await dbContext
                            .Set<AuthAuditEvent>()
                            .CountAsync(x => x.UserId == userId, cancellationToken)
                    ).ShouldBe(2);
                    (
                        await dbContext
                            .Set<AuthOutboxMessage>()
                            .CountAsync(x => x.PayloadJson.Contains($"{userId}"), cancellationToken)
                    ).ShouldBe(2);
                    (
                        await dbContext
                            .Set<UserPreference>()
                            .CountAsync(x => x.UserId == userId, cancellationToken)
                    ).ShouldBe(1);
                    (
                        await dbContext
                            .Set<UserAccessProfile>()
                            .CountAsync(x => x.UserId == userId, cancellationToken)
                    ).ShouldBe(1);
                }
            ),
        ];
    }

    public sealed record SessionFlowSetup(string Email)
    {
        public override string ToString() => $"session flow: {Email}";
    }

    public sealed record DuplicateRegistrationSetup(string Email)
    {
        public override string ToString() => $"duplicate normalized email: {Email}";
    }

    public sealed record IdempotencySetup(string Case, string Email)
    {
        public override string ToString() => $"idempotency {Case}: {Email}";
    }

    public static object ValidRegistration(
        string email,
        string? idempotencyKey = null,
        string lastName = "Lovelace"
    ) =>
        new
        {
            FirstName = "Ada",
            LastName = lastName,
            Email = email,
            Password = "Password!1234",
            TermsConsent = true,
            PrivacyConsent = true,
            MarketingConsent = false,
            Source = "homepage",
            IdempotencyKey = idempotencyKey ?? Guid.NewGuid().ToString("N"),
        };
}
