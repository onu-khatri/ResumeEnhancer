using ResumeEnhancer.ResumeModule.AM.Requests;
using ResumeEnhancer.ResumeModule.DM.Entities;
using ResumeEnhancer.BillingModule.DM.Entities;
using ResumeEnhancer.ProfilingModule.DM.Entities;
using ResumeEnhancer.ProfilingModule.DM.Enums;
using ResumeEnhancer.TemplateModule.DM.Entities;
using ResumeEnhancer.Core.DomainLibrary.DomainModel;

namespace ResumeEnhancer.Tests.Unit.TestInfrastructure;

internal static class ResumeTestData
{
    public const int UserId = 1;
    public const int OtherUserId = 2;
    public const int BillingPlanId = 1;
    public const int TemplateId = 1;
    public const int TemplateCategoryId = 1;
    public const int TemplateRenderTypeId = 1;
    public const int BillingAddressTypeId = 1;
    public const int CommunicationAddressTypeId = 2;
    public const int RoleId = 1;
    public const int AccessProfileId = 1;

    public static CreateResumeRequest CreateResumeRequest() =>
        new()
        {
            Title = " Senior Engineer ",
            Summary = " Builds things ",
            Photo = " ",
            ResumeTemplate = " Modern ",
            UserId = UserId,
            PersonalInformation = new PersonalInformationRequest
            {
                Email = " person@example.com ",
                PhoneNumber = " 1234567890 ",
                Address = new AddressRequest
                {
                    StreetAddress = " 1 Main St ",
                    City = " City ",
                    State = " State ",
                    Country = " Country ",
                    ZipCode = " 12345 "
                },
                Awards =
                [
                    new AwardRequest { AwardName = " MVP ", Description = " Great work " }
                ],
                Languages =
                [
                    new LanguageRequest { LanguageName = " English ", ProficiencyLevel = " Native " }
                ],
                Hobbies =
                [
                    new HobbyRequest { HobbyName = " Chess ", Description = " Blitz " }
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
            },
            Education =
            [
                new EducationRequest
                {
                    Degree = " BS ",
                    Institution = " University ",
                    City = " Town ",
                    PassingYear = 2025,
                    Percentage = 92.5m,
                    Grade = " A ",
                    IsCurrent = true
                }
            ],
            Certifications =
            [
                new CertificationRequest
                {
                    CertificationName = " Azure ",
                    IssuingOrganization = " Microsoft ",
                    IssueDate = new DateTime(2024, 1, 1),
                    ExpirationDate = new DateTime(2026, 1, 1),
                    CredentialUrl = " https://example.com/cert "
                }
            ],
            Skills =
            [
                new SkillRequest
                {
                    SkillName = " C# ",
                    ProficiencyLevel = " Advanced ",
                    YearsOfExperience = 8m
                }
            ],
            WorkExperiences =
            [
                new WorkExperienceRequest
                {
                    JobTitle = " Engineer ",
                    CompanyName = " Contoso ",
                    Location = " Remote ",
                    StartDate = new DateTime(2021, 1, 1),
                    EndDate = new DateTime(2023, 1, 1)
                }
            ],
            Projects =
            [
                new ProjectRequest
                {
                    ProjectName = " Resume Builder ",
                    Role = " Lead ",
                    TechnologiesUsed = " .NET ",
                    StartDate = new DateTime(2023, 1, 1),
                    EndDate = new DateTime(2024, 1, 1)
                }
            ]
        };

    public static Resume ResumeGraph(
        int id = 1,
        string title = "Senior Engineer",
        int userId = UserId,
        string? template = "Modern",
        string? photo = "photo.png",
        DateTime? created = null,
        DateTime? updated = null)
    {
        var resume = new Resume
        {
            Id = id,
            Title = title,
            UserId = userId,
            TemplateId = template is null ? null : TemplateId,
            Summary = "Summary",
            ResumeTemplate = template,
            Photo = photo,
            App_CreateDate = created ?? new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            App_UpdateDate = updated
        };

        resume.PersonalInformation = new PersonalInformation
        {
            Id = id * 10,
            Resume = resume,
            Email = $"user-{userId}@example.com",
            PhoneNumber = "1234567890",
            Address = new Address
            {
                Id = id * 10 + 1,
                StreetAddress = "1 Main St",
                City = "City"
            },
            Awards =
            {
                new Award { Id = id * 10 + 2, AwardName = "MVP" }
            },
            Languages =
            {
                new Language { Id = id * 10 + 3, LanguageName = "English" }
            },
            Hobbies =
            {
                new Hobby { Id = id * 10 + 4, HobbyName = "Chess" }
            },
            SocialMediaLinks =
            {
                new SocialMediaLink
                {
                    Id = id * 10 + 5,
                    Platform = "LinkedIn",
                    Url = "https://example.com/profile"
                }
            }
        };

        resume.Education.Add(new Education { Id = id * 100 + 1, Resume = resume, Degree = "BS" });
        resume.Certifications.Add(new Certification
        {
            Id = id * 100 + 2,
            Resume = resume,
            CertificationName = "Azure"
        });
        resume.Skills.Add(new Skill { Id = id * 100 + 3, Resume = resume, SkillName = "C#" });
        resume.WorkExperiences.Add(new WorkExperience
        {
            Id = id * 100 + 4,
            Resume = resume,
            JobTitle = "Engineer",
            CompanyName = "Contoso"
        });
        resume.Projects.Add(new Project
        {
            Id = id * 100 + 5,
            Resume = resume,
            ProjectName = "Resume Builder",
            TechnologiesUsed = ".NET"
        });

        return resume;
    }

    public static User User(
        int id = UserId,
        string email = "user@example.com") =>
        new()
        {
            Id = id,
            FirstName = "Test",
            LastName = "User",
            Email = email
        };

    public static UserAddressTypeSetup UserAddressType(
        int id,
        string code,
        int order) =>
        new()
        {
            Id = id,
            Code = code,
            Description = $"{code} address",
            DisplayName = code,
            Guid = DeterministicGuid($"user-address-type-{id}"),
            Order = order
        };

    public static Role Role(
        int id = RoleId,
        string code = "ADMIN",
        int order = 1) =>
        new()
        {
            Id = id,
            Code = code,
            Description = $"{code} role",
            DisplayName = code,
            Guid = DeterministicGuid($"role-{id}"),
            Order = order
        };

    public static AccessProfile AccessProfile(
        int id = AccessProfileId,
        string code = "DEFAULT",
        int order = 1) =>
        new()
        {
            Id = id,
            Code = code,
            Description = $"{code} access profile",
            DisplayName = code,
            Guid = DeterministicGuid($"access-profile-{id}"),
            Order = order
        };

    public static BillingPlan BillingPlan(
        int id = BillingPlanId,
        string code = "FREE",
        int order = 1) =>
        new()
        {
            Id = id,
            Code = code,
            Description = $"{code} billing plan",
            DisplayName = code,
            Guid = DeterministicGuid($"billing-plan-{id}"),
            Order = order,
            Price = 0,
            Currency = "USD",
            BillingInterval = "Monthly"
        };

    public static TemplateCategory TemplateCategory(
        int id = TemplateCategoryId) =>
        new()
        {
            Id = id,
            Code = $"category-{id}",
            Description = $"Template category {id}",
            DisplayName = $"Category {id}",
            Guid = DeterministicGuid($"template-category-{id}"),
            Order = id
        };

    public static TemplateRenderTypeSetup TemplateRenderType(
        int id = TemplateRenderTypeId) =>
        new()
        {
            Id = id,
            Code = $"render-type-{id}",
            Description = $"Render type {id}",
            DisplayName = $"Render type {id}",
            Guid = DeterministicGuid($"template-render-type-{id}"),
            Order = id
        };

    public static Template Template(
        int id = TemplateId,
        int categoryId = TemplateCategoryId,
        int renderTypeId = TemplateRenderTypeId) =>
        new()
        {
            Id = id,
            Code = $"template-{id}",
            Description = $"Template {id}",
            DisplayName = $"Template {id}",
            Guid = DeterministicGuid($"template-{id}"),
            TemplateCategoryId = categoryId,
            RenderTypeId = renderTypeId,
            Body = "<html></html>"
        };

    private static Guid DeterministicGuid(string value)
    {
        var bytes = System.Security.Cryptography.MD5.HashData(System.Text.Encoding.UTF8.GetBytes(value));
        return new Guid(bytes);
    }
}


