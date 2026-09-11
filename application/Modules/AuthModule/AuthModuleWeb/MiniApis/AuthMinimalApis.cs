using FluentValidation;
using Mediator;
using ResumeEnhancer.AuthModule.AM.Requests;
using ResumeEnhancer.AuthModule.SL.Contracts;
using ResumeEnhancer.AuthModule.SL.Services;

namespace ResumeEnhancer.AuthModule.Web;

public static class AuthMinimalApis
{
    public static IEndpointRouteBuilder MapAuthModuleApis(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/auth").WithTags("Authentication");
        group.MapPost(
            "/register",
            async (
                RegisterRequest request,
                IValidator<RegisterRequest> validator,
                IMediator mediator,
                HttpContext context,
                CancellationToken ct
            ) =>
            {
                var result = await validator.ValidateAsync(request, ct);
                if (!result.IsValid)
                    return Results.ValidationProblem(result.ToDictionary());
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
                    return Results.Created("/api/v1/bootstrap", response);
                }
                catch (ResumeEnhancer.AuthModule.SL.Services.AuthException ex)
                {
                    return Results.Json(
                        new { errorCode = ex.Code, message = ex.Message },
                        statusCode: ex.StatusCode
                    );
                }
            }
        );
        group.MapPost(
            "/refresh",
            async (RefreshRequest request, IMediator mediator, CancellationToken ct) =>
            {
                try
                {
                    return Results.Ok(await mediator.Send(new RefreshCommand(request), ct));
                }
                catch (ResumeEnhancer.AuthModule.SL.Services.AuthException ex)
                {
                    return Results.Json(
                        new { errorCode = ex.Code, message = ex.Message },
                        statusCode: ex.StatusCode
                    );
                }
            }
        );
        group.MapPost(
            "/logout",
            async (RefreshRequest request, IMediator mediator, CancellationToken ct) =>
                Results.Ok(await mediator.Send(new LogoutCommand(request.RefreshToken), ct))
        );
        group.MapPost(
            "/verify-email/resend",
            async (ResendVerificationRequest request, IMediator mediator, CancellationToken ct) =>
                Results.Ok(await mediator.Send(new ResendVerificationCommand(request), ct))
        );
        endpoints.MapGet(
            "/api/v1/bootstrap",
            (HttpRequest request, IRegistrationService service) =>
                Results.Ok(
                    service.CalculateBootstrap(
                        request.Query["source"].ToString(),
                        request.Query["selectedTemplateId"].ToString()
                    )
                )
        );
        return endpoints;
    }
}
