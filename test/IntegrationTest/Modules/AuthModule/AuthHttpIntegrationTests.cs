using System.Net;
using System.Net.Http.Json;
using ResumeEnhancer.Tests.Integration.Modules.AuthModule.TestSupport;
using ResumeEnhancer.TestUtilities.IntegrationSupport;
using Shouldly;

namespace ResumeEnhancer.Tests.Integration.Modules.AuthModule;

[Collection("Sequential_AuthModule")]
public sealed class AuthHttpIntegrationTests(AuthModuleIntegrationTestFixture fixture)
{
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
            json.RootElement.GetProperty("errorCode")
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
        json.RootElement.GetProperty("errorCode").GetString().ShouldBe("AUTH_EMAIL_IN_USE");
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
        var refreshToken = registration
            .RootElement.GetProperty("tokens")
            .GetProperty("refreshToken")
            .GetString();
        refreshToken.ShouldNotBeNullOrWhiteSpace();

        using var refresh = await client.PostAsJsonAsync(
            "/api/v1/auth/refresh",
            new { RefreshToken = refreshToken },
            cancellationToken
        );
        using var refreshed = await refresh.ReadJsonAsync(HttpStatusCode.OK, cancellationToken);
        var rotatedRefreshToken = refreshed.RootElement.GetProperty("refreshToken").GetString();
        rotatedRefreshToken.ShouldNotBeNullOrWhiteSpace();

        using var logout = await client.PostAsJsonAsync(
            "/api/v1/auth/logout",
            new { RefreshToken = rotatedRefreshToken },
            cancellationToken
        );
        logout.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var resend = await client.PostAsJsonAsync(
            "/api/v1/auth/verify-email/resend",
            new { Email = "unknown@example.com" },
            cancellationToken
        );
        resend.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var replay = await client.PostAsJsonAsync(
            "/api/v1/auth/refresh",
            new { RefreshToken = refreshToken },
            cancellationToken
        );
        using var replayJson = await replay.ReadJsonAsync(
            HttpStatusCode.Unauthorized,
            cancellationToken
        );
        replayJson
            .RootElement.GetProperty("errorCode")
            .GetString()
            .ShouldBe("AUTH_REFRESH_REPLAYED");
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
