using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ResumeEnhancer.BillingModule.AM.Requests;
using ResumeEnhancer.BillingModule.SL.Contracts;
using ResumeEnhancer.Core.WebLibrary.Endpoints;

namespace ResumeEnhancer.BillingModule.Web.MiniApis;

internal static class BillingAccountApis
{
    public static IEndpointRouteBuilder MapBillingAccountApis(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/billing/accounts").WithTags("Billing Accounts");

        group.MapGet("/", async (IMediator mediator, CancellationToken cancellationToken) =>
                Results.Ok(await mediator.Send(new ListBillingAccountsQuery(), cancellationToken)))
            .WithName("ListBillingAccounts");

        group.MapGet("/{billingAccountId:int}", async (int billingAccountId, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(new GetBillingAccountQuery(billingAccountId), cancellationToken);
                return response is null ? Results.NotFound() : Results.Ok(response);
            })
            .WithName("GetBillingAccount");

        group.MapPost("/", async (CreateBillingAccountRequest? request, IValidator<CreateBillingAccountRequest> validator, IMediator mediator, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            if (request is null)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["body"] = ["Request body is required."] });
            }

            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            return await ApiEndpointExecutor.ValidateOrExecute(validationResult.ToDictionary(), async () =>
            {
                var response = await mediator.Send(new CreateBillingAccountCommand(request, BillingEndpointHeaders.ReadAuditUserId(httpContext)), cancellationToken);
                return Results.Created($"/api/billing/accounts/{response.Id}", response);
            });
        }).WithName("CreateBillingAccount");

        group.MapPut("/{billingAccountId:int}", async (int billingAccountId, UpdateBillingAccountRequest? request, IValidator<UpdateBillingAccountRequest> validator, IMediator mediator, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            if (request is null)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["body"] = ["Request body is required."] });
            }

            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            return await ApiEndpointExecutor.ValidateOrExecute(validationResult.ToDictionary(), async () =>
            {
                var response = await mediator.Send(new UpdateBillingAccountCommand(billingAccountId, request, BillingEndpointHeaders.ReadAuditUserId(httpContext)), cancellationToken);
                return response is null ? Results.NotFound() : Results.Ok(response);
            });
        }).WithName("UpdateBillingAccount");

        group.MapDelete("/{billingAccountId:int}", async (int billingAccountId, IMediator mediator, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            var deleted = await mediator.Send(new DeleteBillingAccountCommand(billingAccountId, BillingEndpointHeaders.ReadAuditUserId(httpContext)), cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        }).WithName("DeleteBillingAccount");

        return endpoints;
    }
}
