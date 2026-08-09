using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Http;
using ResumeModuleAM.Requests;
using ResumeModuleSL.Contracts;
using ResumeModuleWeb.Validation.Shared;
using WebLibrary.Endpoints;

namespace ResumeModuleWeb.MiniApis.Commands;

internal static partial class ResumeCommandEndpoints
{
    private static Task<IResult> DeleteResumesAsync(
        DeleteResumesRequest? request,
        IValidator<DeleteResumesRequest> validator,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return Task.FromResult(Results.ValidationProblem(ResumeEndpointValidation.BodyRequired()));
        }

        return ValidateAndDeleteAsync();

        async Task<IResult> ValidateAndDeleteAsync()
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            return await ApiEndpointExecutor.ValidateOrExecute(
                validationResult.ToDictionary(),
                async () => Results.Ok(await mediator.Send(
                    new DeleteResumesCommand(
                        request.ResumeIds,
                        ResumeEndpointHeaders.ReadAuditUserId(httpContext),
                        ResumeEndpointHeaders.ReadUserId(httpContext)),
                    cancellationToken)));
        }
    }
}
