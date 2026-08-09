using Mediator;
using Microsoft.AspNetCore.Http;
using ResumeModuleSL.Contracts;
using ResumeModuleWeb.Validation.Shared;
using WebLibrary.Endpoints;

namespace ResumeModuleWeb.MiniApis.Commands;

internal static partial class ResumeCommandEndpoints
{
    private static Task<IResult> DeleteResumeAsync(
        int resumeId,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken cancellationToken) =>
        ApiEndpointExecutor.ValidateOrExecute(
            ResumeEndpointValidation.ResumeId(resumeId),
            async () => Results.Ok(await mediator.Send(
                new DeleteResumeCommand(
                    resumeId,
                    ResumeEndpointHeaders.ReadAuditUserId(httpContext),
                    ResumeEndpointHeaders.ReadUserId(httpContext)),
                cancellationToken)));
}
