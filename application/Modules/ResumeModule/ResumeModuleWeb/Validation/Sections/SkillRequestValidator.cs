using FluentValidation;
using ResumeEnhancer.ResumeModule.AM.Requests;
using ResumeEnhancer.ResumeModule.Web.Validation.Shared;

namespace ResumeEnhancer.ResumeModule.Web.Validation.Sections;

public sealed class SkillRequestValidator : AbstractValidator<SkillRequest>
{
    public SkillRequestValidator(bool isCreate = false)
    {
        this.RuleForRequestId(request => request.Id, isCreate);

        RuleFor(request => request.SkillName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.ProficiencyLevel)
            .MaximumLength(100);

        RuleFor(request => request.YearsOfExperience)
            .GreaterThanOrEqualTo(0m)
            .When(request => request.YearsOfExperience.HasValue);

        RuleFor(request => request.Description)
            .MaximumLength(500);
    }
}

