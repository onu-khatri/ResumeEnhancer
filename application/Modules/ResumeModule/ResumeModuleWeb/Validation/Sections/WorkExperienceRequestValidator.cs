using FluentValidation;
using ResumeModuleAM.Requests;
using ResumeModuleWeb.Validation.Shared;

namespace ResumeModuleWeb.Validation.Sections;

public sealed class WorkExperienceRequestValidator : AbstractValidator<WorkExperienceRequest>
{
    public WorkExperienceRequestValidator(bool isCreate = false)
    {
        this.RuleForRequestId(request => request.Id, isCreate);

        RuleFor(request => request.JobTitle)
            .MaximumLength(150);

        RuleFor(request => request.CompanyName)
            .MaximumLength(200);

        RuleFor(request => request.EndDate)
            .GreaterThanOrEqualTo(request => request.StartDate)
            .When(request => request.StartDate.HasValue && request.EndDate.HasValue)
            .WithMessage("Work experience start date cannot be later than end date.");

        RuleFor(request => request.Location)
            .MaximumLength(200);

        RuleFor(request => request.Description)
            .MaximumLength(1000);
    }
}
