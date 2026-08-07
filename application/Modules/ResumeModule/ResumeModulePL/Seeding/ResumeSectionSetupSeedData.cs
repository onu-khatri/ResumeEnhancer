using ResumeModuleDM.Entities;
using ResumeModuleDM.Enums;

namespace ResumeModulePL.Seeding;

internal static class ResumeSectionSetupSeedData
{
    public static ResumeSectionSetup[] Create() =>
    [
        new ResumeSectionSetup
        {
            Id = 1,
            SectionType = ResumeSectionType.Education,
            SectionTitle = "Education",
            DisplayOrder = 1,
            IsVisible = true
        },
        new ResumeSectionSetup
        {
            Id = 2,
            SectionType = ResumeSectionType.Certifications,
            SectionTitle = "Certifications",
            DisplayOrder = 2,
            IsVisible = true
        },
        new ResumeSectionSetup
        {
            Id = 3,
            SectionType = ResumeSectionType.Skills,
            SectionTitle = "Skills",
            DisplayOrder = 3,
            IsVisible = true
        },
        new ResumeSectionSetup
        {
            Id = 4,
            SectionType = ResumeSectionType.Languages,
            SectionTitle = "Languages",
            DisplayOrder = 4,
            IsVisible = true
        },
        new ResumeSectionSetup
        {
            Id = 5,
            SectionType = ResumeSectionType.WorkExperience,
            SectionTitle = "Work Experience",
            DisplayOrder = 5,
            IsVisible = true
        },
        new ResumeSectionSetup
        {
            Id = 6,
            SectionType = ResumeSectionType.Projects,
            SectionTitle = "Projects",
            DisplayOrder = 6,
            IsVisible = true
        },
        new ResumeSectionSetup
        {
            Id = 7,
            SectionType = ResumeSectionType.Awards,
            SectionTitle = "Awards",
            DisplayOrder = 7,
            IsVisible = true
        },
        new ResumeSectionSetup
        {
            Id = 8,
            SectionType = ResumeSectionType.Hobbies,
            SectionTitle = "Hobbies",
            DisplayOrder = 8,
            IsVisible = true
        },
        new ResumeSectionSetup
        {
            Id = 9,
            SectionType = ResumeSectionType.SocialMediaLinks,
            SectionTitle = "Social Media Links",
            DisplayOrder = 9,
            IsVisible = true
        }
    ];
}
