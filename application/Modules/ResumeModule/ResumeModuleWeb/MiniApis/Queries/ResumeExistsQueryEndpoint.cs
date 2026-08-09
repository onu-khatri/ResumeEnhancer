using Mediator;
using Microsoft.AspNetCore.Http;
using ResumeModuleSL.Contracts;
using ResumeModuleWeb.Validation.Shared;
using WebLibrary.Endpoints;

namespace ResumeModuleWeb.MiniApis.Queries;

internal static partial class ResumeQueryEndpoints
{
    private static Task<IResult> ResumeExistsAsync(
        int resumeId,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken cancellationToken) =>
        ApiEndpointExecutor.ValidateOrExecute(
            ResumeEndpointValidation.ResumeId(resumeId),
            async () => Results.Ok(await mediator.Send(
                new ResumeExistsQuery(
                    resumeId,
                    ResumeEndpointHeaders.ReadUserId(httpContext)),
                cancellationToken)));
}
