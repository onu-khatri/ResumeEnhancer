using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ResumeEnhancer.Core.WebLibrary.Endpoints;
using ResumeEnhancer.Core.WebLibrary.Authorization;
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
            .WithName("ListTemplateCategories")
            .AllowGuest("Guest", "TemplateView");

        group.MapGet("/{templateCategoryId:int}", async (int templateCategoryId, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(new GetTemplateCategoryQuery(templateCategoryId), cancellationToken);
                return response is null ? Results.NotFound() : Results.Ok(response);
            })
            .WithName("GetTemplateCategory")
            .AllowGuest("Guest", "TemplateView");

        group.MapPost("/", async (CreateTemplateCategoryRequest? request, IValidator<CreateTemplateCategoryRequest> validator, IMediator mediator, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            if (request is null)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["body"] = ["Request body is required."] });
            }

            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            return await ApiEndpointExecutor.ValidateOrExecute(validationResult.ToDictionary(), async () =>
            {
                var response = await mediator.Send(new CreateTemplateCategoryCommand(request, TemplateEndpointHeaders.GetPrincipalAuditUserId(httpContext)), cancellationToken);
                return Results.Created($"/api/templates/categories/{response.Id}", response);
            });
        }).WithName("CreateTemplateCategory").RequireProtectedAccess();

        group.MapPut("/{templateCategoryId:int}", async (int templateCategoryId, UpdateTemplateCategoryRequest? request, IValidator<UpdateTemplateCategoryRequest> validator, IMediator mediator, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            if (request is null)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["body"] = ["Request body is required."] });
            }

            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            return await ApiEndpointExecutor.ValidateOrExecute(validationResult.ToDictionary(), async () =>
            {
                var response = await mediator.Send(new UpdateTemplateCategoryCommand(templateCategoryId, request, TemplateEndpointHeaders.GetPrincipalAuditUserId(httpContext)), cancellationToken);
                return response is null ? Results.NotFound() : Results.Ok(response);
            });
        }).WithName("UpdateTemplateCategory").RequireProtectedAccess();

        group.MapDelete("/{templateCategoryId:int}", async (int templateCategoryId, IMediator mediator, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            var deleted = await mediator.Send(new DeleteTemplateCategoryCommand(templateCategoryId, TemplateEndpointHeaders.GetPrincipalAuditUserId(httpContext)), cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        }).WithName("DeleteTemplateCategory").RequireProtectedAccess();

        return endpoints;
    }
}
