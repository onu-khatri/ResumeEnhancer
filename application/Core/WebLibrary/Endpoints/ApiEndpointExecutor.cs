using Microsoft.AspNetCore.Http;

namespace WebLibrary.Endpoints;

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
        catch (KeyNotFoundException exception)
        {
            return Results.NotFound(new { error = exception.Message });
        }
        catch (UnauthorizedAccessException exception)
        {
            return Results.Problem(
                detail: exception.Message,
                statusCode: StatusCodes.Status403Forbidden);
        }
        catch (ArgumentException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }
    }
}
