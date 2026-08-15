using Mapster;
using ResumeEnhancer.ResumeModule.AM.Requests;
using ResumeEnhancer.ResumeModule.DM.Entities;

namespace ResumeEnhancer.ResumeModule.SL.Handlers;

internal static partial class ResumeModelMapper
{
    public static Resume CreateResume(CreateResumeRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var resume = request.Adapt<Resume>(MapsterConfig);

        if (request.PersonalInformation is not null)
        {
            resume.PersonalInformation = CreatePersonalInformation(
                request.PersonalInformation,
                resume);
        }

        AddCreatedChildren(resume.Education, request.Education, item => CreateEducation(item, resume));
        AddCreatedChildren(resume.Certifications, request.Certifications, item => CreateCertification(item, resume));
        AddCreatedChildren(resume.Skills, request.Skills, item => CreateSkill(item, resume));
        AddCreatedChildren(resume.WorkExperiences, request.WorkExperiences, item => CreateWorkExperience(item, resume));
        AddCreatedChildren(resume.Projects, request.Projects, item => CreateProject(item, resume));

        return resume;
    }

    private static PersonalInformation CreatePersonalInformation(
        PersonalInformationRequest request,
        Resume resume)
    {
        var personalInformation = new PersonalInformation { Resume = resume };

        ApplyPersonalInformationScalars(personalInformation, request);

        if (request.Address is not null)
        {
            personalInformation.Address = CreateAddress(request.Address, personalInformation);
        }

        AddCreatedChildren(personalInformation.Awards, request.Awards ?? [], item => CreateAward(item, personalInformation));
        AddCreatedChildren(personalInformation.Languages, request.Languages ?? [], item => CreateLanguage(item, personalInformation));
        AddCreatedChildren(personalInformation.Hobbies, request.Hobbies ?? [], item => CreateHobby(item, personalInformation));
        AddCreatedChildren(personalInformation.SocialMediaLinks, request.SocialMediaLinks ?? [], item => CreateSocialMediaLink(item, personalInformation));

        return personalInformation;
    }

    private static Address CreateAddress(AddressRequest request, PersonalInformation personalInformation)
    {
        var address = new Address { PersonalInformation = personalInformation };

        ApplyAddress(address, request);

        return address;
    }

    private static Education CreateEducation(EducationRequest request, Resume resume)
    {
        var education = new Education { Resume = resume };

        ApplyEducation(education, request);

        return education;
    }

    private static Certification CreateCertification(CertificationRequest request, Resume resume)
    {
        var certification = new Certification { Resume = resume };

        ApplyCertification(certification, request);

        return certification;
    }

    private static Skill CreateSkill(SkillRequest request, Resume resume)
    {
        var skill = new Skill { Resume = resume };

        ApplySkill(skill, request);

        return skill;
    }

    private static WorkExperience CreateWorkExperience(WorkExperienceRequest request, Resume resume)
    {
        var workExperience = new WorkExperience { Resume = resume };

        ApplyWorkExperience(workExperience, request);

        return workExperience;
    }

    private static Project CreateProject(ProjectRequest request, Resume resume)
    {
        var project = new Project { Resume = resume };

        ApplyProject(project, request);

        return project;
    }

    private static Award CreateAward(AwardRequest request, PersonalInformation personalInformation)
    {
        var award = new Award { PersonalInformation = personalInformation };

        ApplyAward(award, request);

        return award;
    }

    private static Language CreateLanguage(LanguageRequest request, PersonalInformation personalInformation)
    {
        var language = new Language { PersonalInformation = personalInformation };

        ApplyLanguage(language, request);

        return language;
    }

    private static Hobby CreateHobby(HobbyRequest request, PersonalInformation personalInformation)
    {
        var hobby = new Hobby { PersonalInformation = personalInformation };

        ApplyHobby(hobby, request);

        return hobby;
    }

    private static SocialMediaLink CreateSocialMediaLink(
        SocialMediaLinkRequest request,
        PersonalInformation personalInformation)
    {
        var socialMediaLink = new SocialMediaLink { PersonalInformation = personalInformation };

        ApplySocialMediaLink(socialMediaLink, request);

        return socialMediaLink;
    }
}

