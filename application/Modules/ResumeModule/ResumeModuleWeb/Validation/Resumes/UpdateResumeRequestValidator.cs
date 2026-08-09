using FluentValidation;
using ResumeModuleAM.Requests;
using ResumeModuleWeb.Validation.PersonalInformation;
using ResumeModuleWeb.Validation.Sections;
using ResumeModuleWeb.Validation.Shared;

namespace ResumeModuleWeb.Validation.Resumes;

public sealed class UpdateResumeRequestValidator : AbstractValidator<UpdateResumeRequest>
{
    public UpdateResumeRequestValidator()
    {
        RuleFor(request => request.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.Summary)
            .MaximumLength(2000);

        RuleFor(request => request.Photo)
            .MaximumLength(500);

        RuleFor(request => request.ResumeTemplate)
            .MaximumLength(100);

        RuleFor(request => request.UserId)
            .MaximumLength(450);

        RuleFor(request => request.PersonalInformation)
            .Null()
            .When(request => request.RemovePersonalInformation)
            .WithMessage("Cannot remove and update personal information in the same request.");

        When(request => request.PersonalInformation is not null, () =>
        {
            RuleFor(request => request.PersonalInformation!)
                .SetValidator(new PersonalInformationRequestValidator(isCreate: false));
        });

        RuleFor(request => request.Education)
            .Must(requests => ResumeValidationRules.HasNoDuplicateExistingIds(requests, request => request.Id))
            .WithMessage((_, requests) => ResumeValidationRules.DuplicateExistingIdMessage(nameof(UpdateResumeRequest.Education), requests, request => request.Id));
        RuleForEach(request => request.Education)
            .SetValidator(new EducationRequestValidator(isCreate: false));

        RuleFor(request => request.Certifications)
            .Must(requests => ResumeValidationRules.HasNoDuplicateExistingIds(requests, request => request.Id))
            .WithMessage((_, requests) => ResumeValidationRules.DuplicateExistingIdMessage(nameof(UpdateResumeRequest.Certifications), requests, request => request.Id));
        RuleForEach(request => request.Certifications)
            .SetValidator(new CertificationRequestValidator(isCreate: false));

        RuleFor(request => request.Skills)
            .Must(requests => ResumeValidationRules.HasNoDuplicateExistingIds(requests, request => request.Id))
            .WithMessage((_, requests) => ResumeValidationRules.DuplicateExistingIdMessage(nameof(UpdateResumeRequest.Skills), requests, request => request.Id));
        RuleForEach(request => request.Skills)
            .SetValidator(new SkillRequestValidator(isCreate: false));

        RuleFor(request => request.WorkExperiences)
            .Must(requests => ResumeValidationRules.HasNoDuplicateExistingIds(requests, request => request.Id))
            .WithMessage((_, requests) => ResumeValidationRules.DuplicateExistingIdMessage(nameof(UpdateResumeRequest.WorkExperiences), requests, request => request.Id));
        RuleForEach(request => request.WorkExperiences)
            .SetValidator(new WorkExperienceRequestValidator(isCreate: false));

        RuleFor(request => request.Projects)
            .Must(requests => ResumeValidationRules.HasNoDuplicateExistingIds(requests, request => request.Id))
            .WithMessage((_, requests) => ResumeValidationRules.DuplicateExistingIdMessage(nameof(UpdateResumeRequest.Projects), requests, request => request.Id));
        RuleForEach(request => request.Projects)
            .SetValidator(new ProjectRequestValidator(isCreate: false));
    }
}
