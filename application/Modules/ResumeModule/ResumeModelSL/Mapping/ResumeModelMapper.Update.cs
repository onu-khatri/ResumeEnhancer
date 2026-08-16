using ResumeEnhancer.Core.DomainLibrary.DomainModel;
using Mapster;
using ResumeEnhancer.ResumeModule.AM.Requests;
using ResumeEnhancer.ResumeModule.DM.Entities;

namespace ResumeEnhancer.ResumeModule.SL.Handlers;

internal static partial class ResumeModelMapper
{
    public static void ApplyResumeUpdate(
        Resume resume,
        UpdateResumeRequest request,
        Action<AuditEntity> remove)
    {
        ArgumentNullException.ThrowIfNull(resume);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(remove);

        request.Adapt(resume, MapsterConfig);

        if (request.UserId is not null)
        {
            resume.UserId = request.UserId.Value;
        }

        if (request.TemplateId is not null)
        {
            resume.TemplateId = request.TemplateId.Value;
        }

        if (request.RemovePersonalInformation)
        {
            RemoveOwned(resume.PersonalInformation, remove);
            resume.PersonalInformation = null;
        }
        else if (request.PersonalInformation is not null)
        {
            ApplyPersonalInformationUpdate(resume, request.PersonalInformation, remove);
        }

        SyncCollection(resume.Education, request.Education, item => CreateEducation(item, resume), ApplyEducation, remove, nameof(request.Education));
        SyncCollection(resume.Certifications, request.Certifications, item => CreateCertification(item, resume), ApplyCertification, remove, nameof(request.Certifications));
        SyncCollection(resume.Skills, request.Skills, item => CreateSkill(item, resume), ApplySkill, remove, nameof(request.Skills));
        SyncCollection(resume.WorkExperiences, request.WorkExperiences, item => CreateWorkExperience(item, resume), ApplyWorkExperience, remove, nameof(request.WorkExperiences));
        SyncCollection(resume.Projects, request.Projects, item => CreateProject(item, resume), ApplyProject, remove, nameof(request.Projects));
    }

    private static void ApplyPersonalInformationUpdate(
        Resume resume,
        PersonalInformationRequest request,
        Action<AuditEntity> remove)
    {
        if (resume.PersonalInformation is null)
        {
            if (request.Id > 0)
            {
                throw new InvalidOperationException(
                    $"Personal information '{request.Id}' does not belong to resume '{resume.Id}'.");
            }

            resume.PersonalInformation = CreatePersonalInformation(request, resume);
            return;
        }

        if (request.Id > 0 && request.Id != resume.PersonalInformation.Id)
        {
            throw new InvalidOperationException(
                $"Personal information '{request.Id}' does not belong to resume '{resume.Id}'.");
        }

        ApplyPersonalInformationScalars(resume.PersonalInformation, request);

        if (request.RemoveAddress)
        {
            RemoveOwned(resume.PersonalInformation.Address, remove);
            resume.PersonalInformation.Address = null;
        }
        else if (request.Address is not null)
        {
            ApplyAddressUpdate(resume.PersonalInformation, request.Address);
        }

        SyncCollection(resume.PersonalInformation.Awards, request.Awards, item => CreateAward(item, resume.PersonalInformation), ApplyAward, remove, nameof(request.Awards));
        SyncCollection(resume.PersonalInformation.Languages, request.Languages, item => CreateLanguage(item, resume.PersonalInformation), ApplyLanguage, remove, nameof(request.Languages));
        SyncCollection(resume.PersonalInformation.Hobbies, request.Hobbies, item => CreateHobby(item, resume.PersonalInformation), ApplyHobby, remove, nameof(request.Hobbies));
        SyncCollection(resume.PersonalInformation.SocialMediaLinks, request.SocialMediaLinks, item => CreateSocialMediaLink(item, resume.PersonalInformation), ApplySocialMediaLink, remove, nameof(request.SocialMediaLinks));
    }

    private static void ApplyAddressUpdate(PersonalInformation personalInformation, AddressRequest request)
    {
        if (personalInformation.Address is null)
        {
            if (request.Id > 0)
            {
                throw new InvalidOperationException(
                    $"Address '{request.Id}' does not belong to personal information '{personalInformation.Id}'.");
            }

            personalInformation.Address = CreateAddress(request, personalInformation);
            return;
        }

        if (request.Id > 0 && request.Id != personalInformation.Address.Id)
        {
            throw new InvalidOperationException(
                $"Address '{request.Id}' does not belong to personal information '{personalInformation.Id}'.");
        }

        ApplyAddress(personalInformation.Address, request);
    }
}

