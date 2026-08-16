using FluentValidation;
using ResumeEnhancer.BillingModule.AM.Requests;

namespace ResumeEnhancer.BillingModule.Web.Validation;

public sealed class CreateBillingAccountRequestValidator : AbstractValidator<CreateBillingAccountRequest>
{
    public CreateBillingAccountRequestValidator()
    {
        RuleFor(request => request.UserId).GreaterThan(0);
        RuleFor(request => request.AccountNumber).NotEmpty().MaximumLength(50);
        RuleFor(request => request.Status).NotEmpty().MaximumLength(50);
        RuleFor(request => request.ExternalReference).MaximumLength(100);
    }
}

public sealed class UpdateBillingAccountRequestValidator : AbstractValidator<UpdateBillingAccountRequest>
{
    public UpdateBillingAccountRequestValidator()
    {
        RuleFor(request => request.UserId).GreaterThan(0);
        RuleFor(request => request.AccountNumber).NotEmpty().MaximumLength(50);
        RuleFor(request => request.Status).NotEmpty().MaximumLength(50);
        RuleFor(request => request.ExternalReference).MaximumLength(100);
    }
}
