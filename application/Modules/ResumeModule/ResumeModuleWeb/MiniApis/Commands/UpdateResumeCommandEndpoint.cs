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
    private static Task<IResult> UpdateResumeAsync(
        int resumeId,
        UpdateResumeRequest? request,
        IValidator<UpdateResumeRequest> validator,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var routeErrors = ResumeEndpointValidation.ResumeId(resumeId);

        if (request is null)
        {
            return Task.FromResult(Results.ValidationProblem(
                ResumeEndpointValidation.Merge(routeErrors, ResumeEndpointValidation.BodyRequired())));
        }

        return ValidateAndUpdateAsync();

        async Task<IResult> ValidateAndUpdateAsync()
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            return await ApiEndpointExecutor.ValidateOrExecute(
                ResumeEndpointValidation.Merge(routeErrors, validationResult.ToDictionary()),
                async () => Results.Ok(await mediator.Send(
                    new UpdateResumeCommand(
                        resumeId,
                        request,
                        ResumeEndpointHeaders.ReadAuditUserId(httpContext),
                        ResumeEndpointHeaders.ReadUserId(httpContext)),
                    cancellationToken)));
        }
    }
}
