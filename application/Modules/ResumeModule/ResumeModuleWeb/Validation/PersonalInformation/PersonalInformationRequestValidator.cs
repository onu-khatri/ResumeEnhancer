using FluentValidation;
using ResumeModuleAM.Requests;
using ResumeModuleWeb.Validation.Sections;
using ResumeModuleWeb.Validation.Shared;

namespace ResumeModuleWeb.Validation.PersonalInformation;

public sealed class PersonalInformationRequestValidator : AbstractValidator<PersonalInformationRequest>
{
    public PersonalInformationRequestValidator(bool isCreate = false)
    {
        this.RuleForRequestId(request => request.Id, isCreate);

        RuleFor(request => request.Email)
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(request => request.PhoneNumber)
            .MaximumLength(30);

        if (isCreate)
        {
            RuleFor(request => request.RemoveAddress)
                .Equal(false)
                .WithMessage("Cannot remove an address while creating personal information.");
        }
        else
        {
            RuleFor(request => request.Address)
                .Null()
                .When(request => request.RemoveAddress)
                .WithMessage("Cannot remove and update address in the same request.");
        }

        When(request => request.Address is not null, () =>
        {
            RuleFor(request => request.Address!)
                .SetValidator(new AddressRequestValidator(isCreate));
        });

        RuleFor(request => request.Awards)
            .Must(requests => ResumeValidationRules.HasNoDuplicateExistingIds(requests, request => request.Id))
            .WithMessage((_, requests) => ResumeValidationRules.DuplicateExistingIdMessage(nameof(PersonalInformationRequest.Awards), requests, request => request.Id));
        RuleForEach(request => request.Awards)
            .SetValidator(new AwardRequestValidator(isCreate));

        RuleFor(request => request.Languages)
            .Must(requests => ResumeValidationRules.HasNoDuplicateExistingIds(requests, request => request.Id))
            .WithMessage((_, requests) => ResumeValidationRules.DuplicateExistingIdMessage(nameof(PersonalInformationRequest.Languages), requests, request => request.Id));
        RuleForEach(request => request.Languages)
            .SetValidator(new LanguageRequestValidator(isCreate));

        RuleFor(request => request.Hobbies)
            .Must(requests => ResumeValidationRules.HasNoDuplicateExistingIds(requests, request => request.Id))
            .WithMessage((_, requests) => ResumeValidationRules.DuplicateExistingIdMessage(nameof(PersonalInformationRequest.Hobbies), requests, request => request.Id));
        RuleForEach(request => request.Hobbies)
            .SetValidator(new HobbyRequestValidator(isCreate));

        RuleFor(request => request.SocialMediaLinks)
            .Must(requests => ResumeValidationRules.HasNoDuplicateExistingIds(requests, request => request.Id))
            .WithMessage((_, requests) => ResumeValidationRules.DuplicateExistingIdMessage(nameof(PersonalInformationRequest.SocialMediaLinks), requests, request => request.Id));
        RuleForEach(request => request.SocialMediaLinks)
            .SetValidator(new SocialMediaLinkRequestValidator(isCreate));
    }
}
