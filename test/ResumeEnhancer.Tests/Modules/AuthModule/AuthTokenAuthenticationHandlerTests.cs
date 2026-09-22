using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging.Abstractions;
using ResumeEnhancer.AuthModule.SL.Abstractions;
using ResumeEnhancer.AuthModule.SL.Services;
using ResumeEnhancer.AuthModule.Web.Authentication;
using Shouldly;

namespace ResumeEnhancer.Tests.Unit.Modules.AuthModule;

public sealed class AuthTokenAuthenticationHandlerTests
{
    [Fact]
    public async Task Bearer_authentication_denies_a_token_after_current_state_dependency_failure()
    {
        var sessionId = Guid.NewGuid();
        var handler = new AuthTokenAuthenticationHandler(
            new FixedOptionsMonitor<AuthenticationSchemeOptions>(new AuthenticationSchemeOptions()),
            NullLoggerFactory.Instance,
            UrlEncoder.Default,
            TimeProvider.System,
            new ValidBearerTokenService(sessionId),
            new FailingCurrentStateService());
        var context = new DefaultHttpContext();
        context.Request.Headers.Authorization = "Bearer access-token";

        await handler.InitializeAsync(
            new AuthenticationScheme("Bearer", "Bearer", typeof(AuthTokenAuthenticationHandler)),
            context);

        var result = await handler.AuthenticateAsync();

        result.Succeeded.ShouldBeFalse();
        result.Failure.ShouldNotBeNull();
        result.Failure!.Message.ShouldBe("Authentication state is unavailable.");
        context.User.Identity?.IsAuthenticated.ShouldBeFalse();
    }

    [Fact]
    public async Task Bearer_authentication_denies_a_token_after_current_state_mutation()
    {
        var sessionId = Guid.NewGuid();
        var handler = new AuthTokenAuthenticationHandler(
            new FixedOptionsMonitor<AuthenticationSchemeOptions>(new AuthenticationSchemeOptions()),
            NullLoggerFactory.Instance,
            UrlEncoder.Default,
            TimeProvider.System,
            new ValidBearerTokenService(sessionId),
            new InvalidCurrentStateService());
        var context = new DefaultHttpContext();
        context.Request.Headers.Authorization = "Bearer access-token";

        await handler.InitializeAsync(
            new AuthenticationScheme("Bearer", "Bearer", typeof(AuthTokenAuthenticationHandler)),
            context);

        var result = await handler.AuthenticateAsync();

        result.Succeeded.ShouldBeFalse();
        result.Failure.ShouldNotBeNull();
        result.Failure!.Message.ShouldBe("Authentication state is no longer valid.");
        context.User.Identity?.IsAuthenticated.ShouldBeFalse();
    }

    [Fact]
    public async Task Authentication_returns_no_result_without_a_bearer_header()
    {
        var handler = CreateHandler(new ValidBearerTokenService(Guid.NewGuid()), new InvalidCurrentStateService());
        var context = new DefaultHttpContext();

        await handler.InitializeAsync(
            new AuthenticationScheme("Bearer", "Bearer", typeof(AuthTokenAuthenticationHandler)),
            context);

        var result = await handler.AuthenticateAsync();

        result.None.ShouldBeTrue();
    }

    [Fact]
    public async Task Authentication_rejects_invalid_tokens_and_claims_before_current_state_lookup()
    {
        var invalidTokenHandler = CreateHandler(new InvalidBearerTokenService(), new InvalidCurrentStateService());
        var invalidTokenContext = new DefaultHttpContext();
        invalidTokenContext.Request.Headers.Authorization = "Bearer invalid";
        await invalidTokenHandler.InitializeAsync(
            new AuthenticationScheme("Bearer", "Bearer", typeof(AuthTokenAuthenticationHandler)),
            invalidTokenContext);

        var invalidToken = await invalidTokenHandler.AuthenticateAsync();
        invalidToken.Succeeded.ShouldBeFalse();
        invalidToken.Failure!.Message.ShouldBe("Invalid access token.");

        var invalidClaimsHandler = CreateHandler(new InvalidClaimsTokenService(), new InvalidCurrentStateService());
        var invalidClaimsContext = new DefaultHttpContext();
        invalidClaimsContext.Request.Headers.Authorization = "Bearer malformed-claims";
        await invalidClaimsHandler.InitializeAsync(
            new AuthenticationScheme("Bearer", "Bearer", typeof(AuthTokenAuthenticationHandler)),
            invalidClaimsContext);

        var invalidClaims = await invalidClaimsHandler.AuthenticateAsync();
        invalidClaims.Succeeded.ShouldBeFalse();
        invalidClaims.Failure!.Message.ShouldBe("Invalid access token claims.");
    }

    private static AuthTokenAuthenticationHandler CreateHandler(ITokenService tokens, IAuthCurrentStateService currentState) =>
        new(
            new FixedOptionsMonitor<AuthenticationSchemeOptions>(new AuthenticationSchemeOptions()),
            NullLoggerFactory.Instance,
            UrlEncoder.Default,
            TimeProvider.System,
            tokens,
            currentState);

    private sealed class FixedOptionsMonitor<T>(T value) : IOptionsMonitor<T>
    {
        public T CurrentValue => value;
        public T Get(string? name) => value;
        public IDisposable OnChange(Action<T, string> listener) => new NoopDisposable();
    }

    private sealed class NoopDisposable : IDisposable
    {
        public void Dispose() { }
    }

    private class ValidBearerTokenService(Guid sessionId) : ITokenService
    {
        public virtual Task<ClaimsPrincipal?> ValidateAccessTokenAsync(string token, CancellationToken cancellationToken = default) =>
            Task.FromResult<ClaimsPrincipal?>(new ClaimsPrincipal(new ClaimsIdentity(
                [
                    new Claim("sub", "42"),
                    new Claim("sid", sessionId.ToString("N")),
                ],
                "Bearer")));

        public Task<(string AccessToken, DateTime ExpiresAtUtc)> CreateAccessTokenAsync(int userId, Guid sessionId, CancellationToken cancellationToken = default) =>
            Task.FromResult<(string, DateTime)>(("unused", DateTime.UtcNow));

        public string CreateRefreshToken() => "unused";
        public string HashRefreshToken(string token) => token;
        public Task InvalidateKeyAsync(string keyIdentifier, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class FailingCurrentStateService : IAuthCurrentStateService
    {
        public Task<AuthCurrentState> EvaluateAsync(int userId, Guid sessionKey, CancellationToken cancellationToken = default) =>
            Task.FromException<AuthCurrentState>(new InvalidOperationException("state dependency unavailable"));
    }

    private sealed class InvalidCurrentStateService : IAuthCurrentStateService
    {
        public Task<AuthCurrentState> EvaluateAsync(int userId, Guid sessionKey, CancellationToken cancellationToken = default) =>
            Task.FromResult(new AuthCurrentState(false, "profile_invalid"));
    }

    private sealed class InvalidBearerTokenService : ValidBearerTokenService
    {
        public InvalidBearerTokenService() : base(Guid.NewGuid()) { }

        public override Task<ClaimsPrincipal?> ValidateAccessTokenAsync(string token, CancellationToken cancellationToken = default) =>
            Task.FromResult<ClaimsPrincipal?>(null);
    }

    private sealed class InvalidClaimsTokenService : ValidBearerTokenService
    {
        public InvalidClaimsTokenService() : base(Guid.NewGuid()) { }

        public override Task<ClaimsPrincipal?> ValidateAccessTokenAsync(string token, CancellationToken cancellationToken = default) =>
            Task.FromResult<ClaimsPrincipal?>(new ClaimsPrincipal(new ClaimsIdentity([new Claim("sub", "not-an-int")], "Bearer")));
    }
}
