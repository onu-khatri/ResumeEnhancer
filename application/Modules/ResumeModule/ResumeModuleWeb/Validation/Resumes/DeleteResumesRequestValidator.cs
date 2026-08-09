using FluentValidation;
using ResumeModuleAM.Requests;

namespace ResumeModuleWeb.Validation.Resumes;

public sealed class DeleteResumesRequestValidator : AbstractValidator<DeleteResumesRequest>
{
    public DeleteResumesRequestValidator()
    {
        RuleFor(request => request.ResumeIds)
            .NotNull();

        RuleForEach(request => request.ResumeIds)
            .GreaterThan(0)
            .WithMessage("ResumeIds cannot contain zero or negative ids.");
    }
}
