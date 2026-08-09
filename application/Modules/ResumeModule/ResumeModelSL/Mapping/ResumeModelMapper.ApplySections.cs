using Mapster;
using ResumeModuleAM.Requests;
using ResumeModuleDM.Entities;

namespace ResumeModuleSL.Handlers;

internal static partial class ResumeModelMapper
{
    private static void ApplyPersonalInformationScalars(
        PersonalInformation personalInformation,
        PersonalInformationRequest request)
    {
        request.Adapt(personalInformation, MapsterConfig);
    }

    private static void ApplyAddress(Address address, AddressRequest request)
    {
        request.Adapt(address, MapsterConfig);
    }

    private static void ApplyEducation(Education education, EducationRequest request)
    {
        request.Adapt(education, MapsterConfig);
    }

    private static void ApplyCertification(Certification certification, CertificationRequest request)
    {
        request.Adapt(certification, MapsterConfig);
    }

    private static void ApplySkill(Skill skill, SkillRequest request)
    {
        request.Adapt(skill, MapsterConfig);
    }

    private static void ApplyWorkExperience(WorkExperience workExperience, WorkExperienceRequest request)
    {
        request.Adapt(workExperience, MapsterConfig);
    }

    private static void ApplyProject(Project project, ProjectRequest request)
    {
        request.Adapt(project, MapsterConfig);
    }

    private static void ApplyAward(Award award, AwardRequest request)
    {
        request.Adapt(award, MapsterConfig);
    }

    private static void ApplyLanguage(Language language, LanguageRequest request)
    {
        request.Adapt(language, MapsterConfig);
    }

    private static void ApplyHobby(Hobby hobby, HobbyRequest request)
    {
        request.Adapt(hobby, MapsterConfig);
    }

    private static void ApplySocialMediaLink(SocialMediaLink socialMediaLink, SocialMediaLinkRequest request)
    {
        request.Adapt(socialMediaLink, MapsterConfig);
    }
}
