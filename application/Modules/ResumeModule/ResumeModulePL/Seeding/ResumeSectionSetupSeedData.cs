using ResumeEnhancer.ResumeModule.DM.Entities;
using ResumeEnhancer.ResumeModule.DM.Enums;

namespace ResumeEnhancer.ResumeModule.PL.Seeding;

internal static class ResumeSectionSetupSeedData
{
    public static ResumeSectionSetup[] Create() =>
    [
        new ResumeSectionSetup
        {
            Code = nameof(ResumeSectionType.Education),
            Description = "Education",
            Guid = System.Guid.Parse("11111111-1111-1111-1111-111111111001"),
            ObsoleteFlag = false,
            SectionType = ResumeSectionType.Education,
            DisplayOrder = 1,
            IsVisible = true
        },
        new ResumeSectionSetup
        {
            Code = nameof(ResumeSectionType.Certifications),
            Description = "Certifications",
            Guid = System.Guid.Parse("11111111-1111-1111-1111-111111111002"),
            ObsoleteFlag = false,
            SectionType = ResumeSectionType.Certifications,
            DisplayOrder = 2,
            IsVisible = true
        },
        new ResumeSectionSetup
        {
            Code = nameof(ResumeSectionType.Skills),
            Description = "Skills",
            Guid = System.Guid.Parse("11111111-1111-1111-1111-111111111003"),
            ObsoleteFlag = false,
            SectionType = ResumeSectionType.Skills,
            DisplayOrder = 3,
            IsVisible = true
        },
        new ResumeSectionSetup
        {
            Code = nameof(ResumeSectionType.Languages),
            Description = "Languages",
            Guid = System.Guid.Parse("11111111-1111-1111-1111-111111111004"),
            ObsoleteFlag = false,
            SectionType = ResumeSectionType.Languages,
            DisplayOrder = 4,
            IsVisible = true
        },
        new ResumeSectionSetup
        {
            Code = nameof(ResumeSectionType.WorkExperience),
            Description = "Work Experience",
            Guid = System.Guid.Parse("11111111-1111-1111-1111-111111111005"),
            ObsoleteFlag = false,
            SectionType = ResumeSectionType.WorkExperience,
            DisplayOrder = 5,
            IsVisible = true
        },
        new ResumeSectionSetup
        {
            Code = nameof(ResumeSectionType.Projects),
            Description = "Projects",
            Guid = System.Guid.Parse("11111111-1111-1111-1111-111111111006"),
            ObsoleteFlag = false,
            SectionType = ResumeSectionType.Projects,
            DisplayOrder = 6,
            IsVisible = true
        },
        new ResumeSectionSetup
        {
            Code = nameof(ResumeSectionType.Awards),
            Description = "Awards",
            Guid = System.Guid.Parse("11111111-1111-1111-1111-111111111007"),
            ObsoleteFlag = false,
            SectionType = ResumeSectionType.Awards,
            DisplayOrder = 7,
            IsVisible = true
        },
        new ResumeSectionSetup
        {
            Code = nameof(ResumeSectionType.Hobbies),
            Description = "Hobbies",
            Guid = System.Guid.Parse("11111111-1111-1111-1111-111111111008"),
            ObsoleteFlag = false,
            SectionType = ResumeSectionType.Hobbies,
            DisplayOrder = 8,
            IsVisible = true
        },
        new ResumeSectionSetup
        {
            Code = nameof(ResumeSectionType.SocialMediaLinks),
            Description = "Social Media Links",
            Guid = System.Guid.Parse("11111111-1111-1111-1111-111111111009"),
            ObsoleteFlag = false,
            SectionType = ResumeSectionType.SocialMediaLinks,
            DisplayOrder = 9,
            IsVisible = true
        }
    ];
}

