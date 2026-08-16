using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ResumeEnhancer.Core.WebLibrary.Endpoints;
using ResumeEnhancer.ProfilingModule.AM.Requests;
using ResumeEnhancer.ProfilingModule.SL.Contracts;

namespace ResumeEnhancer.ProfilingModule.Web.MiniApis;

internal static class AccessProfileApis
{
    public static IEndpointRouteBuilder MapAccessProfileApis(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/profiling/access-profiles").WithTags("Profiling Access Profiles");

        group.MapGet("/", async (IMediator mediator, CancellationToken cancellationToken) =>
                Results.Ok(await mediator.Send(new ListAccessProfilesQuery(), cancellationToken)))
            .WithName("ListAccessProfiles");

        group.MapGet("/{accessProfileId:int}", async (int accessProfileId, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(new GetAccessProfileQuery(accessProfileId), cancellationToken);
                return response is null ? Results.NotFound() : Results.Ok(response);
            })
            .WithName("GetAccessProfile");

        group.MapPost("/", async (CreateAccessProfileRequest? request, IValidator<CreateAccessProfileRequest> validator, IMediator mediator, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            if (request is null)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["body"] = ["Request body is required."] });
            }

            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            return await ApiEndpointExecutor.ValidateOrExecute(validationResult.ToDictionary(), async () =>
            {
                var response = await mediator.Send(new CreateAccessProfileCommand(request, ProfilingEndpointHeaders.ReadAuditUserId(httpContext)), cancellationToken);
                return Results.Created($"/api/profiling/access-profiles/{response.Id}", response);
            });
        }).WithName("CreateAccessProfile");

        group.MapPut("/{accessProfileId:int}", async (int accessProfileId, UpdateAccessProfileRequest? request, IValidator<UpdateAccessProfileRequest> validator, IMediator mediator, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            if (request is null)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["body"] = ["Request body is required."] });
            }

            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            return await ApiEndpointExecutor.ValidateOrExecute(validationResult.ToDictionary(), async () =>
            {
                var response = await mediator.Send(new UpdateAccessProfileCommand(accessProfileId, request, ProfilingEndpointHeaders.ReadAuditUserId(httpContext)), cancellationToken);
                return response is null ? Results.NotFound() : Results.Ok(response);
            });
        }).WithName("UpdateAccessProfile");

        group.MapDelete("/{accessProfileId:int}", async (int accessProfileId, IMediator mediator, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            var deleted = await mediator.Send(new DeleteAccessProfileCommand(accessProfileId, ProfilingEndpointHeaders.ReadAuditUserId(httpContext)), cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        }).WithName("DeleteAccessProfile");

        return endpoints;
    }
}
