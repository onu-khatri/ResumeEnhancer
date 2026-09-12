using FluentValidation;
using ResumeEnhancer.BillingModule.AM.Requests;

namespace ResumeEnhancer.BillingModule.Web.Validation;

public sealed class CreateBillingPlanRequestValidator : AbstractValidator<CreateBillingPlanRequest>
{
    public CreateBillingPlanRequestValidator()
    {
        RuleFor(request => request.Code).NotEmpty().MaximumLength(100);
        RuleFor(request => request.Description).NotEmpty().MaximumLength(1000);
        RuleFor(request => request.DisplayName).NotEmpty().MaximumLength(200);
        RuleFor(request => request.Price).GreaterThanOrEqualTo(0);
        RuleFor(request => request.CurrencyId).GreaterThan(0);
        RuleFor(request => request.BillingIntervalId).GreaterThan(0);
        RuleFor(request => request.AccessProfileId).GreaterThan(0);
    }
}

public sealed class UpdateBillingPlanRequestValidator : AbstractValidator<UpdateBillingPlanRequest>
{
    public UpdateBillingPlanRequestValidator()
    {
        RuleFor(request => request.Code).NotEmpty().MaximumLength(100);
        RuleFor(request => request.Description).NotEmpty().MaximumLength(1000);
        RuleFor(request => request.DisplayName).NotEmpty().MaximumLength(200);
        RuleFor(request => request.Price).GreaterThanOrEqualTo(0);
        RuleFor(request => request.CurrencyId).GreaterThan(0);
        RuleFor(request => request.BillingIntervalId).GreaterThan(0);
        RuleFor(request => request.AccessProfileId).GreaterThan(0);
    }
}
