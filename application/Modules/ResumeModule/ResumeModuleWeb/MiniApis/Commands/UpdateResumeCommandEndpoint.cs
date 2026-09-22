using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Http;
using ResumeEnhancer.ResumeModule.AM.Requests;
using ResumeEnhancer.ResumeModule.SL.Contracts;
using ResumeEnhancer.ResumeModule.Web.Validation.Shared;
using ResumeEnhancer.Core.WebLibrary.Endpoints;
using ResumeEnhancer.Core.WebLibrary.Authorization;

namespace ResumeEnhancer.ResumeModule.Web.MiniApis.Commands;

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
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var subjectId = httpContext.User.GetSubjectId();
            if (subjectId is null || request.UserId is not null && request.UserId != subjectId)
            {
                return Results.StatusCode(StatusCodes.Status403Forbidden);
            }

            return await ApiEndpointExecutor.ValidateOrExecute(
                ResumeEndpointValidation.Merge(routeErrors, validationResult.ToDictionary()),
                async () => Results.Ok(await mediator.Send(
                    new UpdateResumeCommand(
                        resumeId,
                        request,
                        ResumeEndpointHeaders.GetPrincipalAuditUserId(httpContext),
                        ResumeEndpointHeaders.GetPrincipalUserId(httpContext)),
                    cancellationToken)));
        }
    }
}

