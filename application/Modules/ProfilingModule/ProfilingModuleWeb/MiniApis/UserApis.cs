using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ResumeEnhancer.Core.WebLibrary.Endpoints;
using ResumeEnhancer.ProfilingModule.AM.Requests;
using ResumeEnhancer.ProfilingModule.SL.Contracts;

namespace ResumeEnhancer.ProfilingModule.Web.MiniApis;

internal static class UserApis
{
    public static IEndpointRouteBuilder MapUserApis(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/profiling/users").WithTags("Profiling Users");

        group.MapGet("/", async (IMediator mediator, CancellationToken cancellationToken) =>
                Results.Ok(await mediator.Send(new ListUsersQuery(), cancellationToken)))
            .WithName("ListUsers");

        group.MapGet("/{userId:int}", async (int userId, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(new GetUserQuery(userId), cancellationToken);
                return response is null ? Results.NotFound() : Results.Ok(response);
            })
            .WithName("GetUser");

        group.MapPost("/", async (CreateUserRequest? request, IValidator<CreateUserRequest> validator, IMediator mediator, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            if (request is null)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["body"] = ["Request body is required."] });
            }

            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            return await ApiEndpointExecutor.ValidateOrExecute(validationResult.ToDictionary(), async () =>
            {
                var response = await mediator.Send(new CreateUserCommand(request, ProfilingEndpointHeaders.ReadAuditUserId(httpContext)), cancellationToken);
                return Results.Created($"/api/profiling/users/{response.Id}", response);
            });
        }).WithName("CreateUser");

        group.MapPut("/{userId:int}", async (int userId, UpdateUserRequest? request, IValidator<UpdateUserRequest> validator, IMediator mediator, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            if (request is null)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["body"] = ["Request body is required."] });
            }

            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            return await ApiEndpointExecutor.ValidateOrExecute(validationResult.ToDictionary(), async () =>
            {
                var response = await mediator.Send(new UpdateUserCommand(userId, request, ProfilingEndpointHeaders.ReadAuditUserId(httpContext)), cancellationToken);
                return response is null ? Results.NotFound() : Results.Ok(response);
            });
        }).WithName("UpdateUser");

        group.MapDelete("/{userId:int}", async (int userId, IMediator mediator, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            var deleted = await mediator.Send(new DeleteUserCommand(userId, ProfilingEndpointHeaders.ReadAuditUserId(httpContext)), cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        }).WithName("DeleteUser");

        return endpoints;
    }
}
