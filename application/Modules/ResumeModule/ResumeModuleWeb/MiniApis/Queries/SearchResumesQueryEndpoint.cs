using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Http;
using ResumeEnhancer.ResumeModule.AM.Requests;
using ResumeEnhancer.ResumeModule.SL.Contracts;
using ResumeEnhancer.ResumeModule.Web.Validation.Shared;
using ResumeEnhancer.Core.WebLibrary.Endpoints;
using ResumeEnhancer.Core.WebLibrary.Authorization;

namespace ResumeEnhancer.ResumeModule.Web.MiniApis.Queries;

internal static partial class ResumeQueryEndpoints
{
    private static Task<IResult> SearchResumesAsync(
        ResumeSearchRequest? request,
        IValidator<ResumeSearchRequest> validator,
        IMediator mediator,
        HttpContext httpContext,
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
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var subjectId = httpContext.User.GetSubjectId();
            if (subjectId is null || request.UserId is not null && request.UserId != subjectId)
            {
                return Results.StatusCode(StatusCodes.Status403Forbidden);
            }

            request.UserId = subjectId;

            return await ApiEndpointExecutor.ValidateOrExecute(
                validationResult.ToDictionary(),
                async () => Results.Ok(await mediator.Send(
                    new SearchResumesQuery(request),
                    cancellationToken)));
        }
    }
}

