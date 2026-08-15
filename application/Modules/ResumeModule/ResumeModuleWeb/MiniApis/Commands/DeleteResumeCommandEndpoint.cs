using Mediator;
using Microsoft.AspNetCore.Http;
using ResumeEnhancer.ResumeModule.SL.Contracts;
using ResumeEnhancer.ResumeModule.Web.Validation.Shared;
using ResumeEnhancer.Core.WebLibrary.Endpoints;

namespace ResumeEnhancer.ResumeModule.Web.MiniApis.Commands;

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

