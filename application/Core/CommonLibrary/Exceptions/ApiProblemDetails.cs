using Microsoft.AspNetCore.Http;

namespace ResumeEnhancer.Core.CommonLibrary.Exceptions;

public static class ApiProblemDetails
{
    public static IResult Create(HttpContext context, string code, int statusCode, TimeSpan? retryAfter = null)
    {
        var detail = code switch
        {
            "AUTH_INVALID_CREDENTIALS" => "The credentials are invalid.",
            "AUTH_RATE_LIMITED" => "Too many authentication attempts.",
            "AUTH_CSRF_INVALID" or "AUTH_REFRESH_TRANSPORT_AMBIGUOUS" => "The request could not be verified.",
            "AUTH_CONFIGURATION_UNAVAILABLE" => "Authentication is temporarily unavailable.",
            "AUTH_REFRESH_REPLAYED" => "The authentication session is no longer valid.",
            "AUTH_ACCOUNT_UNAVAILABLE" or "AUTH_FORBIDDEN" => "Access denied.",
            _ => "The authentication request could not be completed.",
        };
        var extensions = new Dictionary<string, object?>
        {
            ["code"] = code,
            ["correlationId"] = context.TraceIdentifier,
        };
        if (retryAfter is not null)
            extensions["retryAfterSeconds"] = Math.Max(0, (int)Math.Ceiling(retryAfter.Value.TotalSeconds));
        return Results.Problem(
            statusCode: statusCode,
            title: statusCode switch
            {
                401 => "Authentication failed.",
                403 => "Access denied.",
                429 => "Too many requests.",
                _ when statusCode >= 500 => "Authentication service unavailable.",
                _ => "Authentication request failed.",
            },
            detail: detail,
            type: $"https://resumeenhancer.dev/problems/{code.ToLowerInvariant()}",
            instance: context.Request.Path,
            extensions: extensions);
    }

    public static IResult Validation(HttpContext context, IDictionary<string, string[]> errors) =>
        Results.ValidationProblem(errors, extensions: new Dictionary<string, object?>
        {
            ["code"] = "VALIDATION_FAILED",
            ["correlationId"] = context.TraceIdentifier,
        });

    public static async Task WriteAsync(HttpContext context, string code, int statusCode)
    {
        await Create(context, code, statusCode).ExecuteAsync(context);
    }
}
