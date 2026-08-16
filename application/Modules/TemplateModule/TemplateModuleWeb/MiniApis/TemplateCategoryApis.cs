using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ResumeEnhancer.Core.WebLibrary.Endpoints;
using ResumeEnhancer.TemplateModule.AM.Requests;
using ResumeEnhancer.TemplateModule.SL.Contracts;

namespace ResumeEnhancer.TemplateModule.Web.MiniApis;

internal static class TemplateCategoryApis
{
    public static IEndpointRouteBuilder MapTemplateCategoryApis(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/templates/categories").WithTags("Template Categories");

        group.MapGet("/", async (IMediator mediator, CancellationToken cancellationToken) =>
                Results.Ok(await mediator.Send(new ListTemplateCategoriesQuery(), cancellationToken)))
            .WithName("ListTemplateCategories");

        group.MapGet("/{templateCategoryId:int}", async (int templateCategoryId, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(new GetTemplateCategoryQuery(templateCategoryId), cancellationToken);
                return response is null ? Results.NotFound() : Results.Ok(response);
            })
            .WithName("GetTemplateCategory");

        group.MapPost("/", async (CreateTemplateCategoryRequest? request, IValidator<CreateTemplateCategoryRequest> validator, IMediator mediator, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            if (request is null)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["body"] = ["Request body is required."] });
            }

            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            return await ApiEndpointExecutor.ValidateOrExecute(validationResult.ToDictionary(), async () =>
            {
                var response = await mediator.Send(new CreateTemplateCategoryCommand(request, TemplateEndpointHeaders.ReadAuditUserId(httpContext)), cancellationToken);
                return Results.Created($"/api/templates/categories/{response.Id}", response);
            });
        }).WithName("CreateTemplateCategory");

        group.MapPut("/{templateCategoryId:int}", async (int templateCategoryId, UpdateTemplateCategoryRequest? request, IValidator<UpdateTemplateCategoryRequest> validator, IMediator mediator, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            if (request is null)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["body"] = ["Request body is required."] });
            }

            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            return await ApiEndpointExecutor.ValidateOrExecute(validationResult.ToDictionary(), async () =>
            {
                var response = await mediator.Send(new UpdateTemplateCategoryCommand(templateCategoryId, request, TemplateEndpointHeaders.ReadAuditUserId(httpContext)), cancellationToken);
                return response is null ? Results.NotFound() : Results.Ok(response);
            });
        }).WithName("UpdateTemplateCategory");

        group.MapDelete("/{templateCategoryId:int}", async (int templateCategoryId, IMediator mediator, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            var deleted = await mediator.Send(new DeleteTemplateCategoryCommand(templateCategoryId, TemplateEndpointHeaders.ReadAuditUserId(httpContext)), cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        }).WithName("DeleteTemplateCategory");

        return endpoints;
    }
}
