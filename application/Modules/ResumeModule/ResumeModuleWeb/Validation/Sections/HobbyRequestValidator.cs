using FluentValidation;
using ResumeModuleAM.Requests;
using ResumeModuleWeb.Validation.Shared;

namespace ResumeModuleWeb.Validation.Sections;

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
