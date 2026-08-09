using FluentValidation;
using ResumeModuleAM.Requests;

namespace ResumeModuleWeb.Validation.Resumes;

public sealed class ResumeSearchRequestValidator : AbstractValidator<ResumeSearchRequest>
{
    public ResumeSearchRequestValidator()
    {
        RuleForEach(request => request.Ids)
            .GreaterThan(0)
            .WithMessage("Ids cannot contain zero or negative ids.");

        RuleFor(request => request.UserId)
            .MaximumLength(450);

        RuleFor(request => request.SearchText)
            .MaximumLength(200);

        RuleFor(request => request.ResumeTemplate)
            .MaximumLength(100);

        RuleFor(request => request.CreatedToUtc)
            .GreaterThanOrEqualTo(request => request.CreatedFromUtc)
            .When(request => request.CreatedFromUtc.HasValue && request.CreatedToUtc.HasValue)
            .WithMessage("Created start date cannot be later than end date.");

        RuleFor(request => request.UpdatedToUtc)
            .GreaterThanOrEqualTo(request => request.UpdatedFromUtc)
            .When(request => request.UpdatedFromUtc.HasValue && request.UpdatedToUtc.HasValue)
            .WithMessage("Updated start date cannot be later than end date.");

        RuleFor(request => request.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(request => request.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(request => request.SortBy)
            .IsInEnum();

        RuleFor(request => request.SortDirection)
            .IsInEnum();
    }
}
