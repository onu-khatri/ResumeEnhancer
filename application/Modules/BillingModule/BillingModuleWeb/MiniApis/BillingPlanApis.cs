using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ResumeEnhancer.BillingModule.AM.Requests;
using ResumeEnhancer.BillingModule.SL.Contracts;
using ResumeEnhancer.Core.WebLibrary.Endpoints;

namespace ResumeEnhancer.BillingModule.Web.MiniApis;

internal static class BillingPlanApis
{
    public static IEndpointRouteBuilder MapBillingPlanApis(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/billing/plans").WithTags("Billing Plans");

        group.MapGet("/", async (IMediator mediator, CancellationToken cancellationToken) =>
                Results.Ok(await mediator.Send(new ListBillingPlansQuery(), cancellationToken)))
            .WithName("ListBillingPlans");

        group.MapGet("/{billingPlanId:int}", async (int billingPlanId, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(new GetBillingPlanQuery(billingPlanId), cancellationToken);
                return response is null ? Results.NotFound() : Results.Ok(response);
            })
            .WithName("GetBillingPlan");

        group.MapPost("/", async (CreateBillingPlanRequest? request, IValidator<CreateBillingPlanRequest> validator, IMediator mediator, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            if (request is null)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["body"] = ["Request body is required."] });
            }

            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            return await ApiEndpointExecutor.ValidateOrExecute(validationResult.ToDictionary(), async () =>
            {
                var response = await mediator.Send(new CreateBillingPlanCommand(request, BillingEndpointHeaders.ReadAuditUserId(httpContext)), cancellationToken);
                return Results.Created($"/api/billing/plans/{response.Id}", response);
            });
        }).WithName("CreateBillingPlan");

        group.MapPut("/{billingPlanId:int}", async (int billingPlanId, UpdateBillingPlanRequest? request, IValidator<UpdateBillingPlanRequest> validator, IMediator mediator, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            if (request is null)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["body"] = ["Request body is required."] });
            }

            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            return await ApiEndpointExecutor.ValidateOrExecute(validationResult.ToDictionary(), async () =>
            {
                var response = await mediator.Send(new UpdateBillingPlanCommand(billingPlanId, request, BillingEndpointHeaders.ReadAuditUserId(httpContext)), cancellationToken);
                return response is null ? Results.NotFound() : Results.Ok(response);
            });
        }).WithName("UpdateBillingPlan");

        group.MapDelete("/{billingPlanId:int}", async (int billingPlanId, IMediator mediator, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            var deleted = await mediator.Send(new DeleteBillingPlanCommand(billingPlanId, BillingEndpointHeaders.ReadAuditUserId(httpContext)), cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        }).WithName("DeleteBillingPlan");

        return endpoints;
    }
}
