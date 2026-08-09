using FluentValidation;
using ResumeModuleAM.Requests;
using ResumeModuleWeb.Validation.Shared;

namespace ResumeModuleWeb.Validation.Sections;

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
