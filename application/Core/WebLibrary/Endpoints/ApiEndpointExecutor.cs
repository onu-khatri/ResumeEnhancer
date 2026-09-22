using Microsoft.AspNetCore.Http;

namespace ResumeEnhancer.Core.WebLibrary.Endpoints;

public static class ApiEndpointExecutor
{
    public static Task<IResult> ValidateOrExecute(
        IDictionary<string, string[]> validationErrors,
        Func<Task<IResult>> action) =>
        validationErrors.Count > 0
            ? Task.FromResult(Results.ValidationProblem(validationErrors))
            : ExecuteAsync(action);

    public static async Task<IResult> ExecuteAsync(Func<Task<IResult>> action)
    {
        try
        {
            return await action();
        }
        catch (KeyNotFoundException)
        {
            return Results.Problem(title: "Resource not found.", statusCode: StatusCodes.Status404NotFound, extensions: new Dictionary<string, object?> { ["code"] = "NOT_FOUND" });
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Problem(
                detail: "The request is not authorized.",
                statusCode: StatusCodes.Status403Forbidden);
        }
        catch (ArgumentException)
        {
            return Results.Problem(title: "The request is invalid.", statusCode: StatusCodes.Status400BadRequest, extensions: new Dictionary<string, object?> { ["code"] = "INVALID_REQUEST" });
        }
        catch (InvalidOperationException)
        {
            return Results.Problem(title: "The request could not be completed.", statusCode: StatusCodes.Status400BadRequest, extensions: new Dictionary<string, object?> { ["code"] = "INVALID_OPERATION" });
        }
    }
}

