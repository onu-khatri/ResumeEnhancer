using FluentValidation;
using ResumeModuleAM.Requests;
using ResumeModuleWeb.Validation.Shared;

namespace ResumeModuleWeb.Validation.Sections;

public sealed class SocialMediaLinkRequestValidator : AbstractValidator<SocialMediaLinkRequest>
{
    public SocialMediaLinkRequestValidator(bool isCreate = false)
    {
        this.RuleForRequestId(request => request.Id, isCreate);

        RuleFor(request => request.Platform)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.Url)
            .NotEmpty()
            .MaximumLength(500)
            .Must(ResumeValidationRules.IsValidHttpUrl)
            .WithMessage("Url must be a valid http or https URL.");

        RuleFor(request => request.DisplayName)
            .MaximumLength(100);
    }
}
