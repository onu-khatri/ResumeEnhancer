using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ResumeEnhancer.Core.CommonLibrary.Exceptions;
using ResumeEnhancer.Tests.Unit.TestInfrastructure;
using Shouldly;

namespace ResumeEnhancer.Tests.Unit.Core.CommonLibrary;

public sealed class ApiProblemDetailsTests
{
    [Theory]
    [InlineData("AUTH_INVALID_CREDENTIALS", "The credentials are invalid.")]
    [InlineData("AUTH_RATE_LIMITED", "Too many authentication attempts.")]
    [InlineData("AUTH_CSRF_INVALID", "The request could not be verified.")]
    [InlineData("AUTH_REFRESH_TRANSPORT_AMBIGUOUS", "The request could not be verified.")]
    [InlineData("AUTH_CONFIGURATION_UNAVAILABLE", "Authentication is temporarily unavailable.")]
    [InlineData("AUTH_REFRESH_REPLAYED", "The authentication session is no longer valid.")]
    [InlineData("AUTH_ACCOUNT_UNAVAILABLE", "Access denied.")]
    [InlineData("AUTH_FORBIDDEN", "Access denied.")]
    [InlineData("AUTH_UNEXPECTED", "The authentication request could not be completed.")]
    public async Task Create_writes_stable_safe_problem_contract_for_each_authentication_code(
        string code,
        string detail)
    {
        var context = CreateContext("/api/auth", "trace-123");

        var snapshot = await ApiProblemDetails.Create(context, code, StatusCodes.Status401Unauthorized)
            .ExecuteAsync();
        var json = snapshot.ReadJson();

        snapshot.StatusCode.ShouldBe(StatusCodes.Status401Unauthorized);
        json.GetProperty("title").GetString().ShouldBe("Authentication failed.");
        json.GetProperty("detail").GetString().ShouldBe(detail);
        json.GetProperty("type").GetString().ShouldBe($"https://resumeenhancer.dev/problems/{code.ToLowerInvariant()}");
        json.GetProperty("instance").GetString().ShouldBe("/api/auth");
        json.GetProperty("code").GetString().ShouldBe(code);
        json.GetProperty("correlationId").GetString().ShouldBe("trace-123");
        json.TryGetProperty("retryAfterSeconds", out _).ShouldBeFalse();
    }

    [Theory]
    [InlineData(403, "Access denied.")]
    [InlineData(429, "Too many requests.")]
    [InlineData(500, "Authentication service unavailable.")]
    [InlineData(400, "Authentication request failed.")]
    public async Task Create_selects_status_title_and_clamped_retry_after(int statusCode, string title)
    {
        var context = CreateContext("/api/auth/refresh", "trace-429");

        var snapshot = await ApiProblemDetails.Create(
                context,
                "AUTH_RATE_LIMITED",
                statusCode,
                TimeSpan.FromMilliseconds(-1))
            .ExecuteAsync();
        var json = snapshot.ReadJson();

        json.GetProperty("title").GetString().ShouldBe(title);
        json.GetProperty("retryAfterSeconds").GetInt32().ShouldBe(0);

        var rounded = await ApiProblemDetails.Create(
                context,
                "AUTH_RATE_LIMITED",
                StatusCodes.Status429TooManyRequests,
                TimeSpan.FromSeconds(1.1))
            .ExecuteAsync();
        rounded.ReadJson().GetProperty("retryAfterSeconds").GetInt32().ShouldBe(2);
    }

    [Fact]
    public async Task Validation_and_write_helpers_preserve_correlation_and_problem_shape()
    {
        var context = CreateContext("/api/auth/register", "trace-validation");

        var validation = await ApiProblemDetails.Validation(
                context,
                new Dictionary<string, string[]> { ["Email"] = ["Email is invalid."] })
            .ExecuteAsync();
        var validationJson = validation.ReadJson();
        validation.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        validationJson.GetProperty("code").GetString().ShouldBe("VALIDATION_FAILED");
        validationJson.GetProperty("correlationId").GetString().ShouldBe("trace-validation");
        validationJson.GetProperty("errors").GetProperty("Email")[0].GetString().ShouldBe("Email is invalid.");

        await ApiProblemDetails.WriteAsync(context, "AUTH_FORBIDDEN", StatusCodes.Status403Forbidden);
        context.Response.StatusCode.ShouldBe(StatusCodes.Status403Forbidden);
        context.Response.Body.Position = 0;
        using var document = await JsonDocument.ParseAsync(context.Response.Body);
        document.RootElement.GetProperty("code").GetString().ShouldBe("AUTH_FORBIDDEN");
    }

    private static DefaultHttpContext CreateContext(string path, string traceIdentifier)
    {
        var context = new DefaultHttpContext
        {
            RequestServices = new ServiceCollection()
                .AddLogging()
                .AddProblemDetails()
                .BuildServiceProvider(),
        };
        context.Request.Path = path;
        context.TraceIdentifier = traceIdentifier;
        context.Response.Body = new MemoryStream();
        return context;
    }
}
