using FluentValidation;
using ResumeModuleAM.Requests;
using ResumeModuleWeb.Validation.Shared;

namespace ResumeModuleWeb.Validation.PersonalInformation;

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
