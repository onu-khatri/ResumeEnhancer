using Shouldly;
using ResumeEnhancer.Tests.Unit.TestInfrastructure;
using ResumeEnhancer.ResumeModule.AM.Requests;
using ResumeEnhancer.ResumeModule.Web.Validation.PersonalInformation;
using ResumeEnhancer.ResumeModule.Web.Validation.Sections;

namespace ResumeEnhancer.Tests.Unit.Modules.ResumeModule.Web.Validation;

public sealed class SectionValidatorTests
{
    [Fact]
    public void EducationRequestValidator_CreateRequestWithExistingId_ReturnsIdError()
    {
        var validator = new EducationRequestValidator(isCreate: true);
        var request = new EducationRequest { Id = 12 };

        var result = validator.Validate(request);

        result.ShouldHaveErrorFor(nameof(EducationRequest.Id), "must be 0 when creating");
    }

    [Fact]
    public void EducationRequestValidator_PercentageOutsideRange_ReturnsPercentageError()
    {
        var validator = new EducationRequestValidator();
        var request = new EducationRequest { Percentage = 101m };

        var result = validator.Validate(request);

        result.ShouldHaveErrorFor(nameof(EducationRequest.Percentage));
    }

    [Fact]
    public void CertificationRequestValidator_EndDateBeforeStartDate_ReturnsDateError()
    {
        var validator = new CertificationRequestValidator();
        var request = new CertificationRequest
        {
            CertificationName = "Azure",
            IssueDate = new DateTime(2025, 1, 1),
            ExpirationDate = new DateTime(2024, 1, 1)
        };

        var result = validator.Validate(request);

        result.ShouldHaveErrorFor(nameof(CertificationRequest.ExpirationDate), "start date cannot be later");
    }

    [Fact]
    public void CertificationRequestValidator_NonHttpCredentialUrl_ReturnsUrlError()
    {
        var validator = new CertificationRequestValidator();
        var request = new CertificationRequest
        {
            CertificationName = "Azure",
            CredentialUrl = "ftp://example.com/cert"
        };

        var result = validator.Validate(request);

        result.ShouldHaveErrorFor(nameof(CertificationRequest.CredentialUrl), "valid http or https URL");
    }

    [Fact]
    public void SkillRequestValidator_MissingNameAndNegativeExperience_ReturnsErrors()
    {
        var validator = new SkillRequestValidator();
        var request = new SkillRequest { YearsOfExperience = -1m };

        var result = validator.Validate(request);

        result.ShouldHaveErrorFor(nameof(SkillRequest.SkillName));
        result.ShouldHaveErrorFor(nameof(SkillRequest.YearsOfExperience));
    }

    [Fact]
    public void WorkExperienceRequestValidator_EndDateBeforeStartDate_ReturnsDateError()
    {
        var validator = new WorkExperienceRequestValidator();
        var request = new WorkExperienceRequest
        {
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2024, 1, 1)
        };

        var result = validator.Validate(request);

        result.ShouldHaveErrorFor(nameof(WorkExperienceRequest.EndDate), "start date cannot be later");
    }

    [Fact]
    public void ProjectRequestValidator_ValidRequest_Passes()
    {
        var validator = new ProjectRequestValidator();
        var request = new ProjectRequest
        {
            ProjectName = "Resume Builder",
            StartDate = new DateTime(2024, 1, 1),
            EndDate = new DateTime(2025, 1, 1)
        };

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void AwardLanguageHobbyAndSocialMediaValidators_RequiredFieldsMissing_ReturnErrors()
    {
        new AwardRequestValidator().Validate(new AwardRequest())
            .ShouldHaveErrorFor(nameof(AwardRequest.AwardName));
        new LanguageRequestValidator().Validate(new LanguageRequest())
            .ShouldHaveErrorFor(nameof(LanguageRequest.LanguageName));
        new HobbyRequestValidator().Validate(new HobbyRequest())
            .ShouldHaveErrorFor(nameof(HobbyRequest.HobbyName));
        new SocialMediaLinkRequestValidator().Validate(new SocialMediaLinkRequest { Url = "https://example.com" })
            .ShouldHaveErrorFor(nameof(SocialMediaLinkRequest.Platform));
    }

    [Fact]
    public void SocialMediaLinkRequestValidator_InvalidScheme_ReturnsUrlError()
    {
        var validator = new SocialMediaLinkRequestValidator();
        var request = new SocialMediaLinkRequest
        {
            Platform = "LinkedIn",
            Url = "mailto:person@example.com"
        };

        var result = validator.Validate(request);

        result.ShouldHaveErrorFor(nameof(SocialMediaLinkRequest.Url), "valid http or https URL");
    }

    [Fact]
    public void AddressRequestValidator_FieldExceedsMaxLength_ReturnsError()
    {
        var validator = new AddressRequestValidator();
        var request = new AddressRequest { ZipCode = new string('1', 21) };

        var result = validator.Validate(request);

        result.ShouldHaveErrorFor(nameof(AddressRequest.ZipCode));
    }
}


