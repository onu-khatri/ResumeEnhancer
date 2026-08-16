using Bogus;
using ResumeEnhancer.ResumeModule.AM.Requests;
using ResumeEnhancer.ResumeModule.AM.Responses;
using ResumeEnhancer.ResumeModule.DM.Entities;

namespace ResumeEnhancer.Tests.Integration.Modules.ResumeModule;

internal static class ResumeApiTestData
{
    public const int OwnerUserId = 1001;
    public const int OtherUserId = 1002;
    public const int IntruderUserId = 1003;

    public static CreateResumeRequest CreateResumeRequest(
        int userId = OwnerUserId,
        bool includeFullGraph = true,
        int seed = 100)
    {
        var faker = Faker(seed);
        var request = new CreateResumeRequest
        {
            Title = $" {faker.Name.JobTitle()} Resume ",
            Summary = $" {faker.Lorem.Sentence()} ",
            Photo = includeFullGraph ? $" https://example.com/photos/{seed}.png " : null,
            ResumeTemplate = " Modern ",
            UserId = userId,
            PersonalInformation = includeFullGraph ? PersonalInformationRequest(seed) : null,
            Education = includeFullGraph ? [EducationRequest(seed)] : [],
            Certifications = includeFullGraph ? [CertificationRequest(seed)] : [],
            Skills = includeFullGraph ? [SkillRequest(seed)] : [],
            WorkExperiences = includeFullGraph ? [WorkExperienceRequest(seed)] : [],
            Projects = includeFullGraph ? [ProjectRequest(seed)] : []
        };

        return request;
    }

    public static UpdateResumeRequest UpdateResumeRequest(int seed = 200) =>
        new()
        {
            Title = " Updated Integration Resume ",
            Summary = " Updated summary from API ",
            Photo = " https://example.com/photos/updated.png ",
            ResumeTemplate = " Focused ",
            PersonalInformation = new PersonalInformationRequest
            {
                Email = $" updated-{seed}@example.com ",
                PhoneNumber = " 5551234567 ",
                RemoveAddress = true
            },
            Skills =
            [
                new SkillRequest
                {
                    SkillName = " Updated C# ",
                    ProficiencyLevel = " Expert ",
                    YearsOfExperience = 9
                },
                new SkillRequest
                {
                    SkillName = " Distributed Systems ",
                    ProficiencyLevel = " Advanced ",
                    YearsOfExperience = 6
                }
            ],
            Education = [],
            Certifications = null,
            WorkExperiences = null,
            Projects = null
        };

    public static ResumeSearchRequest SearchRequest(
        int userId = OwnerUserId,
        string? searchText = null,
        string? template = null,
        bool? hasPhoto = null) =>
        new()
        {
            UserId = userId,
            SearchText = searchText,
            ResumeTemplate = template,
            HasPhoto = hasPhoto,
            PageNumber = 1,
            PageSize = 10,
            SortBy = ResumeSearchSortBy.Id,
            SortDirection = SortDirection.Ascending
        };

    public static Resume ResumeGraph(
        int userId = OwnerUserId,
        string title = "Integration API Resume",
        string? template = "Modern",
        string? photo = "https://example.com/photo.png",
        int seed = 300)
    {
        var faker = Faker(seed);
        var resume = new Resume
        {
            Title = title,
            Summary = faker.Lorem.Sentence(),
            ResumeTemplate = template,
            Photo = photo,
            UserId = userId
        };

        resume.PersonalInformation = new PersonalInformation
        {
            Resume = resume,
            Email = $"user-{userId}@example.com",
            PhoneNumber = "5550001111",
            Address = new Address
            {
                StreetAddress = "100 Integration Way",
                City = "Testville",
                State = "TS",
                Country = "USA",
                ZipCode = "10001"
            }
        };
        resume.Skills.Add(new Skill
        {
            Resume = resume,
            SkillName = "C#",
            ProficiencyLevel = "Advanced",
            YearsOfExperience = 8
        });
        resume.Skills.Add(new Skill
        {
            Resume = resume,
            SkillName = "Legacy Skill",
            ProficiencyLevel = "Beginner",
            YearsOfExperience = 1
        });
        resume.Education.Add(new Education
        {
            Resume = resume,
            Degree = "BS Computer Science",
            Institution = "Integration University",
            PassingYear = 2020
        });
        resume.Projects.Add(new Project
        {
            Resume = resume,
            ProjectName = "Talent Platform",
            TechnologiesUsed = ".NET, SQL"
        });

        return resume;
    }

    private static PersonalInformationRequest PersonalInformationRequest(int seed)
    {
        var faker = Faker(seed);

        return new PersonalInformationRequest
        {
            Email = $" {faker.Internet.Email()} ",
            PhoneNumber = " 5551239876 ",
            Address = new AddressRequest
            {
                StreetAddress = $" {faker.Address.StreetAddress()} ",
                City = $" {faker.Address.City()} ",
                State = $" {faker.Address.StateAbbr()} ",
                Country = " USA ",
                ZipCode = " 90210 "
            },
            Awards =
            [
                new AwardRequest
                {
                    AwardName = " Delivery Excellence ",
                    IssuingOrganization = " Engineering Guild "
                }
            ],
            Languages =
            [
                new LanguageRequest
                {
                    LanguageName = " English ",
                    ProficiencyLevel = " Native "
                }
            ],
            Hobbies =
            [
                new HobbyRequest
                {
                    HobbyName = " Chess "
                }
            ],
            SocialMediaLinks =
            [
                new SocialMediaLinkRequest
                {
                    Platform = " LinkedIn ",
                    Url = " https://example.com/profile ",
                    DisplayName = " Profile "
                }
            ]
        };
    }

    private static EducationRequest EducationRequest(int seed) =>
        new()
        {
            Degree = " Bachelor of Technology ",
            Institution = " Integration University ",
            City = " Pune ",
            State = " MH ",
            PassingYear = 2020 + seed % 3,
            Percentage = 91.5m,
            Grade = " A "
        };

    private static CertificationRequest CertificationRequest(int seed) =>
        new()
        {
            CertificationName = " Azure Developer ",
            IssuingOrganization = " Microsoft ",
            IssueDate = new DateTime(2024, 1, 1),
            ExpirationDate = new DateTime(2027, 1, 1),
            CredentialId = $" AZ-{seed} ",
            CredentialUrl = $" https://example.com/certifications/{seed} "
        };

    private static SkillRequest SkillRequest(int seed) =>
        new()
        {
            SkillName = " C# ",
            ProficiencyLevel = " Advanced ",
            YearsOfExperience = 5 + seed % 5,
            Description = " Backend APIs "
        };

    private static WorkExperienceRequest WorkExperienceRequest(int seed) =>
        new()
        {
            JobTitle = " Software Engineer ",
            CompanyName = " Contoso ",
            Location = " Remote ",
            StartDate = new DateTime(2021, 1, 1),
            EndDate = new DateTime(2024, 1, 1),
            Description = $" Built API platform {seed} "
        };

    private static ProjectRequest ProjectRequest(int seed) =>
        new()
        {
            ProjectName = " Resume Enhancer ",
            Role = " Lead Engineer ",
            StartDate = new DateTime(2024, 2, 1),
            EndDate = new DateTime(2025, 2, 1),
            Description = $" Improved candidate workflows {seed} ",
            TechnologiesUsed = " .NET, EF Core "
        };

    private static Faker Faker(int seed)
    {
        Randomizer.Seed = new Random(seed);

        return new Faker("en");
    }
}


