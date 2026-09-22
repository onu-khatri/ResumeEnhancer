using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using ResumeEnhancer.AuthModule.DM.Entities;
using ResumeEnhancer.AuthModule.SL.Abstractions;
using ResumeEnhancer.BillingModule.AM.Requests;
using ResumeEnhancer.Core.WebLibrary.Authorization;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.ProfilingModule.DM.Entities;
using ResumeEnhancer.Tests.Integration.Modules.AuthModule.TestSupport;
using ResumeEnhancer.Tests.Integration.TestSupport;
using ResumeEnhancer.TestUtilities.IntegrationSupport;
using Shouldly;

namespace ResumeEnhancer.Tests.Integration.Modules.AuthModule;

[Collection("Sequential_Integration")]
public sealed class AuthHttpIntegrationTests(IntegrationTestAssemblyFixture fixture)
{
    [Fact]
    public async Task ResetAndSeed_rebases_shared_fake_time_between_tests()
    {
        fixture.TimeProvider.Advance(TimeSpan.FromDays(7));

        await fixture.ResetAndSeedAsync(TestContext.Current.CancellationToken);

        fixture.TimeProvider.GetUtcNow().ShouldBe(IntegrationTestAssemblyFixture.InitialUtcNow);
    }

    [Fact]
    public async Task Protected_endpoint_requires_authentication_even_when_a_client_identity_header_is_forged()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await fixture.ResetAndSeedAsync(cancellationToken);

        using var client = fixture.Utilities.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/resumes/1");
        request.Headers.Add("X-User-Id", "1");
        request.Headers.Add("X-Audit-UserId", "1");

        using var response = await client.SendAsync(request, cancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        body.ShouldNotContain("X-User-Id");
        body.ShouldNotContain("X-Audit-UserId");
    }

    [Fact]
    public async Task Profiling_role_and_access_profile_routes_allow_a_privileged_authenticated_principal()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await fixture.ResetAndSeedAsync(cancellationToken);
        using var setupper = fixture.RealUtilities.CreateSetupper();
        using var client = fixture.RealUtilities.CreateClient();
        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            AuthApiTestData.ValidRegistration("profiling-admin@example.com"),
            cancellationToken);
        using var registration = await register.ReadJsonAsync(HttpStatusCode.Created, cancellationToken);
        var userId = registration.RootElement.GetProperty("userId").GetInt32();
        await VerifyIdentityAsync(setupper, "profiling-admin@example.com", cancellationToken);

        var dbContext = (AppDbContext)setupper.GetFreshDbContext();
        var user = await dbContext.Set<User>()
            .Include(item => item.UserAccessProfiles)
            .SingleAsync(item => item.Id == userId, cancellationToken);
        var administrator = await dbContext.Set<AccessProfile>()
            .SingleAsync(item => item.Code == "Administrator", cancellationToken);
        var adminSource = await dbContext.Set<AccessProfileSource>()
            .SingleAsync(item => item.Code == "admin", cancellationToken);
        var adminRole = await dbContext.Set<Role>()
            .SingleAsync(item => item.Code == "ViewAdminPortal", cancellationToken);
        adminRole.Capability = "ViewAdminPortal";
        if (!await dbContext.Set<AccessProfileRole>()
                .AnyAsync(item => item.AccessProfileId == administrator.Id && item.RoleId == adminRole.Id, cancellationToken))
        {
            dbContext.Add(new AccessProfileRole
            {
                Guid = Guid.NewGuid(),
                Code = $"{administrator.Code}:{adminRole.Id}",
                AccessProfileId = administrator.Id,
                RoleId = adminRole.Id,
            });
        }

        var assignment = user.UserAccessProfiles.Single();
        assignment.AccessProfileId = administrator.Id;
        assignment.AccessProfileSourceId = adminSource.Id;
        assignment.Enabled = true;
        await dbContext.SaveChangesAsync(cancellationToken);

        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new { Email = "profiling-admin@example.com", Password = "Password!1234" },
            cancellationToken);
        using var loginJson = await login.ReadJsonAsync(HttpStatusCode.OK, cancellationToken);
        var accessToken = loginJson.RootElement.GetProperty("accessToken").GetString();
        accessToken.ShouldNotBeNullOrWhiteSpace();
        var principal = await fixture.RealUtilities.Services.GetRequiredService<ITokenService>()
            .ValidateAccessTokenAsync(accessToken!, cancellationToken);
        principal.ShouldNotBeNull();
        var subject = principal!.FindFirst("sub")?.Value;
        var session = principal.FindFirst("sid")?.Value;
        int.TryParse(subject, out var authenticatedUserId).ShouldBeTrue();
        Guid.TryParseExact(session, "N", out var sessionKey).ShouldBeTrue();
        var state = await fixture.RealUtilities.Services.GetRequiredService<IAuthCurrentStateService>()
            .EvaluateAsync(authenticatedUserId, sessionKey, cancellationToken);
        state.IsValid.ShouldBeTrue(state.FailureCode);

        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        using var roles = await client.GetAsync("/api/profiling/roles/", cancellationToken);
        using var accessProfiles = await client.GetAsync(
            "/api/profiling/access-profiles/",
            cancellationToken);

        var rolesBody = await roles.Content.ReadAsStringAsync(cancellationToken);
        roles.StatusCode.ShouldBe(HttpStatusCode.OK, rolesBody);
        accessProfiles.StatusCode.ShouldBe(HttpStatusCode.OK);
        using var rolesJson = await roles.ReadJsonAsync(HttpStatusCode.OK, cancellationToken);
        using var profilesJson = await accessProfiles.ReadJsonAsync(HttpStatusCode.OK, cancellationToken);
        rolesJson.RootElement.EnumerateArray()
            .Any(item => item.GetProperty("code").GetString() == "ViewAdminPortal")
            .ShouldBeTrue();
        profilesJson.RootElement.EnumerateArray()
            .Any(item => item.GetProperty("code").GetString() == "Administrator")
            .ShouldBeTrue();
    }

    [Fact]
    public async Task Registration_throttle_tracks_email_and_ip_windows_independently()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await fixture.ResetAndSeedAsync(cancellationToken);
        var throttle = fixture.RealUtilities.Services.GetRequiredService<IRegistrationThrottle>();

        for (var attempt = 0; attempt < 5; attempt++)
        {
            var allowed = await throttle.TryConsumeAsync(
                LimiterOperation.Login,
                "same@example.com",
                null,
                "198.51.100.10",
                cancellationToken);
            allowed.Allowed.ShouldBeTrue();
        }

        // The email window is independent of the IP window.
        (await throttle.TryConsumeAsync(
            LimiterOperation.Login,
            "same@example.com",
            null,
            "198.51.100.11",
            cancellationToken)).Allowed.ShouldBeFalse();

        // The IP window is independent of the email window.
        (await throttle.TryConsumeAsync(
            LimiterOperation.Login,
            "different@example.com",
            null,
            "198.51.100.10",
            cancellationToken)).Allowed.ShouldBeFalse();

        (await throttle.TryConsumeAsync(
            LimiterOperation.Login,
            "different@example.com",
            null,
            "198.51.100.11",
            cancellationToken)).Allowed.ShouldBeTrue();
    }

    [Fact]
    public async Task Real_bearer_handler_rejects_malformed_expired_and_forged_tokens_safely()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await fixture.ResetAndSeedAsync(cancellationToken);
        using var client = fixture.RealUtilities.CreateClient();

        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            AuthApiTestData.ValidRegistration("bearer-validation@example.com"),
            cancellationToken);
        using var registration = await register.ReadJsonAsync(HttpStatusCode.Created, cancellationToken);
        var accessToken = registration.RootElement.GetProperty("tokens")
            .GetProperty("accessToken").GetString();
        accessToken.ShouldNotBeNullOrWhiteSpace();
        fixture.RealUtilities.Services.GetRequiredService<TimeProvider>()
            .ShouldBeSameAs(fixture.TimeProvider);

        async Task<HttpResponseMessage> SendBearerAsync(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/auth/me");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return await client.SendAsync(request, cancellationToken);
        }

        using var malformed = await SendBearerAsync("not-a-jwt");
        malformed.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        using var forged = await SendBearerAsync($"{accessToken}.forged");
        forged.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        fixture.TimeProvider.Advance(TimeSpan.FromMinutes(31));
        using var expired = await SendBearerAsync(accessToken);
        expired.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        (await expired.Content.ReadAsStringAsync(cancellationToken)).ShouldNotContain(accessToken);
    }

    [Fact]
    public async Task Logout_rate_limit_returns_safe_problem_details_without_refresh_secret()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await fixture.ResetAndSeedAsync(cancellationToken);
        using var client = fixture.Utilities.CreateClient();
        const string refreshToken = "invalid-logout-refresh-token";
        const string csrfToken = "synthetic-csrf-proof";

        for (var attempt = 0; attempt < 5; attempt++)
        {
            using var allowed = new HttpRequestMessage(HttpMethod.Post, "/api/v1/auth/logout");
            AddBrowserHeaders(allowed, refreshToken, csrfToken, "https://localhost");
            using var response = await client.SendAsync(allowed, cancellationToken);
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        using var limited = new HttpRequestMessage(HttpMethod.Post, "/api/v1/auth/logout");
        AddBrowserHeaders(limited, refreshToken, csrfToken, "https://localhost");
        using var limitedResponse = await client.SendAsync(limited, cancellationToken);
        using var limitedJson = await limitedResponse.ReadJsonAsync(
            HttpStatusCode.TooManyRequests,
            cancellationToken);
        limitedJson.RootElement.GetProperty("code").GetString().ShouldBe("AUTH_RATE_LIMITED");
        limitedJson.RootElement.GetProperty("detail").GetString().ShouldBe("Too many authentication attempts.");
        limitedJson.RootElement.ToString().ShouldNotContain(refreshToken);
    }

    [Fact]
    public async Task Authenticated_principal_controls_me_and_resume_search_despite_forged_identity_headers()
    {
        using var setupper = fixture.RealUtilities.CreateSetupper();
        var cancellationToken = TestContext.Current.CancellationToken;
        await fixture.ResetAndSeedAsync(cancellationToken);
        using var client = fixture.RealUtilities.CreateClient();
        const string email = "principal-ownership@example.com";

        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            AuthApiTestData.ValidRegistration(email),
            cancellationToken);
        using var registration = await register.ReadJsonAsync(HttpStatusCode.Created, cancellationToken);
        var userId = registration.RootElement.GetProperty("userId").GetInt32();
        await VerifyIdentityAsync(setupper, email, cancellationToken);

        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new { Email = email, Password = "Password!1234" },
            cancellationToken);
        using var loginJson = await login.ReadJsonAsync(HttpStatusCode.OK, cancellationToken);
        var accessToken = loginJson.RootElement.GetProperty("accessToken").GetString();
        accessToken.ShouldNotBeNullOrWhiteSpace();

        using var me = new HttpRequestMessage(HttpMethod.Get, "/api/v1/auth/me");
        me.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        me.Headers.Add("X-User-Id", "999999");
        me.Headers.Add("X-Audit-UserId", "999999");
        using var meResponse = await client.SendAsync(me, cancellationToken);
        using var meJson = await meResponse.ReadJsonAsync(HttpStatusCode.OK, cancellationToken);
        meJson.RootElement.GetProperty("userId").GetInt32().ShouldBe(userId);

        using var search = new HttpRequestMessage(HttpMethod.Post, "/api/resumes/search")
        {
            Content = JsonContent.Create(new { UserId = 999999, PageNumber = 1, PageSize = 10 }),
        };
        search.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        search.Headers.Add("X-User-Id", "999999");
        using var searchResponse = await client.SendAsync(search, cancellationToken);
        searchResponse.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Profiling_role_and_access_profile_routes_reject_guest_requests()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await fixture.ResetAndSeedAsync(cancellationToken);
        using var client = fixture.RealUtilities.CreateClient();

        using var roles = await client.GetAsync("/api/profiling/roles/", cancellationToken);
        using var accessProfiles = await client.GetAsync(
            "/api/profiling/access-profiles/",
            cancellationToken);

        roles.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        accessProfiles.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Invalid_refresh_transport_redacts_the_supplied_secret()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await fixture.ResetAndSeedAsync(cancellationToken);
        using var client = fixture.RealUtilities.CreateClient();
        const string suppliedSecret = "refresh-secret-that-must-not-appear";

        using var response = await client.PostAsJsonAsync(
            "/api/v1/auth/refresh",
            new { RefreshToken = suppliedSecret },
            cancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        body.ShouldNotContain(suppliedSecret);
        if (response.Headers.TryGetValues("Set-Cookie", out var setCookies))
            setCookies.ShouldAllBe(cookie => !cookie.Contains(suppliedSecret, StringComparison.Ordinal));
    }

    [Fact]
    public async Task Guest_template_endpoint_is_reachable_without_authentication_when_guest_access_is_allowed()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await fixture.ResetAndSeedAsync(cancellationToken);

        using var response = await fixture.Utilities.CreateClient().GetAsync(
            "/api/templates/",
            cancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Login_rate_limit_returns_safe_response_without_disclosing_account_state_or_secrets()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await fixture.ResetAndSeedAsync(cancellationToken);
        using var client = fixture.Utilities.CreateClient();

        for (var attempt = 0; attempt < 5; attempt++)
        {
            using var response = await client.PostAsJsonAsync(
                "/api/v1/auth/login",
                new { Email = "unknown-login@example.com", Password = "WrongPassword!123" },
                cancellationToken);

            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            body.ShouldNotContain("WrongPassword!123");
            body.ShouldNotContain("unknown-login@example.com");
        }

        using var limited = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new { Email = "unknown-login@example.com", Password = "WrongPassword!123" },
            cancellationToken);

        limited.StatusCode.ShouldBe(HttpStatusCode.TooManyRequests);
        var limitedBody = await limited.Content.ReadAsStringAsync(cancellationToken);
        limitedBody.ShouldNotContain("WrongPassword!123");
        limitedBody.ShouldNotContain("unknown-login@example.com");
    }

    [Fact]
    public void ApiRouteInventory_DeclaresExplicitAccessBoundary()
    {
        var endpoints = fixture.Utilities.Services
            .GetServices<EndpointDataSource>()
            .SelectMany(source => source.Endpoints)
            .OfType<RouteEndpoint>()
            .Where(endpoint => endpoint.RoutePattern.RawText?.StartsWith("/api/", StringComparison.Ordinal) == true)
            .ToArray();

        endpoints.ShouldNotBeEmpty();
        var expectedAnonymousRoutes = new HashSet<string>(StringComparer.Ordinal)
        {
            "/api/templates/",
            "/api/templates/{templateId:int}",
            "/api/templates/categories/",
            "/api/templates/categories/{templateCategoryId:int}",
            "/api/v1/auth/register",
            "/api/v1/auth/refresh",
            "/api/v1/auth/logout",
            "/api/v1/auth/verify-email/resend",
            "/api/v1/auth/login",
            "/api/v1/auth/password/forgot",
            "/api/v1/auth/password/reset",
            "/api/v1/auth/verify-email",
            "/api/v1/bootstrap",
        };

        foreach (var endpoint in endpoints)
        {
            var guest = endpoint.Metadata.GetMetadata<GuestAccessMetadata>();
            var protectedAccess = endpoint.Metadata.GetMetadata<ProtectedAccessMetadata>();
            var allowsAnonymous = endpoint.Metadata.GetMetadata<IAllowAnonymous>() is not null;

            (guest is not null).ShouldBe(protectedAccess is null);

            if (guest is not null)
            {
                allowsAnonymous.ShouldBeTrue();
            }

            if (protectedAccess is not null)
            {
                allowsAnonymous.ShouldBeFalse();
            }
        }

        var anonymousRoutes = endpoints
            .Where(endpoint => endpoint.Metadata.GetMetadata<IAllowAnonymous>() is not null)
            .Select(endpoint => endpoint.RoutePattern.RawText)
            .ToHashSet(StringComparer.Ordinal);

        anonymousRoutes.ShouldBe(expectedAnonymousRoutes, ignoreOrder: true);

        var guestGetRoutes = endpoints
            .Where(endpoint => endpoint.Metadata.GetMetadata<GuestAccessMetadata>() is not null)
            .Where(endpoint => endpoint.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods.Contains("GET") == true)
            .Select(endpoint => endpoint.RoutePattern.RawText)
            .ToHashSet(StringComparer.Ordinal);

        guestGetRoutes.ShouldBe(
            [
                "/api/templates/",
                "/api/templates/{templateId:int}",
                "/api/templates/categories/",
                "/api/templates/categories/{templateCategoryId:int}",
                "/api/v1/bootstrap",
            ],
            ignoreOrder: true);

        var templateMutations = endpoints
            .Where(endpoint => endpoint.RoutePattern.RawText?.StartsWith("/api/templates", StringComparison.Ordinal) == true)
            .Where(endpoint => endpoint.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods.Any(method => method is "POST" or "PUT" or "DELETE") == true)
            .ToArray();

        templateMutations.ShouldNotBeEmpty();
        foreach (var endpoint in templateMutations)
        {
            endpoint.Metadata.GetMetadata<ProtectedAccessMetadata>().ShouldNotBeNull();
        }
    }

    [Theory]
    [MemberData(nameof(AuthApiTestData.BootstrapSetups), MemberType = typeof(AuthApiTestData))]
    public async Task Bootstrap_SetupObject_ExercisesRealHttpBoundary(EndpointSetup setup)
    {
        using var setupper = fixture.CreateSetupper();
        var cancellationToken = TestContext.Current.CancellationToken;
        await fixture.ResetAndSeedAsync(cancellationToken);
        await setup.ArrangeAsync(setupper, setup, cancellationToken);

        using var client = fixture.Utilities.CreateClient();
        using var response = await client.GetAsync(setup.Route, cancellationToken);

        await setup.AssertAsync(setupper, response, cancellationToken);
    }

    [Theory]
    [MemberData(nameof(AuthApiTestData.RegisterSetups), MemberType = typeof(AuthApiTestData))]
    public async Task Register_SetupObject_ExercisesRealHttpBoundary(EndpointSetup<object> setup)
    {
        using var setupper = fixture.CreateSetupper();
        var cancellationToken = TestContext.Current.CancellationToken;
        await fixture.ResetAndSeedAsync(cancellationToken);
        await setup.ArrangeAsync(setupper, setup, cancellationToken);

        using var client = fixture.Utilities.CreateClient();
        using var response = await client.PostAsJsonAsync(
            setup.Route,
            setup.Input,
            cancellationToken
        );

        await setup.AssertAsync(setupper, response, cancellationToken);
    }

    [Fact]
    public async Task Registration_and_reset_outbox_persist_only_delivery_envelopes_not_raw_challenges()
    {
        using var setupper = fixture.CreateSetupper();
        var cancellationToken = TestContext.Current.CancellationToken;
        await fixture.ResetAndSeedAsync(cancellationToken);
        const string email = "outbox-secrets@example.com";
        using var client = fixture.Utilities.CreateClient();

        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            AuthApiTestData.ValidRegistration(email),
            cancellationToken);
        register.StatusCode.ShouldBe(HttpStatusCode.Created);

        using var forgot = await client.PostAsJsonAsync(
            "/api/v1/auth/password/forgot",
            new { Email = email },
            cancellationToken);
        forgot.StatusCode.ShouldBe(HttpStatusCode.OK);

        var dbContext = (AppDbContext)setupper.GetFreshDbContext();
        var messages = await dbContext.Set<AuthOutboxMessage>()
            .Where(message => message.Type == "verification-email" || message.Type == "password-reset-email")
            .OrderBy(message => message.Type)
            .ToListAsync(cancellationToken);
        messages.Count.ShouldBe(2);

        foreach (var message in messages)
        {
            message.PayloadJson.ShouldNotContain("RawChallenge");
            message.PayloadJson.ShouldNotContain("TokenHash");
            message.PayloadJson.ShouldNotContain("raw-challenge");
            using var payload = JsonDocument.Parse(message.PayloadJson);
            var challenge = payload.RootElement.GetProperty("Challenge");
            challenge.GetProperty("ProtectedValue").GetString().ShouldNotBeNullOrWhiteSpace();
            challenge.GetProperty("ProtectedValue").GetString().ShouldNotBe(
                payload.RootElement.GetProperty("Email").GetString());
        }
    }

    [Theory]
    [MemberData(nameof(AuthApiTestData.IdempotencySetups), MemberType = typeof(AuthApiTestData))]
    public async Task Register_IdempotencySetup_ExercisesReplayAndConflictBoundary(
        AuthApiTestData.IdempotencySetup setup
    )
    {
        using var setupper = fixture.CreateSetupper();
        var cancellationToken = TestContext.Current.CancellationToken;
        await fixture.ResetAndSeedAsync(cancellationToken);
        using var client = fixture.Utilities.CreateClient();
        const string idempotencyKey = "integration-idempotency-key";
        var request = AuthApiTestData.ValidRegistration(setup.Email, idempotencyKey);

        using var first = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            request,
            cancellationToken
        );
        first.StatusCode.ShouldBe(HttpStatusCode.Created);
        var firstBody = await first.Content.ReadAsStringAsync(cancellationToken);

        using var second = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            setup.Case == "conflict"
                ? AuthApiTestData.ValidRegistration(setup.Email, idempotencyKey, "Byron")
                : request,
            cancellationToken
        );

        if (setup.Case == "replay")
        {
            second.StatusCode.ShouldBe(HttpStatusCode.Created);
            (await second.Content.ReadAsStringAsync(cancellationToken)).ShouldBe(firstBody);
        }
        else
        {
            second.StatusCode.ShouldBe(HttpStatusCode.Conflict);
            using var json = await second.ReadJsonAsync(HttpStatusCode.Conflict, cancellationToken);
            json.RootElement.GetProperty("code")
                .GetString()
                .ShouldBe("AUTH_IDEMPOTENCY_CONFLICT");
        }
    }

    [Theory]
    [MemberData(
        nameof(AuthApiTestData.DuplicateRegistrationSetups),
        MemberType = typeof(AuthApiTestData)
    )]
    public async Task Register_DuplicateSetup_ExercisesConflictBoundary(
        AuthApiTestData.DuplicateRegistrationSetup setup
    )
    {
        using var setupper = fixture.CreateSetupper();
        var cancellationToken = TestContext.Current.CancellationToken;
        await fixture.ResetAndSeedAsync(cancellationToken);

        using var client = fixture.Utilities.CreateClient();
        using var first = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            AuthApiTestData.ValidRegistration(setup.Email),
            cancellationToken
        );
        first.StatusCode.ShouldBe(
            HttpStatusCode.Created,
            await first.Content.ReadAsStringAsync(cancellationToken)
        );

        using var second = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            AuthApiTestData.ValidRegistration($" {setup.Email.ToUpperInvariant()} "),
            cancellationToken
        );
        using var json = await second.ReadJsonAsync(HttpStatusCode.Conflict, cancellationToken);
        json.RootElement.GetProperty("code").GetString().ShouldBe("AUTH_EMAIL_IN_USE");
    }

    [Theory]
    [MemberData(nameof(AuthApiTestData.RefreshSetups), MemberType = typeof(AuthApiTestData))]
    public async Task Refresh_SetupObject_ExercisesRealHttpBoundary(EndpointSetup<object> setup)
    {
        using var setupper = fixture.CreateSetupper();
        var cancellationToken = TestContext.Current.CancellationToken;
        await fixture.ResetAndSeedAsync(cancellationToken);
        await setup.ArrangeAsync(setupper, setup, cancellationToken);

        using var client = fixture.Utilities.CreateClient();
        using var response = await client.PostAsJsonAsync(
            setup.Route,
            setup.Input,
            cancellationToken
        );

        await setup.AssertAsync(setupper, response, cancellationToken);
    }

    [Theory]
    [MemberData(nameof(AuthApiTestData.SessionFlowSetups), MemberType = typeof(AuthApiTestData))]
    public async Task SessionFlow_SetupObject_ExercisesRegisterRefreshLogoutResendAndReplay(
        AuthApiTestData.SessionFlowSetup setup
    )
    {
        using var setupper = fixture.CreateSetupper();
        var cancellationToken = TestContext.Current.CancellationToken;
        await fixture.ResetAndSeedAsync(cancellationToken);

        using var client = fixture.Utilities.CreateClient();
        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            AuthApiTestData.ValidRegistration(setup.Email),
            cancellationToken
        );
        using var registration = await register.ReadJsonAsync(
            HttpStatusCode.Created,
            cancellationToken
        );
        registration.RootElement.GetProperty("tokens").ToString().ShouldNotContain("refreshToken");
        var cookies = ReadCookies(register);
        cookies.ShouldContainKey("__Host-resumeenhancer-refresh");
        cookies.ShouldContainKey("resumeenhancer-csrf");
        register.Headers.GetValues("Set-Cookie").ShouldAllBe(x =>
            x.Contains("Secure", StringComparison.OrdinalIgnoreCase) &&
            x.Contains("SameSite=Lax", StringComparison.OrdinalIgnoreCase) &&
            x.Contains("Path=/", StringComparison.OrdinalIgnoreCase));
        register.Headers.GetValues("Set-Cookie").ShouldAllBe(x => !x.Contains("Domain=", StringComparison.OrdinalIgnoreCase));
        var refreshToken = cookies["__Host-resumeenhancer-refresh"];
        var csrfToken = cookies["resumeenhancer-csrf"];
        await VerifyIdentityAsync(setupper, setup.Email, cancellationToken);

        using var refreshRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/auth/refresh");
        AddBrowserHeaders(refreshRequest, refreshToken, csrfToken, "https://localhost");
        using var refresh = await client.SendAsync(refreshRequest, cancellationToken);
        using var refreshed = await refresh.ReadJsonAsync(HttpStatusCode.OK, cancellationToken);
        refreshed.RootElement.ToString().ShouldNotContain("refreshToken");
        var rotatedCookies = ReadCookies(refresh);
        var rotatedRefreshToken = rotatedCookies["__Host-resumeenhancer-refresh"];
        var rotatedCsrfToken = rotatedCookies["resumeenhancer-csrf"];

        using var logoutRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/auth/logout");
        AddBrowserHeaders(logoutRequest, rotatedRefreshToken, rotatedCsrfToken, "https://localhost");
        using var logout = await client.SendAsync(logoutRequest, cancellationToken);
        logout.StatusCode.ShouldBe(HttpStatusCode.OK);
        logout.Headers.GetValues("Set-Cookie").ShouldAllBe(x => x.Contains("Max-Age=0", StringComparison.OrdinalIgnoreCase));

        using var resend = await client.PostAsJsonAsync(
            "/api/v1/auth/verify-email/resend",
            new { Email = "unknown@example.com" },
            cancellationToken
        );
        resend.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var replayRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/auth/refresh");
        AddBrowserHeaders(replayRequest, refreshToken, csrfToken, "https://localhost");
        using var replay = await client.SendAsync(replayRequest, cancellationToken);
        using var replayJson = await replay.ReadJsonAsync(
            HttpStatusCode.Unauthorized,
            cancellationToken
        );
        replayJson
            .RootElement.GetProperty("code")
            .GetString()
            .ShouldBe("AUTH_REFRESH_REPLAYED");
    }

    [Fact]
    public async Task Real_bearer_handler_rejects_the_old_access_token_after_refresh_rotation()
    {
        using var setupper = fixture.RealUtilities.CreateSetupper();
        var cancellationToken = TestContext.Current.CancellationToken;
        await fixture.ResetAndSeedAsync(cancellationToken);
        using var client = fixture.RealUtilities.CreateClient();
        const string email = "rotated-bearer@example.com";

        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            AuthApiTestData.ValidRegistration(email),
            cancellationToken);
        using var registration = await register.ReadJsonAsync(HttpStatusCode.Created, cancellationToken);
        await VerifyIdentityAsync(setupper, email, cancellationToken);

        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new { Email = email, Password = "Password!1234" },
            cancellationToken);
        using var loginJson = await login.ReadJsonAsync(HttpStatusCode.OK, cancellationToken);
        var oldAccessToken = loginJson.RootElement.GetProperty("accessToken").GetString();
        oldAccessToken.ShouldNotBeNullOrWhiteSpace();
        var cookies = ReadCookies(login);

        using var beforeRotation = new HttpRequestMessage(HttpMethod.Get, "/api/v1/auth/me");
        beforeRotation.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
            "Bearer", oldAccessToken);
        (await client.SendAsync(beforeRotation, cancellationToken)).StatusCode.ShouldBe(HttpStatusCode.OK);

        using var refreshRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/auth/refresh");
        AddBrowserHeaders(
            refreshRequest,
            cookies["__Host-resumeenhancer-refresh"],
            cookies["resumeenhancer-csrf"],
            "https://localhost");
        (await client.SendAsync(refreshRequest, cancellationToken)).StatusCode.ShouldBe(HttpStatusCode.OK);

        using var afterRotation = new HttpRequestMessage(HttpMethod.Get, "/api/v1/auth/me");
        afterRotation.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
            "Bearer", oldAccessToken);
        (await client.SendAsync(afterRotation, cancellationToken)).StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Real_auth_lane_issues_and_validates_production_rs256_jwt()
    {
        using var setupper = fixture.RealUtilities.CreateSetupper();
        var cancellationToken = TestContext.Current.CancellationToken;
        await fixture.ResetAndSeedAsync(cancellationToken);
        using var client = fixture.RealUtilities.CreateClient();
        const string email = "production-jwt@example.com";

        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            AuthApiTestData.ValidRegistration(email),
            cancellationToken);
        register.StatusCode.ShouldBe(HttpStatusCode.Created);
        using var registration = await register.ReadJsonAsync(HttpStatusCode.Created, cancellationToken);
        await VerifyIdentityAsync(setupper, email, cancellationToken);

        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new { Email = email, Password = "Password!1234" },
            cancellationToken);
        using var loginJson = await login.ReadJsonAsync(HttpStatusCode.OK, cancellationToken);
        var accessToken = loginJson.RootElement.GetProperty("accessToken").GetString();
        accessToken.ShouldNotBeNullOrWhiteSpace();

        var parts = accessToken!.Split('.');
        parts.Length.ShouldBe(3);
        using var header = JsonDocument.Parse(DecodeJwtPart(parts[0]));
        using var payload = JsonDocument.Parse(DecodeJwtPart(parts[1]));
        header.RootElement.GetProperty("alg").GetString().ShouldBe("RS256");
        header.RootElement.GetProperty("kid").GetString().ShouldBe("primary");
        payload.RootElement.GetProperty("iss").GetString().ShouldBe("ResumeEnhancer");
        payload.RootElement.GetProperty("aud").GetString().ShouldBe("ResumeEnhancer.Api");

        var keyProvider = fixture.RealUtilities.Services.GetRequiredService<IAuthSigningKeyProvider>();
        using var keySet = await keyProvider.GetValidationKeySetAsync(
            fixture.TimeProvider.GetUtcNow().UtcDateTime,
            cancellationToken);
        keySet.ShouldNotBeNull();
        var directValidation = await new JsonWebTokenHandler().ValidateTokenAsync(
            accessToken,
            new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = keySet!.ValidationKeys.Select(key => new RsaSecurityKey(key.Key) { KeyId = key.KeyIdentifier }),
                ValidateIssuer = true,
                ValidIssuer = "ResumeEnhancer",
                ValidateAudience = true,
                ValidAudience = "ResumeEnhancer.Api",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30),
                RequireSignedTokens = true,
                ValidAlgorithms = [SecurityAlgorithms.RsaSha256],
                RequireExpirationTime = true,
            });
        directValidation.IsValid.ShouldBeTrue(directValidation.Exception?.ToString());

        var tokenService = fixture.RealUtilities.Services.GetRequiredService<ITokenService>();
        var principal = await tokenService.ValidateAccessTokenAsync(accessToken, cancellationToken);
        principal.ShouldNotBeNull();
        var subject = principal!.FindFirst("sub")?.Value
            ?? principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var session = principal.FindFirst("sid")?.Value
            ?? principal.FindFirst(System.Security.Claims.ClaimTypes.Sid)?.Value;
        subject.ShouldBe(registration.RootElement.GetProperty("userId").GetInt32().ToString());
        Guid.TryParseExact(session, "N", out var sessionKey).ShouldBeTrue();
        var state = await fixture.RealUtilities.Services.GetRequiredService<IAuthCurrentStateService>()
            .EvaluateAsync(int.Parse(subject!), sessionKey, cancellationToken);
        state.IsValid.ShouldBeTrue();

        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/auth/me");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        (await client.SendAsync(request, cancellationToken)).StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CORS_preflight_uses_explicit_trusted_origin_allowlist()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await fixture.ResetAndSeedAsync(cancellationToken);
        using var client = fixture.RealUtilities.CreateClient();

        using var trusted = new HttpRequestMessage(HttpMethod.Options, "/api/v1/auth/refresh");
        trusted.Headers.Add("Origin", "https://localhost");
        trusted.Headers.Add("Access-Control-Request-Method", "POST");
        trusted.Headers.Add("Access-Control-Request-Headers", "content-type,x-csrf-token");
        using var trustedResponse = await client.SendAsync(trusted, cancellationToken);
        trustedResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        trustedResponse.Headers.GetValues("Access-Control-Allow-Origin").Single().ShouldBe("https://localhost");
        trustedResponse.Headers.GetValues("Access-Control-Allow-Credentials").Single().ShouldBe("true");

        using var untrusted = new HttpRequestMessage(HttpMethod.Options, "/api/v1/auth/refresh");
        untrusted.Headers.Add("Origin", "https://evil.example");
        untrusted.Headers.Add("Access-Control-Request-Method", "POST");
        using var untrustedResponse = await client.SendAsync(untrusted, cancellationToken);
        untrustedResponse.Headers.Contains("Access-Control-Allow-Origin").ShouldBeFalse();
    }

    [Fact]
    public async Task Cookie_mutations_reject_missing_mismatched_and_untrusted_csrf_proofs()
    {
        using var setupper = fixture.CreateSetupper();
        var cancellationToken = TestContext.Current.CancellationToken;
        await fixture.ResetAndSeedAsync(cancellationToken);
        using var client = fixture.Utilities.CreateClient();

        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            AuthApiTestData.ValidRegistration("csrf@example.com"),
            cancellationToken);
        var cookies = ReadCookies(register);
        var refreshToken = cookies["__Host-resumeenhancer-refresh"];
        var csrfToken = cookies["resumeenhancer-csrf"];

        using var missingHeader = new HttpRequestMessage(HttpMethod.Post, "/api/v1/auth/refresh");
        missingHeader.Headers.Add("Cookie", $"__Host-resumeenhancer-refresh={refreshToken}; resumeenhancer-csrf={csrfToken}");
        missingHeader.Headers.Add("Origin", "https://localhost");
        (await client.SendAsync(missingHeader, cancellationToken)).StatusCode.ShouldBe(HttpStatusCode.Forbidden);

        using var mismatched = new HttpRequestMessage(HttpMethod.Post, "/api/v1/auth/refresh");
        mismatched.Headers.Add("Cookie", $"__Host-resumeenhancer-refresh={refreshToken}; resumeenhancer-csrf={csrfToken}");
        mismatched.Headers.Add("X-CSRF-Token", "wrong");
        mismatched.Headers.Add("Origin", "https://localhost");
        (await client.SendAsync(mismatched, cancellationToken)).StatusCode.ShouldBe(HttpStatusCode.Forbidden);

        using var untrusted = new HttpRequestMessage(HttpMethod.Post, "/api/v1/auth/refresh");
        AddBrowserHeaders(untrusted, refreshToken, csrfToken, "https://evil.example");
        (await client.SendAsync(untrusted, cancellationToken)).StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Authorization_denial_correlation_matches_persisted_audit_correlation()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await fixture.ResetAndSeedAsync(cancellationToken);
        using var setupper = fixture.CreateSetupper();
        await setupper.SetupAccessAsync(1, privileges: "OtherCapability");

        using var response = await fixture.Utilities.CreateClient().PutAsJsonAsync(
            "/api/billing/plans/1",
            new UpdateBillingPlanRequest
            {
                Code = "FREE",
                Description = "Free",
                DisplayName = "Free",
                Price = 0,
                CurrencyId = 1,
                BillingIntervalId = 1,
                AccessProfileId = 2,
                CascadeExistingSubscriptions = false,
            },
            cancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        using var responseJson = await response.ReadJsonAsync(HttpStatusCode.Forbidden, cancellationToken);
        var responseCorrelation = responseJson.RootElement.GetProperty("correlationId").GetString();
        responseCorrelation.ShouldNotBeNullOrWhiteSpace();

        var dbContext = (ResumeEnhancer.Infrastructure.Persistence.AppDbContext)setupper.GetFreshDbContext();
        var audit = await dbContext.Set<AuthAuditEvent>()
            .Where(item => item.EventType == "authorization_denied" && item.UserId == 1)
            .OrderByDescending(item => item.App_CreateDate)
            .FirstAsync(cancellationToken);
        using var metadata = System.Text.Json.JsonDocument.Parse(audit.MetadataJson);
        metadata.RootElement.GetProperty("correlationId").GetString().ShouldBe(responseCorrelation);
    }

    [Fact]
    public async Task Refresh_retry_after_uses_injected_time_at_positive_boundary_and_expiry()
    {
        await AssertRetryAfterTimelineAsync(
            async (client, cancellationToken) => await client.PostAsJsonAsync(
                "/api/v1/auth/refresh",
                new { RefreshToken = "retry-after-refresh" },
                cancellationToken),
            TimeSpan.FromMinutes(15));
    }

    [Fact]
    public async Task Logout_retry_after_uses_injected_time_at_positive_boundary_and_expiry()
    {
        await AssertRetryAfterTimelineAsync(
            async (client, cancellationToken) => await client.PostAsJsonAsync(
                "/api/v1/auth/logout",
                new { RefreshToken = "retry-after-logout" },
                cancellationToken),
            TimeSpan.FromMinutes(15));
    }

    [Fact]
    public async Task Verification_resend_retry_after_uses_injected_time_at_positive_boundary_and_expiry()
    {
        await AssertRetryAfterTimelineAsync(
            async (client, cancellationToken) => await client.PostAsJsonAsync(
                "/api/v1/auth/verify-email/resend",
                new { Email = "retry-after-verification@example.com" },
                cancellationToken),
            TimeSpan.FromHours(1));
    }

    private async Task AssertRetryAfterTimelineAsync(
        Func<HttpClient, CancellationToken, Task<HttpResponseMessage>> send,
        TimeSpan window)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await fixture.ResetAndSeedAsync(cancellationToken);
        using var client = fixture.Utilities.CreateClient();

        for (var attempt = 0; attempt < 5; attempt++)
        {
            using var allowed = await send(client, cancellationToken);
            allowed.StatusCode.ShouldNotBe(HttpStatusCode.TooManyRequests);
        }

        fixture.TimeProvider.Advance(TimeSpan.FromMinutes(1));
        using (var positive = await send(client, cancellationToken))
        {
            positive.StatusCode.ShouldBe(HttpStatusCode.TooManyRequests);
            using var json = await positive.ReadJsonAsync(HttpStatusCode.TooManyRequests, cancellationToken);
            json.RootElement.GetProperty("retryAfterSeconds").GetInt32().ShouldBe(
                (int)(window - TimeSpan.FromMinutes(1)).TotalSeconds);
        }

        fixture.TimeProvider.Advance(window - TimeSpan.FromMinutes(1));
        using (var boundary = await send(client, cancellationToken))
        {
            boundary.StatusCode.ShouldBe(HttpStatusCode.TooManyRequests);
            using var json = await boundary.ReadJsonAsync(HttpStatusCode.TooManyRequests, cancellationToken);
            json.RootElement.GetProperty("retryAfterSeconds").GetInt32().ShouldBe(0);
        }

        fixture.TimeProvider.Advance(TimeSpan.FromTicks(1));
        using var expired = await send(client, cancellationToken);
        expired.StatusCode.ShouldNotBe(HttpStatusCode.TooManyRequests);
    }

    [Fact]
    public async Task Legacy_body_refresh_remains_supported_without_a_refresh_cookie()
    {
        using var setupper = fixture.CreateSetupper();
        var cancellationToken = TestContext.Current.CancellationToken;
        await fixture.ResetAndSeedAsync(cancellationToken);
        using var client = fixture.Utilities.CreateClient();

        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            AuthApiTestData.ValidRegistration("legacy-body@example.com"),
            cancellationToken);
        var cookies = ReadCookies(register);
        var refreshToken = cookies["__Host-resumeenhancer-refresh"];
        await VerifyIdentityAsync(setupper, "legacy-body@example.com", cancellationToken);

        using var refresh = await client.PostAsJsonAsync(
            "/api/v1/auth/refresh",
            new { RefreshToken = refreshToken },
            cancellationToken);

        refresh.StatusCode.ShouldBe(HttpStatusCode.OK);
        using var json = await refresh.ReadJsonAsync(HttpStatusCode.OK, cancellationToken);
        json.RootElement.GetProperty("accessToken").GetString().ShouldNotBeNullOrWhiteSpace();
        json.RootElement.ToString().ShouldNotContain("refreshToken");
    }

    [Fact]
    public async Task Refresh_rejects_requests_that_mix_cookie_and_legacy_body_transports()
    {
        using var setupper = fixture.CreateSetupper();
        var cancellationToken = TestContext.Current.CancellationToken;
        await fixture.ResetAndSeedAsync(cancellationToken);
        using var client = fixture.Utilities.CreateClient();

        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            AuthApiTestData.ValidRegistration("ambiguous-transport@example.com"),
            cancellationToken);
        var cookies = ReadCookies(register);

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/auth/refresh")
        {
            Content = JsonContent.Create(new { RefreshToken = cookies["__Host-resumeenhancer-refresh"] }),
        };
        request.Headers.Add("Cookie", $"__Host-resumeenhancer-refresh={cookies["__Host-resumeenhancer-refresh"]}; resumeenhancer-csrf={cookies["resumeenhancer-csrf"]}");
        request.Headers.Add("Origin", "https://localhost");
        request.Headers.Add("X-CSRF-Token", cookies["resumeenhancer-csrf"]);

        using var response = await client.SendAsync(request, cancellationToken);
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        using var json = await response.ReadJsonAsync(HttpStatusCode.BadRequest, cancellationToken);
        json.RootElement.GetProperty("code").GetString().ShouldBe("AUTH_REFRESH_TRANSPORT_AMBIGUOUS");
    }

    [Theory]
    [InlineData("unverified")]
    [InlineData("locked")]
    [InlineData("disabled")]
    [InlineData("deleted")]
    public async Task Refresh_denies_a_session_after_current_account_state_mutation(string mutation)
    {
        using var setupper = fixture.CreateSetupper();
        var cancellationToken = TestContext.Current.CancellationToken;
        await fixture.ResetAndSeedAsync(cancellationToken);
        using var client = fixture.Utilities.CreateClient();
        var email = $"state-{mutation}@example.com";

        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            AuthApiTestData.ValidRegistration(email),
            cancellationToken);
        using var registration = await register.ReadJsonAsync(HttpStatusCode.Created, cancellationToken);
        var userId = registration.RootElement.GetProperty("userId").GetInt32();
        var cookies = ReadCookies(register);
        await VerifyIdentityAsync(setupper, email, cancellationToken);

        var dbContext = (AppDbContext)setupper.GetFreshDbContext();
        var identity = await dbContext.Set<AuthenticationIdentity>()
            .SingleAsync(item => item.UserId == userId, cancellationToken);
        var user = await dbContext.Set<User>().SingleAsync(item => item.Id == userId, cancellationToken);
        switch (mutation)
        {
            case "unverified":
                identity.EmailVerified = false;
                break;
            case "locked":
                identity.LockedUntilUtc = fixture.TimeProvider.GetUtcNow().UtcDateTime.AddMinutes(15);
                break;
            case "disabled":
                user.IsDeactivated = true;
                break;
            case "deleted":
                user.IsDeleted = true;
                break;
        }
        await dbContext.SaveChangesAsync(cancellationToken);

        using var response = await client.PostAsJsonAsync(
            "/api/v1/auth/refresh",
            new { RefreshToken = cookies["__Host-resumeenhancer-refresh"] },
            cancellationToken);
        using var json = await response.ReadJsonAsync(HttpStatusCode.Unauthorized, cancellationToken);
        json.RootElement.GetProperty("code").GetString().ShouldBe("AUTH_REFRESH_INVALID");
    }

    [Theory]
    [InlineData("disabled")]
    [InlineData("deleted")]
    [InlineData("locked")]
    public async Task Real_bearer_handler_rejects_stale_access_token_after_account_state_mutation(string mutation)
    {
        using var setupper = fixture.RealUtilities.CreateSetupper();
        var cancellationToken = TestContext.Current.CancellationToken;
        await fixture.ResetAndSeedAsync(cancellationToken);
        using var client = fixture.RealUtilities.CreateClient();
        var email = $"bearer-state-{mutation}@example.com";

        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            AuthApiTestData.ValidRegistration(email),
            cancellationToken);
        using var registration = await register.ReadJsonAsync(HttpStatusCode.Created, cancellationToken);
        var userId = registration.RootElement.GetProperty("userId").GetInt32();
        await VerifyIdentityAsync(setupper, email, cancellationToken);

        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new { Email = email, Password = "Password!1234" },
            cancellationToken);
        using var loginJson = await login.ReadJsonAsync(HttpStatusCode.OK, cancellationToken);
        var accessToken = loginJson.RootElement.GetProperty("accessToken").GetString();
        accessToken.ShouldNotBeNullOrWhiteSpace();

        using var before = new HttpRequestMessage(HttpMethod.Get, "/api/v1/auth/me");
        before.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        using var beforeResponse = await client.SendAsync(before, cancellationToken);
        beforeResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        var dbContext = (AppDbContext)setupper.GetFreshDbContext();
        var identity = await dbContext.Set<AuthenticationIdentity>()
            .SingleAsync(item => item.UserId == userId, cancellationToken);
        var user = await dbContext.Set<User>()
            .SingleAsync(item => item.Id == userId, cancellationToken);
        if (mutation == "locked")
            identity.LockedUntilUtc = fixture.TimeProvider.GetUtcNow().UtcDateTime.AddMinutes(15);
        else if (mutation == "disabled")
            user.IsDeactivated = true;
        else
            user.IsDeleted = true;
        await dbContext.SaveChangesAsync(cancellationToken);

        using var after = new HttpRequestMessage(HttpMethod.Get, "/api/v1/auth/me");
        after.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        (await client.SendAsync(after, cancellationToken)).StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData("body-only")]
    [InlineData("cookie-only")]
    [InlineData("cookie-plus-body")]
    public async Task Refresh_handles_real_hosted_unknown_length_transport(string transport)
    {
        using var setupper = fixture.CreateSetupper();
        var cancellationToken = TestContext.Current.CancellationToken;
        await fixture.ResetAndSeedAsync(cancellationToken);
        using var client = fixture.Utilities.CreateClient();

        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            AuthApiTestData.ValidRegistration($"unknown-length-{transport}@example.com"),
            cancellationToken);
        var cookies = ReadCookies(register);
        var refreshToken = cookies["__Host-resumeenhancer-refresh"];
        var csrfToken = cookies["resumeenhancer-csrf"];
        await VerifyIdentityAsync(setupper, $"unknown-length-{transport}@example.com", cancellationToken);
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/auth/refresh");

        if (transport is "cookie-only" or "cookie-plus-body")
            AddBrowserHeaders(request, refreshToken, csrfToken, "https://localhost");
        if (transport is "body-only" or "cookie-plus-body")
            request.Content = new UnknownLengthJsonContent($"{{\"refreshToken\":\"{refreshToken}\"}}");

        using var response = await client.SendAsync(request, cancellationToken);
        if (transport == "cookie-plus-body")
        {
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            using var json = await response.ReadJsonAsync(HttpStatusCode.BadRequest, cancellationToken);
            json.RootElement.GetProperty("code").GetString().ShouldBe("AUTH_REFRESH_TRANSPORT_AMBIGUOUS");
        }
        else
        {
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }
    }

    private static Dictionary<string, string> ReadCookies(HttpResponseMessage response) => response.Headers
        .GetValues("Set-Cookie")
        .Select(value => value.Split(';', 2)[0].Split('=', 2))
        .ToDictionary(parts => parts[0], parts => parts[1], StringComparer.Ordinal);

    private static string DecodeJwtPart(string value)
    {
        var encoded = value.Replace('-', '+').Replace('_', '/');
        encoded = encoded.PadRight(encoded.Length + ((4 - encoded.Length % 4) % 4), '=');
        return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
    }

    private static async Task VerifyIdentityAsync(ISetupper setupper, string email, CancellationToken cancellationToken)
    {
        var dbContext = (AppDbContext)setupper.GetFreshDbContext();
        var identity = await dbContext.Set<AuthenticationIdentity>()
            .SingleAsync(item => item.NormalizedEmail == email, cancellationToken);
        identity.EmailVerified = true;
        identity.EmailVerifiedAtUtc = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static void AddBrowserHeaders(HttpRequestMessage request, string refreshToken, string csrfToken, string origin)
    {
        request.Headers.Add("Cookie", $"__Host-resumeenhancer-refresh={refreshToken}; resumeenhancer-csrf={csrfToken}");
        request.Headers.Add("X-CSRF-Token", csrfToken);
        request.Headers.Add("Origin", origin);
    }

    private sealed class UnknownLengthJsonContent : HttpContent
    {
        private readonly byte[] bytes;

        public UnknownLengthJsonContent(string json)
        {
            bytes = System.Text.Encoding.UTF8.GetBytes(json);
            Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context) =>
            stream.WriteAsync(bytes).AsTask();

        protected override bool TryComputeLength(out long length)
        {
            length = 0;
            return false;
        }

    }

    [Theory]
    [MemberData(
        nameof(AuthApiTestData.AtomicRegistrationSetups),
        MemberType = typeof(AuthApiTestData)
    )]
    public async Task Register_AtomicSetup_ExercisesPersistenceBoundary(EndpointSetup<object> setup)
    {
        using var setupper = fixture.CreateSetupper();
        var cancellationToken = TestContext.Current.CancellationToken;
        await fixture.ResetAndSeedAsync(cancellationToken);

        using var client = fixture.Utilities.CreateClient();
        using var response = await client.PostAsJsonAsync(
            setup.Route,
            setup.Input,
            cancellationToken
        );

        await setup.AssertAsync(setupper, response, cancellationToken);
    }

    [Theory]
    [MemberData(
        nameof(AuthApiTestData.MalformedPayloadSetups),
        MemberType = typeof(AuthApiTestData)
    )]
    public async Task MalformedPayload_SetupObject_ExercisesRealHttpBoundary(
        EndpointSetup<string> setup
    )
    {
        using var setupper = fixture.CreateSetupper();
        var cancellationToken = TestContext.Current.CancellationToken;
        await fixture.ResetAndSeedAsync(cancellationToken);
        await setup.ArrangeAsync(setupper, setup, cancellationToken);

        using var client = fixture.Utilities.CreateClient();
        using var response = await client.PostAsync(
            setup.Route,
            new StringContent(setup.Input, System.Text.Encoding.UTF8, "application/json"),
            cancellationToken
        );

        await setup.AssertAsync(setupper, response, cancellationToken);
    }
}
