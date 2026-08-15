using System.ComponentModel.DataAnnotations;
using Shouldly;
using ResumeEnhancer.ResumeModule.DM.Entities;
using ResumeEnhancer.ResumeModule.DM.Enums;

namespace ResumeEnhancer.Tests.Unit.Modules.ResumeModule.DomainModel;

public sealed class ResumeDomainModelTests
{
    [Fact]
    public void Resume_DefaultCollections_AreInitialized()
    {
        var resume = new Resume();

        resume.Education.ShouldBeEmpty();
        resume.Certifications.ShouldBeEmpty();
        resume.Skills.ShouldBeEmpty();
        resume.WorkExperiences.ShouldBeEmpty();
        resume.Projects.ShouldBeEmpty();
        resume.PersonalInformation.ShouldBeNull();
    }

    [Fact]
    public void PersonalInformation_DefaultCollections_AreInitialized()
    {
        var personalInformation = new PersonalInformation();

        personalInformation.Awards.ShouldBeEmpty();
        personalInformation.Languages.ShouldBeEmpty();
        personalInformation.Hobbies.ShouldBeEmpty();
        personalInformation.SocialMediaLinks.ShouldBeEmpty();
        personalInformation.Address.ShouldBeNull();
    }

    [Fact]
    public void ResumeSectionSetup_DefaultValues_AreVisibleAndEducationStartsAtOne()
    {
        var setup = new ResumeSectionSetup
        {
            SectionType = ResumeSectionType.Education
        };

        setup.IsVisible.ShouldBeTrue();
        ((int)setup.SectionType).ShouldBe(1);
    }

    [Fact]
    public void Resume_TitleExceedsMaxLength_FailsDataAnnotationsValidation()
    {
        var resume = new Resume
        {
            Title = new string('T', 201),
            UserId = "user"
        };
        var validationResults = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(
            resume,
            new ValidationContext(resume),
            validationResults,
            validateAllProperties: true);

        isValid.ShouldBeFalse();
        validationResults.ShouldContain(result => result.MemberNames.Contains(nameof(Resume.Title)));
    }
}


