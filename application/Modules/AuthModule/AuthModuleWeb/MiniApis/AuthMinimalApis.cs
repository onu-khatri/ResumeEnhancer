using FluentValidation;
using Mediator;
using ResumeEnhancer.AuthModule.AM.Requests;
using ResumeEnhancer.AuthModule.SL.Contracts;
using ResumeEnhancer.AuthModule.SL.Services;
using ResumeEnhancer.AuthModule.SL.Options;
using ResumeEnhancer.AuthModule.SL.Abstractions;
using ResumeEnhancer.AuthModule.Web.Authentication;
using ResumeEnhancer.Core.WebLibrary.Authorization;
using ResumeEnhancer.Core.CommonLibrary.Exceptions;
using System.Security.Claims;

namespace ResumeEnhancer.AuthModule.Web;

public static class AuthMinimalApis
{
    public static IEndpointRouteBuilder MapAuthModuleApis(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/auth").WithTags("Authentication");
        group.MapPost(
            "/login",
            async (LoginRequest request, [Microsoft.AspNetCore.Mvc.FromServices] IValidator<LoginRequest> validator, IMediator mediator, HttpContext context, [Microsoft.AspNetCore.Mvc.FromServices] AuthSecurityOptions options, CancellationToken ct) =>
            {
                var validation = await validator.ValidateAsync(request, ct);
                if (!validation.IsValid) return ApiProblemDetails.Validation(context, validation.ToDictionary());
                try
                {
                    var response = await mediator.Send(new LoginCommand(request, context.Connection.RemoteIpAddress?.ToString(), context.Request.Headers.UserAgent.ToString()), ct);
                    AuthBrowserTransport.SetRefreshCookies(context.Response, response.Tokens, options);
                    return Results.Ok(new { response.UserId, response.Tokens.AccessToken, response.Tokens.AccessTokenExpiresAtUtc, response.Identity });
                }
                catch (AuthException ex) { return AuthProblemDetails.Create(context, ex.Code, ex.StatusCode); }
            }
        ).AllowGuest("Guest", "TemplateView");
        group.MapPost(
            "/register",
            async (
                RegisterRequest request,
                IValidator<RegisterRequest> validator,
                IMediator mediator,
                HttpContext context,
                [Microsoft.AspNetCore.Mvc.FromServices] AuthSecurityOptions options,
                CancellationToken ct
            ) =>
            {
                var result = await validator.ValidateAsync(request, ct);
                if (!result.IsValid)
                    return ApiProblemDetails.Validation(context, result.ToDictionary());
                try
                {
                    var response = await mediator.Send(
                        new RegisterCommand(
                            request,
                            context.Connection.RemoteIpAddress?.ToString(),
                            context.Request.Headers.UserAgent.ToString()
                        ),
                        ct
                    );
                    AuthBrowserTransport.SetRefreshCookies(context.Response, response.Tokens, options);
                    return Results.Created("/api/v1/bootstrap", new { response.UserId, Tokens = new { response.Tokens.AccessToken, response.Tokens.AccessTokenExpiresAtUtc }, response.Bootstrap });
                }
                catch (ResumeEnhancer.AuthModule.SL.Services.AuthException ex)
                {
                    return AuthProblemDetails.Create(context, ex.Code, ex.StatusCode);
                }
            }
        ).AllowGuest("Guest", "TemplateView");
        group.MapPost(
            "/password/change",
            async (ChangePasswordRequest request, [Microsoft.AspNetCore.Mvc.FromServices] IValidator<ChangePasswordRequest> validator, IMediator mediator, HttpContext context, CancellationToken ct) =>
            {
                var validation = await validator.ValidateAsync(request, ct);
                if (!validation.IsValid) return ApiProblemDetails.Validation(context, validation.ToDictionary());
                if (!AuthBrowserTransport.HasValidCookieMutationProof(context.Request, context.RequestServices.GetRequiredService<AuthSecurityOptions>()))
                {
                    await RecordDenialAsync(context, "csrf_denied");
                    return AuthProblemDetails.Create(context, "AUTH_CSRF_INVALID", 403);
                }
                if (!TryGetUserId(context.User, out var userId)) return ApiProblemDetails.Create(context, "AUTH_UNAUTHORIZED", 401);
                try { return Results.Ok(await mediator.Send(new ChangePasswordCommand(request, userId), ct)); }
                catch (AuthException ex) { return AuthProblemDetails.Create(context, ex.Code, ex.StatusCode); }
            }
        ).RequireProtectedAccess();
        group.MapPost(
            "/password/forgot",
            async (ForgotPasswordRequest request, [Microsoft.AspNetCore.Mvc.FromServices] IValidator<ForgotPasswordRequest> validator, IMediator mediator, HttpContext context, CancellationToken ct) =>
            {
                var validation = await validator.ValidateAsync(request, ct);
                if (!validation.IsValid) return ApiProblemDetails.Validation(context, validation.ToDictionary());
                try { return Results.Ok(await mediator.Send(new ForgotPasswordCommand(request, context.Connection.RemoteIpAddress?.ToString(), context.Request.Headers.UserAgent.ToString()), ct)); }
                catch (AuthException ex) { return AuthProblemDetails.Create(context, ex.Code, ex.StatusCode); }
            }
        ).AllowGuest("Guest", "TemplateView");
        group.MapPost(
            "/password/reset",
            async (ResetPasswordRequest request, [Microsoft.AspNetCore.Mvc.FromServices] IValidator<ResetPasswordRequest> validator, IMediator mediator, HttpContext context, CancellationToken ct) =>
            {
                var validation = await validator.ValidateAsync(request, ct);
                if (!validation.IsValid) return ApiProblemDetails.Validation(context, validation.ToDictionary());
                try { return Results.Ok(await mediator.Send(new ResetPasswordCommand(request, context.Connection.RemoteIpAddress?.ToString()), ct)); }
                catch (AuthException ex) { return AuthProblemDetails.Create(context, ex.Code, ex.StatusCode); }
            }
        ).AllowGuest("Guest", "TemplateView");
        group.MapPost(
            "/verify-email",
            async (VerifyEmailRequest request, [Microsoft.AspNetCore.Mvc.FromServices] IValidator<VerifyEmailRequest> validator, IMediator mediator, HttpContext context, CancellationToken ct) =>
            {
                var validation = await validator.ValidateAsync(request, ct);
                if (!validation.IsValid) return ApiProblemDetails.Validation(context, validation.ToDictionary());
                try { return Results.Ok(await mediator.Send(new VerifyEmailCommand(request, context.Connection.RemoteIpAddress?.ToString()), ct)); }
                catch (AuthException ex) { return AuthProblemDetails.Create(context, ex.Code, ex.StatusCode); }
            }
        ).AllowGuest("Guest", "TemplateView");
        group.MapGet(
            "/me",
            async (HttpContext context, [Microsoft.AspNetCore.Mvc.FromServices] IAuthLifecycleService service, CancellationToken ct) =>
            {
                if (!TryGetUserId(context.User, out var userId)) return ApiProblemDetails.Create(context, "AUTH_UNAUTHORIZED", 401);
                var identity = await service.GetMeAsync(userId, ct);
                return identity is null ? ApiProblemDetails.Create(context, "AUTH_UNAUTHORIZED", 401) : Results.Ok(identity);
            }
        ).RequireProtectedAccess();
        group.MapPost(
            "/refresh",
             async (HttpRequest httpRequest, HttpResponse httpResponse, IMediator mediator, [Microsoft.AspNetCore.Mvc.FromServices] IRegistrationThrottle limiter, [Microsoft.AspNetCore.Mvc.FromServices] IAuthAuditRecorder auditRecorder, [Microsoft.AspNetCore.Mvc.FromServices] AuthSecurityOptions options, [Microsoft.AspNetCore.Mvc.FromServices] TimeProvider timeProvider, CancellationToken ct) =>
             {
                var transport = await AuthBrowserTransport.ReadRefreshTransportAsync(httpRequest, options, ct);
                if (transport.Transport == AuthBrowserTransport.RefreshTransport.Ambiguous)
                {
                    await RecordDenialAsync(httpRequest.HttpContext, "replay_denied");
                    return AuthProblemDetails.Create(httpRequest.HttpContext, "AUTH_REFRESH_TRANSPORT_AMBIGUOUS", 400);
                }
                if (!AuthBrowserTransport.HasValidCookieMutationProof(httpRequest, options))
                {
                    await RecordDenialAsync(httpRequest.HttpContext, "csrf_denied");
                    return AuthProblemDetails.Create(httpRequest.HttpContext, "AUTH_CSRF_INVALID", 403);
                }
                var decision = await limiter.TryConsumeAsync(LimiterOperation.Refresh, null,
                    HashOperationSubject(transport.Token), httpRequest.HttpContext.Connection.RemoteIpAddress?.ToString(), ct);
                await RecordLimiterOutcomeAsync(httpRequest.HttpContext, auditRecorder, LimiterOperation.Refresh, decision, ct);
                var nowUtc = timeProvider.GetUtcNow().UtcDateTime;
                if (!decision.Allowed)
                    return ApiProblemDetails.Create(httpRequest.HttpContext, "AUTH_RATE_LIMITED", 429, decision.RetryAfter(nowUtc));
                try
                {
                    var response = await mediator.Send(new RefreshCommand(new RefreshRequest { RefreshToken = transport.Token ?? string.Empty }), ct);
                    AuthBrowserTransport.SetRefreshCookies(httpResponse, response, options);
                    return Results.Ok(new { response.AccessToken, response.AccessTokenExpiresAtUtc });
                }
                catch (ResumeEnhancer.AuthModule.SL.Services.AuthException ex)
                {
                    AuthBrowserTransport.ClearRefreshCookies(httpResponse, options);
                    return AuthProblemDetails.Create(httpRequest.HttpContext, ex.Code, ex.StatusCode);
                }
                catch (Exception) when (!ct.IsCancellationRequested)
                {
                    AuthBrowserTransport.ClearRefreshCookies(httpResponse, options);
                    return AuthProblemDetails.Create(httpRequest.HttpContext, "AUTH_CONFIGURATION_UNAVAILABLE", 503);
                }
            }
        ).AllowGuest("Guest", "TemplateView");
        group.MapPost(
            "/logout",
             async (HttpRequest httpRequest, HttpResponse httpResponse, IMediator mediator, [Microsoft.AspNetCore.Mvc.FromServices] IRegistrationThrottle limiter, [Microsoft.AspNetCore.Mvc.FromServices] IAuthAuditRecorder auditRecorder, [Microsoft.AspNetCore.Mvc.FromServices] AuthSecurityOptions options, [Microsoft.AspNetCore.Mvc.FromServices] TimeProvider timeProvider, CancellationToken ct) =>
             {
                var transport = await AuthBrowserTransport.ReadRefreshTransportAsync(httpRequest, options, ct);
                if (transport.Transport == AuthBrowserTransport.RefreshTransport.Ambiguous)
                {
                    await RecordDenialAsync(httpRequest.HttpContext, "replay_denied");
                    return AuthProblemDetails.Create(httpRequest.HttpContext, "AUTH_REFRESH_TRANSPORT_AMBIGUOUS", 400);
                }
                if (!AuthBrowserTransport.HasValidCookieMutationProof(httpRequest, options))
                {
                    await RecordDenialAsync(httpRequest.HttpContext, "csrf_denied");
                    return AuthProblemDetails.Create(httpRequest.HttpContext, "AUTH_CSRF_INVALID", 403);
                }
                var decision = await limiter.TryConsumeAsync(LimiterOperation.Logout, null,
                    HashOperationSubject(transport.Token), httpRequest.HttpContext.Connection.RemoteIpAddress?.ToString(), ct);
                await RecordLimiterOutcomeAsync(httpRequest.HttpContext, auditRecorder, LimiterOperation.Logout, decision, ct);
                var nowUtc = timeProvider.GetUtcNow().UtcDateTime;
                if (!decision.Allowed)
                    return ApiProblemDetails.Create(httpRequest.HttpContext, "AUTH_RATE_LIMITED", 429, decision.RetryAfter(nowUtc));
                try
                {
                    var result = await mediator.Send(new LogoutCommand(transport.Token ?? string.Empty), ct);
                    AuthBrowserTransport.ClearRefreshCookies(httpResponse, options);
                    return Results.Ok(result);
                }
                catch (Exception) when (!ct.IsCancellationRequested)
                {
                    AuthBrowserTransport.ClearRefreshCookies(httpResponse, options);
                    return AuthProblemDetails.Create(httpRequest.HttpContext, "AUTH_CONFIGURATION_UNAVAILABLE", 503);
                }
            }
        ).AllowGuest("Guest", "TemplateView");
        group.MapPost(
            "/verify-email/resend",
                 async (ResendVerificationRequest request, [Microsoft.AspNetCore.Mvc.FromServices] IValidator<ResendVerificationRequest> validator, IMediator mediator, [Microsoft.AspNetCore.Mvc.FromServices] IRegistrationThrottle limiter, [Microsoft.AspNetCore.Mvc.FromServices] IAuthAuditRecorder auditRecorder, HttpContext context, [Microsoft.AspNetCore.Mvc.FromServices] TimeProvider timeProvider, CancellationToken ct) =>
                 {
                     var validation = await validator.ValidateAsync(request, ct);
                     if (!validation.IsValid) return ApiProblemDetails.Validation(context, validation.ToDictionary());
                     var email = request.Email.Trim().ToLowerInvariant();
                     var decision = await limiter.TryConsumeAsync(LimiterOperation.Verification, email, null,
                         context.Connection.RemoteIpAddress?.ToString(), ct);
                     await RecordLimiterOutcomeAsync(context, auditRecorder, LimiterOperation.Verification, decision, ct);
                     var nowUtc = timeProvider.GetUtcNow().UtcDateTime;
                     if (!decision.Allowed)
                         return ApiProblemDetails.Create(context, "AUTH_RATE_LIMITED", 429, decision.RetryAfter(nowUtc));
                     try { return Results.Ok(await mediator.Send(new ResendVerificationCommand(request), ct)); }
                     catch (AuthException ex) { return AuthProblemDetails.Create(context, ex.Code, ex.StatusCode); }
                 }
         ).AllowGuest("Guest", "TemplateView");
        endpoints.MapGet(
            "/api/v1/bootstrap",
            (HttpRequest request, IRegistrationService service) =>
                Results.Ok(
                    service.CalculateBootstrap(
                        request.Query["source"].ToString(),
                        request.Query["selectedTemplateId"].ToString()
                    )
                )
        ).AllowGuest("Guest", "TemplateView");
        return endpoints;
    }

    private static bool TryGetUserId(ClaimsPrincipal principal, out int userId) =>
        int.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? principal.FindFirstValue("sub"), out userId) && userId > 0;

    private static Task RecordDenialAsync(HttpContext context, string eventType) =>
        context.RequestServices.GetRequiredService<ResumeEnhancer.AuthModule.SL.Abstractions.IAuthAuditRecorder>()
            .RecordAsync(eventType, context.User.GetSubjectId(), context.Connection.RemoteIpAddress?.ToString(),
                System.Text.Json.JsonSerializer.Serialize(new { outcome = "denied", route = context.Request.Path.ToString() }),
                 context.RequestAborted);

    private static async Task RecordLimiterOutcomeAsync(HttpContext context, IAuthAuditRecorder auditRecorder, LimiterOperation operation, LimiterDecision decision, CancellationToken ct)
    {
        if (decision.Degraded)
            await auditRecorder.RecordAsync("limiter_degraded", null, context.Connection.RemoteIpAddress?.ToString(),
                System.Text.Json.JsonSerializer.Serialize(new { operation = operation.ToString(), outcome = "fail_open" }), ct);
        else if (!decision.Allowed)
            await auditRecorder.RecordAsync("throttled", null, context.Connection.RemoteIpAddress?.ToString(),
                System.Text.Json.JsonSerializer.Serialize(new { operation = operation.ToString(), outcome = "denied" }), ct);
    }

    private static string? HashOperationSubject(string? value) => string.IsNullOrWhiteSpace(value)
        ? null
        : Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(value))).ToLowerInvariant();
}

internal static class AuthProblemDetails
{
    public static IResult Create(HttpContext context, string code, int statusCode)
        => ApiProblemDetails.Create(context, code, statusCode);
}
