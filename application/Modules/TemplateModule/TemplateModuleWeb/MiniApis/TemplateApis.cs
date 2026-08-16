using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ResumeEnhancer.Core.WebLibrary.Endpoints;
using ResumeEnhancer.TemplateModule.AM.Requests;
using ResumeEnhancer.TemplateModule.SL.Contracts;

namespace ResumeEnhancer.TemplateModule.Web.MiniApis;

internal static class TemplateApis
{
    public static IEndpointRouteBuilder MapTemplateApis(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/templates").WithTags("Templates");

        group.MapGet("/", async (IMediator mediator, CancellationToken cancellationToken) =>
                Results.Ok(await mediator.Send(new ListTemplatesQuery(), cancellationToken)))
            .WithName("ListTemplates");

        group.MapGet("/{templateId:int}", async (int templateId, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(new GetTemplateQuery(templateId), cancellationToken);
                return response is null ? Results.NotFound() : Results.Ok(response);
            })
            .WithName("GetTemplate");

        group.MapPost("/", async (CreateTemplateRequest? request, IValidator<CreateTemplateRequest> validator, IMediator mediator, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            if (request is null)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["body"] = ["Request body is required."] });
            }

            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            return await ApiEndpointExecutor.ValidateOrExecute(validationResult.ToDictionary(), async () =>
            {
                var response = await mediator.Send(new CreateTemplateCommand(request, TemplateEndpointHeaders.ReadAuditUserId(httpContext)), cancellationToken);
                return response is null
                    ? Results.ValidationProblem(new Dictionary<string, string[]>
                    {
                        ["TemplateCategoryId"] = ["Template category was not found."],
                        ["RenderTypeCode"] = ["Template render type was not found."]
                    })
                    : Results.Created($"/api/templates/{response.Id}", response);
            });
        }).WithName("CreateTemplate");

        group.MapPut("/{templateId:int}", async (int templateId, UpdateTemplateRequest? request, IValidator<UpdateTemplateRequest> validator, IMediator mediator, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            if (request is null)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["body"] = ["Request body is required."] });
            }

            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            return await ApiEndpointExecutor.ValidateOrExecute(validationResult.ToDictionary(), async () =>
            {
                var response = await mediator.Send(new UpdateTemplateCommand(templateId, request, TemplateEndpointHeaders.ReadAuditUserId(httpContext)), cancellationToken);
                return response is null ? Results.NotFound() : Results.Ok(response);
            });
        }).WithName("UpdateTemplate");

        group.MapDelete("/{templateId:int}", async (int templateId, IMediator mediator, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            var deleted = await mediator.Send(new DeleteTemplateCommand(templateId, TemplateEndpointHeaders.ReadAuditUserId(httpContext)), cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        }).WithName("DeleteTemplate");

        return endpoints;
    }
}
