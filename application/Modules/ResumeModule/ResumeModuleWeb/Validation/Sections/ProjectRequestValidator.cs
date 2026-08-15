using FluentValidation;
using ResumeEnhancer.ResumeModule.AM.Requests;
using ResumeEnhancer.ResumeModule.Web.Validation.Shared;

namespace ResumeEnhancer.ResumeModule.Web.Validation.Sections;

public sealed class ProjectRequestValidator : AbstractValidator<ProjectRequest>
{
    public ProjectRequestValidator(bool isCreate = false)
    {
        this.RuleForRequestId(request => request.Id, isCreate);

        RuleFor(request => request.ProjectName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.Role)
            .MaximumLength(150);

        RuleFor(request => request.EndDate)
            .GreaterThanOrEqualTo(request => request.StartDate)
            .When(request => request.StartDate.HasValue && request.EndDate.HasValue)
            .WithMessage("Project start date cannot be later than end date.");

        RuleFor(request => request.Description)
            .MaximumLength(1000);

        RuleFor(request => request.TechnologiesUsed)
            .MaximumLength(500);
    }
}

