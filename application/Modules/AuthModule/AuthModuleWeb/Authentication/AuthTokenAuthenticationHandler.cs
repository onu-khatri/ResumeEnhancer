using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using ResumeEnhancer.AuthModule.SL.Abstractions;

namespace ResumeEnhancer.AuthModule.Web.Authentication;

public sealed class AuthTokenAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly ITokenService tokens;
    private readonly IAuthCurrentStateService currentState;

    public AuthTokenAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        TimeProvider timeProvider,
        ITokenService tokens,
        IAuthCurrentStateService currentState)
        : base(options, logger, encoder)
    {
        this.tokens = tokens;
        this.currentState = currentState;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var header) ||
            !header.ToString().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return AuthenticateResult.NoResult();

        var value = header.ToString()[7..].Trim();
        var principal = await tokens.ValidateAccessTokenAsync(value, Context.RequestAborted);
        if (principal is null)
            return AuthenticateResult.Fail("Invalid access token.");
        var subject = principal.FindFirst("sub")?.Value
            ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var session = principal.FindFirst("sid")?.Value
            ?? principal.FindFirst(ClaimTypes.Sid)?.Value;
        if (!int.TryParse(subject, out var userId)
            || !Guid.TryParseExact(session, "N", out var sessionKey))
            return AuthenticateResult.Fail("Invalid access token claims.");
        try
        {
            var state = await currentState.EvaluateAsync(userId, sessionKey, Context.RequestAborted);
            return state.IsValid
                ? AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name))
                : AuthenticateResult.Fail("Authentication state is no longer valid.");
        }
        catch (OperationCanceledException) when (Context.RequestAborted.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception)
        {
            return AuthenticateResult.Fail("Authentication state is unavailable.");
        }
    }
}
