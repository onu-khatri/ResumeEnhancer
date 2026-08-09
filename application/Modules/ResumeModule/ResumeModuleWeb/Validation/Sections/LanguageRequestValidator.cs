using FluentValidation;
using ResumeModuleAM.Requests;
using ResumeModuleWeb.Validation.Shared;

namespace ResumeModuleWeb.Validation.Sections;

public sealed class LanguageRequestValidator : AbstractValidator<LanguageRequest>
{
    public LanguageRequestValidator(bool isCreate = false)
    {
        this.RuleForRequestId(request => request.Id, isCreate);

        RuleFor(request => request.LanguageName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.ProficiencyLevel)
            .MaximumLength(100);

        RuleFor(request => request.Description)
            .MaximumLength(500);
    }
}
