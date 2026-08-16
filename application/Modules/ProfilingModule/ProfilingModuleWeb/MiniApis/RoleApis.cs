using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ResumeEnhancer.Core.WebLibrary.Endpoints;
using ResumeEnhancer.ProfilingModule.AM.Requests;
using ResumeEnhancer.ProfilingModule.SL.Contracts;

namespace ResumeEnhancer.ProfilingModule.Web.MiniApis;

internal static class RoleApis
{
    public static IEndpointRouteBuilder MapRoleApis(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/profiling/roles").WithTags("Profiling Roles");

        group.MapGet("/", async (IMediator mediator, CancellationToken cancellationToken) =>
                Results.Ok(await mediator.Send(new ListRolesQuery(), cancellationToken)))
            .WithName("ListRoles");

        group.MapGet("/{roleId:int}", async (int roleId, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(new GetRoleQuery(roleId), cancellationToken);
                return response is null ? Results.NotFound() : Results.Ok(response);
            })
            .WithName("GetRole");

        group.MapPost("/", async (CreateRoleRequest? request, IValidator<CreateRoleRequest> validator, IMediator mediator, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            if (request is null)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["body"] = ["Request body is required."] });
            }

            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            return await ApiEndpointExecutor.ValidateOrExecute(validationResult.ToDictionary(), async () =>
            {
                var response = await mediator.Send(new CreateRoleCommand(request, ProfilingEndpointHeaders.ReadAuditUserId(httpContext)), cancellationToken);
                return Results.Created($"/api/profiling/roles/{response.Id}", response);
            });
        }).WithName("CreateRole");

        group.MapPut("/{roleId:int}", async (int roleId, UpdateRoleRequest? request, IValidator<UpdateRoleRequest> validator, IMediator mediator, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            if (request is null)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["body"] = ["Request body is required."] });
            }

            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            return await ApiEndpointExecutor.ValidateOrExecute(validationResult.ToDictionary(), async () =>
            {
                var response = await mediator.Send(new UpdateRoleCommand(roleId, request, ProfilingEndpointHeaders.ReadAuditUserId(httpContext)), cancellationToken);
                return response is null ? Results.NotFound() : Results.Ok(response);
            });
        }).WithName("UpdateRole");

        group.MapDelete("/{roleId:int}", async (int roleId, IMediator mediator, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            var deleted = await mediator.Send(new DeleteRoleCommand(roleId, ProfilingEndpointHeaders.ReadAuditUserId(httpContext)), cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        }).WithName("DeleteRole");

        return endpoints;
    }
}
