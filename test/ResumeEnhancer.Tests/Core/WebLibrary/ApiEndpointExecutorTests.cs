using Microsoft.AspNetCore.Http;
using Shouldly;
using ResumeEnhancer.Tests.TestInfrastructure;
using WebLibrary.Endpoints;

namespace ResumeEnhancer.Tests.Core.WebLibrary;

public sealed class ApiEndpointExecutorTests
{
    [Fact]
    public async Task ValidateOrExecute_ValidationErrorsExist_ReturnsValidationProblem()
    {
        var errors = new Dictionary<string, string[]>
        {
            ["Title"] = ["Title is required."]
        };

        var result = await ApiEndpointExecutor.ValidateOrExecute(
            errors,
            () => Task.FromResult(Results.Ok()));
        var snapshot = await result.ExecuteAsync();

        snapshot.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        snapshot.Body.ShouldContain("Title is required.");
    }

    [Fact]
    public async Task ValidateOrExecute_NoValidationErrors_ExecutesAction()
    {
        var executed = false;

        var result = await ApiEndpointExecutor.ValidateOrExecute(
            new Dictionary<string, string[]>(),
            () =>
            {
                executed = true;
                return Task.FromResult(Results.Ok(new { ok = true }));
            });
        var snapshot = await result.ExecuteAsync();

        executed.ShouldBeTrue();
        snapshot.StatusCode.ShouldBe(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task ExecuteAsync_KeyNotFoundException_ReturnsNotFound()
    {
        var result = await ApiEndpointExecutor.ExecuteAsync(
            () => throw new KeyNotFoundException("missing"));
        var snapshot = await result.ExecuteAsync();

        snapshot.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
        snapshot.Body.ShouldContain("missing");
    }

    [Fact]
    public async Task ExecuteAsync_UnauthorizedAccessException_ReturnsForbiddenProblem()
    {
        var result = await ApiEndpointExecutor.ExecuteAsync(
            () => throw new UnauthorizedAccessException("denied"));
        var snapshot = await result.ExecuteAsync();

        snapshot.StatusCode.ShouldBe(StatusCodes.Status403Forbidden);
        snapshot.Body.ShouldContain("denied");
    }

    [Fact]
    public async Task ExecuteAsync_ArgumentException_ReturnsBadRequest()
    {
        var result = await ApiEndpointExecutor.ExecuteAsync(
            () => throw new ArgumentException("bad input"));
        var snapshot = await result.ExecuteAsync();

        snapshot.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        snapshot.Body.ShouldContain("bad input");
    }

    [Fact]
    public async Task ExecuteAsync_InvalidOperationException_ReturnsBadRequest()
    {
        var result = await ApiEndpointExecutor.ExecuteAsync(
            () => throw new InvalidOperationException("bad state"));
        var snapshot = await result.ExecuteAsync();

        snapshot.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        snapshot.Body.ShouldContain("bad state");
    }
}
