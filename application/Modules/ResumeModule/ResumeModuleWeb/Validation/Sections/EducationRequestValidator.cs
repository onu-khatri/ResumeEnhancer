using FluentValidation;
using ResumeEnhancer.ResumeModule.AM.Requests;
using ResumeEnhancer.ResumeModule.Web.Validation.Shared;

namespace ResumeEnhancer.ResumeModule.Web.Validation.Sections;

public sealed class EducationRequestValidator : AbstractValidator<EducationRequest>
{
    public EducationRequestValidator(bool isCreate = false)
    {
        this.RuleForRequestId(request => request.Id, isCreate);

        RuleFor(request => request.PassingYear)
            .InclusiveBetween(1900, DateTime.UtcNow.Year + 10)
            .When(request => request.PassingYear.HasValue)
            .WithMessage($"Passing year must be between 1900 and {DateTime.UtcNow.Year + 10}.");

        RuleFor(request => request.Degree)
            .MaximumLength(200);

        RuleFor(request => request.Institution)
            .MaximumLength(200);

        RuleFor(request => request.City)
            .MaximumLength(100);

        RuleFor(request => request.State)
            .MaximumLength(100);

        RuleFor(request => request.Description)
            .MaximumLength(1000);

        RuleFor(request => request.Percentage)
            .InclusiveBetween(0m, 100m)
            .When(request => request.Percentage.HasValue);

        RuleFor(request => request.Grade)
            .MaximumLength(50);
    }
}

