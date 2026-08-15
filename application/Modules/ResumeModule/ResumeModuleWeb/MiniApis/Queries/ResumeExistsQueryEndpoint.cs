using Mediator;
using Microsoft.AspNetCore.Http;
using ResumeEnhancer.ResumeModule.SL.Contracts;
using ResumeEnhancer.ResumeModule.Web.Validation.Shared;
using ResumeEnhancer.Core.WebLibrary.Endpoints;

namespace ResumeEnhancer.ResumeModule.Web.MiniApis.Queries;

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

