using FluentValidation;
using ResumeModuleAM.Requests;
using ResumeModuleWeb.Validation.PersonalInformation;
using ResumeModuleWeb.Validation.Sections;

namespace ResumeModuleWeb.Validation.Resumes;

public sealed class CreateResumeRequestValidator : AbstractValidator<CreateResumeRequest>
{
    public CreateResumeRequestValidator()
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
            .NotEmpty()
            .MaximumLength(450);

        When(request => request.PersonalInformation is not null, () =>
        {
            RuleFor(request => request.PersonalInformation!)
                .SetValidator(new PersonalInformationRequestValidator(isCreate: true));
        });

        RuleFor(request => request.Education)
            .NotNull();
        RuleForEach(request => request.Education)
            .SetValidator(new EducationRequestValidator(isCreate: true));

        RuleFor(request => request.Certifications)
            .NotNull();
        RuleForEach(request => request.Certifications)
            .SetValidator(new CertificationRequestValidator(isCreate: true));

        RuleFor(request => request.Skills)
            .NotNull();
        RuleForEach(request => request.Skills)
            .SetValidator(new SkillRequestValidator(isCreate: true));

        RuleFor(request => request.WorkExperiences)
            .NotNull();
        RuleForEach(request => request.WorkExperiences)
            .SetValidator(new WorkExperienceRequestValidator(isCreate: true));

        RuleFor(request => request.Projects)
            .NotNull();
        RuleForEach(request => request.Projects)
            .SetValidator(new ProjectRequestValidator(isCreate: true));
    }
}
