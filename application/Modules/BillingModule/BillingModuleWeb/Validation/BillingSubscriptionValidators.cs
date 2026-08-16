using FluentValidation;
using ResumeEnhancer.BillingModule.AM.Requests;

namespace ResumeEnhancer.BillingModule.Web.Validation;

public sealed class CreateBillingSubscriptionRequestValidator : AbstractValidator<CreateBillingSubscriptionRequest>
{
    public CreateBillingSubscriptionRequestValidator()
    {
        RuleFor(request => request.BillingAccountId).GreaterThan(0);
        RuleFor(request => request.BillingPlanId).GreaterThan(0);
        RuleFor(request => request.ResumeId).GreaterThan(0).When(request => request.ResumeId.HasValue);
        RuleFor(request => request.Status).NotEmpty().MaximumLength(50);
    }
}

public sealed class UpdateBillingSubscriptionRequestValidator : AbstractValidator<UpdateBillingSubscriptionRequest>
{
    public UpdateBillingSubscriptionRequestValidator()
    {
        RuleFor(request => request.BillingAccountId).GreaterThan(0);
        RuleFor(request => request.BillingPlanId).GreaterThan(0);
        RuleFor(request => request.ResumeId).GreaterThan(0).When(request => request.ResumeId.HasValue);
        RuleFor(request => request.Status).NotEmpty().MaximumLength(50);
    }
}
