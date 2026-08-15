using FluentValidation;
using ResumeEnhancer.ResumeModule.AM.Requests;
using ResumeEnhancer.ResumeModule.Web.Validation.Shared;

namespace ResumeEnhancer.ResumeModule.Web.Validation.PersonalInformation;

public sealed class AddressRequestValidator : AbstractValidator<AddressRequest>
{
    public AddressRequestValidator(bool isCreate = false)
    {
        this.RuleForRequestId(request => request.Id, isCreate);

        RuleFor(request => request.StreetAddress)
            .MaximumLength(200);

        RuleFor(request => request.City)
            .MaximumLength(100);

        RuleFor(request => request.State)
            .MaximumLength(100);

        RuleFor(request => request.Country)
            .MaximumLength(100);

        RuleFor(request => request.ZipCode)
            .MaximumLength(20);
    }
}

