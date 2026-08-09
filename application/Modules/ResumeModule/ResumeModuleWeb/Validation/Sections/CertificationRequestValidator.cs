using FluentValidation;
using ResumeModuleAM.Requests;
using ResumeModuleWeb.Validation.Shared;

namespace ResumeModuleWeb.Validation.Sections;

public sealed class CertificationRequestValidator : AbstractValidator<CertificationRequest>
{
    public CertificationRequestValidator(bool isCreate = false)
    {
        this.RuleForRequestId(request => request.Id, isCreate);

        RuleFor(request => request.CertificationName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.IssuingOrganization)
            .MaximumLength(200);

        RuleFor(request => request.ExpirationDate)
            .GreaterThanOrEqualTo(request => request.IssueDate)
            .When(request => request.IssueDate.HasValue && request.ExpirationDate.HasValue)
            .WithMessage("Certification start date cannot be later than end date.");

        RuleFor(request => request.CredentialId)
            .MaximumLength(100);

        RuleFor(request => request.CredentialUrl)
            .MaximumLength(500)
            .Must(ResumeValidationRules.IsValidHttpUrl)
            .WithMessage("CredentialUrl must be a valid http or https URL.");

        RuleFor(request => request.Description)
            .MaximumLength(1000);
    }
}
