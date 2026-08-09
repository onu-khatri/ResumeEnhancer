using Mapster;
using ResumeModuleAM.Requests;
using ResumeModuleAM.Responses;
using ResumeModuleDM.Entities;
using ResumeModuleSL.Abstractions.Persistence;

namespace ResumeModuleSL.Handlers;

internal static partial class ResumeModelMapper
{
    private static readonly TypeAdapterConfig MapsterConfig = CreateMapsterConfig();

    private static TypeAdapterConfig CreateMapsterConfig()
    {
        var config = new TypeAdapterConfig();

        config.NewConfig<CreateResumeRequest, Resume>()
            .Map(dest => dest.Title, src => NormalizeRequired(src.Title))
            .Map(dest => dest.Summary, src => NormalizeOptional(src.Summary))
            .Map(dest => dest.Photo, src => NormalizeOptional(src.Photo))
            .Map(dest => dest.ResumeTemplate, src => NormalizeOptional(src.ResumeTemplate))
            .Map(dest => dest.UserId, src => NormalizeRequired(src.UserId))
            .Ignore(dest => dest.PersonalInformation)
            .Ignore(dest => dest.Education)
            .Ignore(dest => dest.Certifications)
            .Ignore(dest => dest.Skills)
            .Ignore(dest => dest.WorkExperiences)
            .Ignore(dest => dest.Projects);

        config.NewConfig<UpdateResumeRequest, Resume>()
            .Map(dest => dest.Title, src => NormalizeRequired(src.Title))
            .Map(dest => dest.Summary, src => NormalizeOptional(src.Summary))
            .Map(dest => dest.Photo, src => NormalizeOptional(src.Photo))
            .Map(dest => dest.ResumeTemplate, src => NormalizeOptional(src.ResumeTemplate))
            .Ignore(dest => dest.UserId)
            .Ignore(dest => dest.PersonalInformation)
            .Ignore(dest => dest.Education)
            .Ignore(dest => dest.Certifications)
            .Ignore(dest => dest.Skills)
            .Ignore(dest => dest.WorkExperiences)
            .Ignore(dest => dest.Projects);

        config.NewConfig<PersonalInformationRequest, PersonalInformation>()
            .Map(dest => dest.Email, src => NormalizeOptional(src.Email))
            .Map(dest => dest.PhoneNumber, src => NormalizeOptional(src.PhoneNumber))
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.ResumeId)
            .Ignore(dest => dest.Resume)
            .Ignore(dest => dest.Address)
            .Ignore(dest => dest.Awards)
            .Ignore(dest => dest.Languages)
            .Ignore(dest => dest.Hobbies)
            .Ignore(dest => dest.SocialMediaLinks);

        config.NewConfig<AddressRequest, Address>()
            .Map(dest => dest.StreetAddress, src => NormalizeOptional(src.StreetAddress))
            .Map(dest => dest.City, src => NormalizeOptional(src.City))
            .Map(dest => dest.State, src => NormalizeOptional(src.State))
            .Map(dest => dest.Country, src => NormalizeOptional(src.Country))
            .Map(dest => dest.ZipCode, src => NormalizeOptional(src.ZipCode))
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.PersonalInformationId)
            .Ignore(dest => dest.PersonalInformation);

        config.NewConfig<EducationRequest, Education>()
            .Map(dest => dest.Degree, src => NormalizeOptional(src.Degree))
            .Map(dest => dest.Institution, src => NormalizeOptional(src.Institution))
            .Map(dest => dest.City, src => NormalizeOptional(src.City))
            .Map(dest => dest.State, src => NormalizeOptional(src.State))
            .Map(dest => dest.Description, src => NormalizeOptional(src.Description))
            .Map(dest => dest.Grade, src => NormalizeOptional(src.Grade))
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.ResumeId)
            .Ignore(dest => dest.Resume);

        config.NewConfig<CertificationRequest, Certification>()
            .Map(dest => dest.CertificationName, src => NormalizeRequired(src.CertificationName))
            .Map(dest => dest.IssuingOrganization, src => NormalizeOptional(src.IssuingOrganization))
            .Map(dest => dest.CredentialId, src => NormalizeOptional(src.CredentialId))
            .Map(dest => dest.CredentialUrl, src => NormalizeOptional(src.CredentialUrl))
            .Map(dest => dest.Description, src => NormalizeOptional(src.Description))
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.ResumeId)
            .Ignore(dest => dest.Resume);

        config.NewConfig<SkillRequest, Skill>()
            .Map(dest => dest.SkillName, src => NormalizeRequired(src.SkillName))
            .Map(dest => dest.ProficiencyLevel, src => NormalizeOptional(src.ProficiencyLevel))
            .Map(dest => dest.Description, src => NormalizeOptional(src.Description))
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.ResumeId)
            .Ignore(dest => dest.Resume);

        config.NewConfig<WorkExperienceRequest, WorkExperience>()
            .Map(dest => dest.JobTitle, src => NormalizeOptional(src.JobTitle))
            .Map(dest => dest.CompanyName, src => NormalizeOptional(src.CompanyName))
            .Map(dest => dest.Location, src => NormalizeOptional(src.Location))
            .Map(dest => dest.Description, src => NormalizeOptional(src.Description))
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.ResumeId)
            .Ignore(dest => dest.Resume);

        config.NewConfig<ProjectRequest, Project>()
            .Map(dest => dest.ProjectName, src => NormalizeRequired(src.ProjectName))
            .Map(dest => dest.Role, src => NormalizeOptional(src.Role))
            .Map(dest => dest.Description, src => NormalizeOptional(src.Description))
            .Map(dest => dest.TechnologiesUsed, src => NormalizeOptional(src.TechnologiesUsed))
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.ResumeId)
            .Ignore(dest => dest.Resume);

        config.NewConfig<AwardRequest, Award>()
            .Map(dest => dest.AwardName, src => NormalizeRequired(src.AwardName))
            .Map(dest => dest.IssuingOrganization, src => NormalizeOptional(src.IssuingOrganization))
            .Map(dest => dest.Description, src => NormalizeOptional(src.Description))
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.PersonalInformationId)
            .Ignore(dest => dest.PersonalInformation);

        config.NewConfig<LanguageRequest, Language>()
            .Map(dest => dest.LanguageName, src => NormalizeRequired(src.LanguageName))
            .Map(dest => dest.ProficiencyLevel, src => NormalizeOptional(src.ProficiencyLevel))
            .Map(dest => dest.Description, src => NormalizeOptional(src.Description))
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.PersonalInformationId)
            .Ignore(dest => dest.PersonalInformation);

        config.NewConfig<HobbyRequest, Hobby>()
            .Map(dest => dest.HobbyName, src => NormalizeRequired(src.HobbyName))
            .Map(dest => dest.Description, src => NormalizeOptional(src.Description))
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.PersonalInformationId)
            .Ignore(dest => dest.PersonalInformation);

        config.NewConfig<SocialMediaLinkRequest, SocialMediaLink>()
            .Map(dest => dest.Platform, src => NormalizeRequired(src.Platform))
            .Map(dest => dest.Url, src => NormalizeRequired(src.Url))
            .Map(dest => dest.DisplayName, src => NormalizeOptional(src.DisplayName))
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.PersonalInformationId)
            .Ignore(dest => dest.PersonalInformation);

        config.NewConfig<ResumeSearchRequest, ResumeSearchCriteria>()
            .Map(dest => dest.UserId, src => NormalizeOptional(src.UserId))
            .Map(dest => dest.SearchText, src => NormalizeOptional(src.SearchText))
            .Map(dest => dest.ResumeTemplate, src => NormalizeOptional(src.ResumeTemplate))
            .Map(dest => dest.SortBy, src => ToResumeSortBy(src.SortBy))
            .Map(dest => dest.SortDirection, src => ToResumeSortDirection(src.SortDirection));

        config.NewConfig<Resume, ResumeListItemResponse>()
            .Map(dest => dest.EducationCount, src => src.Education.Count)
            .Map(dest => dest.CertificationCount, src => src.Certifications.Count)
            .Map(dest => dest.SkillCount, src => src.Skills.Count)
            .Map(dest => dest.WorkExperienceCount, src => src.WorkExperiences.Count)
            .Map(dest => dest.ProjectCount, src => src.Projects.Count);

        config.NewConfig<ResumeSearchResult, ResumeSearchResponse>()
            .MapWith(src => new ResumeSearchResponse(
                src.Items.Adapt<IReadOnlyList<ResumeListItemResponse>>(MapsterConfig),
                src.PageNumber,
                src.PageSize,
                src.TotalCount));

        config.NewConfig<ResumeDeleteResult, ResumeDeleteResponse>()
            .MapWith(src => new ResumeDeleteResponse(
                src.RequestedIds,
                src.DeletedIds,
                src.NotFoundIds,
                src.ForbiddenIds));

        return config;
    }

    private static ResumeSortBy ToResumeSortBy(ResumeSearchSortBy sortBy) =>
        sortBy switch
        {
            ResumeSearchSortBy.Id => ResumeSortBy.Id,
            ResumeSearchSortBy.Title => ResumeSortBy.Title,
            ResumeSearchSortBy.CreatedDate => ResumeSortBy.CreatedDate,
            ResumeSearchSortBy.ResumeTemplate => ResumeSortBy.ResumeTemplate,
            _ => ResumeSortBy.UpdatedDate
        };

    private static ResumeSortDirection ToResumeSortDirection(SortDirection sortDirection) =>
        sortDirection == SortDirection.Ascending
            ? ResumeSortDirection.Ascending
            : ResumeSortDirection.Descending;
}
