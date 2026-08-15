using FluentValidation;
using ResumeEnhancer.ResumeModule.AM.Requests;
using ResumeEnhancer.ResumeModule.Web.Validation.Shared;

namespace ResumeEnhancer.ResumeModule.Web.Validation.Sections;

public sealed class HobbyRequestValidator : AbstractValidator<HobbyRequest>
{
    public HobbyRequestValidator(bool isCreate = false)
    {
        this.RuleForRequestId(request => request.Id, isCreate);

        RuleFor(request => request.HobbyName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.Description)
            .MaximumLength(500);
    }
}

