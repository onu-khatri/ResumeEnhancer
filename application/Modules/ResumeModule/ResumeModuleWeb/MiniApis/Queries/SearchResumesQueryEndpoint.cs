using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Http;
using ResumeModuleAM.Requests;
using ResumeModuleSL.Contracts;
using ResumeModuleWeb.Validation.Shared;
using WebLibrary.Endpoints;

namespace ResumeModuleWeb.MiniApis.Queries;

internal static partial class ResumeQueryEndpoints
{
    private static Task<IResult> SearchResumesAsync(
        ResumeSearchRequest? request,
        IValidator<ResumeSearchRequest> validator,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return Task.FromResult(Results.ValidationProblem(ResumeEndpointValidation.BodyRequired()));
        }

        return ValidateAndSearchAsync();

        async Task<IResult> ValidateAndSearchAsync()
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            return await ApiEndpointExecutor.ValidateOrExecute(
                validationResult.ToDictionary(),
                async () => Results.Ok(await mediator.Send(
                    new SearchResumesQuery(request),
                    cancellationToken)));
        }
    }
}
