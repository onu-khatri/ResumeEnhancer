using FluentValidation;
using ResumeEnhancer.ProfilingModule.AM.Requests;

namespace ResumeEnhancer.ProfilingModule.Web.Validation;

public sealed class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(request => request.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(request => request.LastName).NotEmpty().MaximumLength(100);
        RuleFor(request => request.Email).NotEmpty().EmailAddress().MaximumLength(320);
        RuleFor(request => request.BillingAddressLine1).MaximumLength(200);
        RuleFor(request => request.BillingCity).MaximumLength(100);
        RuleFor(request => request.BillingCountry).MaximumLength(100);
        RuleFor(request => request.CommunicationAddressLine1).MaximumLength(200);
        RuleFor(request => request.CommunicationCity).MaximumLength(100);
        RuleFor(request => request.CommunicationCountry).MaximumLength(100);
        RuleForEach(request => request.AccessProfileIds).GreaterThan(0);
    }
}

public sealed class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(request => request.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(request => request.LastName).NotEmpty().MaximumLength(100);
        RuleFor(request => request.Email).NotEmpty().EmailAddress().MaximumLength(320);
        RuleFor(request => request.BillingAddressLine1).MaximumLength(200);
        RuleFor(request => request.BillingCity).MaximumLength(100);
        RuleFor(request => request.BillingCountry).MaximumLength(100);
        RuleFor(request => request.CommunicationAddressLine1).MaximumLength(200);
        RuleFor(request => request.CommunicationCity).MaximumLength(100);
        RuleFor(request => request.CommunicationCountry).MaximumLength(100);
        RuleForEach(request => request.AccessProfileIds).GreaterThan(0);
    }
}
