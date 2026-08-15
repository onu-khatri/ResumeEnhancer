using FluentValidation;
using ResumeEnhancer.ResumeModule.AM.Requests;
using ResumeEnhancer.ResumeModule.Web.Validation.Shared;

namespace ResumeEnhancer.ResumeModule.Web.Validation.Sections;

public sealed class AwardRequestValidator : AbstractValidator<AwardRequest>
{
    public AwardRequestValidator(bool isCreate = false)
    {
        this.RuleForRequestId(request => request.Id, isCreate);

        RuleFor(request => request.AwardName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.IssuingOrganization)
            .MaximumLength(200);

        RuleFor(request => request.Description)
            .MaximumLength(1000);
    }
}

