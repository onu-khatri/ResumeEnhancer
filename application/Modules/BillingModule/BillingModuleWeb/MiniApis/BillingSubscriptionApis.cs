using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ResumeEnhancer.BillingModule.AM.Requests;
using ResumeEnhancer.BillingModule.SL.Contracts;
using ResumeEnhancer.Core.WebLibrary.Endpoints;

namespace ResumeEnhancer.BillingModule.Web.MiniApis;

internal static class BillingSubscriptionApis
{
    public static IEndpointRouteBuilder MapBillingSubscriptionApis(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/billing/subscriptions").WithTags("Billing Subscriptions");

        group.MapGet("/", async (IMediator mediator, CancellationToken cancellationToken) =>
                Results.Ok(await mediator.Send(new ListBillingSubscriptionsQuery(), cancellationToken)))
            .WithName("ListBillingSubscriptions");

        group.MapGet("/{billingSubscriptionId:int}", async (int billingSubscriptionId, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(new GetBillingSubscriptionQuery(billingSubscriptionId), cancellationToken);
                return response is null ? Results.NotFound() : Results.Ok(response);
            })
            .WithName("GetBillingSubscription");

        group.MapPost("/", async (CreateBillingSubscriptionRequest? request, IValidator<CreateBillingSubscriptionRequest> validator, IMediator mediator, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            if (request is null)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["body"] = ["Request body is required."] });
            }

            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            return await ApiEndpointExecutor.ValidateOrExecute(validationResult.ToDictionary(), async () =>
            {
                var response = await mediator.Send(new CreateBillingSubscriptionCommand(request, BillingEndpointHeaders.ReadAuditUserId(httpContext)), cancellationToken);
                return response is null
                    ? Results.ValidationProblem(new Dictionary<string, string[]> { ["resumeId"] = ["Linked resume was not found."] })
                    : Results.Created($"/api/billing/subscriptions/{response.Id}", response);
            });
        }).WithName("CreateBillingSubscription");

        group.MapPut("/{billingSubscriptionId:int}", async (int billingSubscriptionId, UpdateBillingSubscriptionRequest? request, IValidator<UpdateBillingSubscriptionRequest> validator, IMediator mediator, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            if (request is null)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["body"] = ["Request body is required."] });
            }

            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            return await ApiEndpointExecutor.ValidateOrExecute(validationResult.ToDictionary(), async () =>
            {
                var response = await mediator.Send(new UpdateBillingSubscriptionCommand(billingSubscriptionId, request, BillingEndpointHeaders.ReadAuditUserId(httpContext)), cancellationToken);
                return response is null
                    ? Results.NotFound()
                    : Results.Ok(response);
            });
        }).WithName("UpdateBillingSubscription");

        group.MapDelete("/{billingSubscriptionId:int}", async (int billingSubscriptionId, IMediator mediator, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            var deleted = await mediator.Send(new DeleteBillingSubscriptionCommand(billingSubscriptionId, BillingEndpointHeaders.ReadAuditUserId(httpContext)), cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        }).WithName("DeleteBillingSubscription");

        return endpoints;
    }
}
