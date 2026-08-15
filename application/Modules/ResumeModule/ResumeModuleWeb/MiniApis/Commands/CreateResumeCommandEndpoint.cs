using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Http;
using ResumeEnhancer.ResumeModule.AM.Requests;
using ResumeEnhancer.ResumeModule.SL.Contracts;
using ResumeEnhancer.ResumeModule.Web.Validation.Shared;
using ResumeEnhancer.Core.WebLibrary.Endpoints;

namespace ResumeEnhancer.ResumeModule.Web.MiniApis.Commands;

internal static partial class ResumeCommandEndpoints
{
    private static Task<IResult> CreateResumeAsync(
        CreateResumeRequest? request,
        IValidator<CreateResumeRequest> validator,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return Task.FromResult(Results.ValidationProblem(ResumeEndpointValidation.BodyRequired()));
        }

        return ValidateAndCreateAsync();

        async Task<IResult> ValidateAndCreateAsync()
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            return await ApiEndpointExecutor.ValidateOrExecute(
                validationResult.ToDictionary(),
                async () =>
                {
                    var response = await mediator.Send(
                        new CreateResumeCommand(
                            request,
                            ResumeEndpointHeaders.ReadAuditUserId(httpContext)),
                        cancellationToken);

                    return Results.Created($"/api/resumes/{response.Id}", response);
                });
        }
    }
}

