using Microsoft.EntityFrameworkCore;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.ProfilingModule.DM.Entities;
using ResumeEnhancer.TestUtilities.IntegrationSupport;
using ResumeEnhancer.ResumeModule.DM.Entities;
using ResumeEnhancer.TemplateModule.DM.Entities;

namespace ResumeEnhancer.Tests.Integration.Modules.ResumeModule;

internal static class ResumeSetupperExtensions
{
    public static async Task<Resume> GenerateResumeAsync(
        this ISetupper setupper,
        int userId = ResumeApiTestData.OwnerUserId,
        string title = "Integration API Resume",
        string? template = "Modern",
        string? photo = "https://example.com/photo.png",
        int auditUserId = 101,
        CancellationToken cancellationToken = default)
    {
        await setupper.EnsureResumeReferenceDataAsync(cancellationToken);
        var resume = ResumeApiTestData.ResumeGraph(userId, title, template, photo);

        return await setupper.SetupResumeAsync(resume, auditUserId, cancellationToken);
    }

    public static async Task<Resume> SetupResumeAsync(
        this ISetupper setupper,
        Resume resume,
        int auditUserId = 101,
        CancellationToken cancellationToken = default)
    {
        var dbContext = (AppDbContext)setupper.GetDbContext();

        await setupper.EnsureResumeReferenceDataAsync(cancellationToken);
        dbContext.Add(resume);
        await dbContext.SaveChangesAsync(new IntegrationTestAudit(auditUserId), cancellationToken);
        dbContext.ChangeTracker.Clear();

        return resume;
    }

    public static async Task<Resume?> FindResumeGraphAsync(
        this ISetupper setupper,
        int resumeId,
        CancellationToken cancellationToken = default)
    {
        var dbContext = (AppDbContext)setupper.GetFreshDbContext();

        return await dbContext.Set<Resume>()
            .Include(resume => resume.PersonalInformation)
                .ThenInclude(personalInformation => personalInformation!.Address)
            .Include(resume => resume.PersonalInformation)
                .ThenInclude(personalInformation => personalInformation!.Awards)
            .Include(resume => resume.PersonalInformation)
                .ThenInclude(personalInformation => personalInformation!.Languages)
            .Include(resume => resume.PersonalInformation)
                .ThenInclude(personalInformation => personalInformation!.Hobbies)
            .Include(resume => resume.PersonalInformation)
                .ThenInclude(personalInformation => personalInformation!.SocialMediaLinks)
            .Include(resume => resume.Education)
            .Include(resume => resume.Certifications)
            .Include(resume => resume.Skills)
            .Include(resume => resume.WorkExperiences)
            .Include(resume => resume.Projects)
            .AsSplitQuery()
            .SingleOrDefaultAsync(resume => resume.Id == resumeId, cancellationToken);
    }

    public static async Task EnsureResumeReferenceDataAsync(
        this ISetupper setupper,
        CancellationToken cancellationToken = default)
    {
        var dbContext = (AppDbContext)setupper.GetDbContext();

        if (!await dbContext.Set<User>().AnyAsync(user => user.Id == ResumeApiTestData.OwnerUserId, cancellationToken))
        {
            dbContext.Add(ResumeApiTestData.User(ResumeApiTestData.OwnerUserId));
        }

        if (!await dbContext.Set<User>().AnyAsync(user => user.Id == ResumeApiTestData.OtherUserId, cancellationToken))
        {
            dbContext.Add(ResumeApiTestData.User(ResumeApiTestData.OtherUserId));
        }

        if (!await dbContext.Set<User>().AnyAsync(user => user.Id == ResumeApiTestData.IntruderUserId, cancellationToken))
        {
            dbContext.Add(ResumeApiTestData.User(ResumeApiTestData.IntruderUserId));
        }

        if (!await dbContext.Set<TemplateCategory>().AnyAsync(
                category => category.Id == ResumeApiTestData.TemplateCategoryId,
                cancellationToken))
        {
            dbContext.Add(ResumeApiTestData.TemplateCategory());
        }

        if (!await dbContext.Set<TemplateRenderTypeSetup>().AnyAsync(
                renderType => renderType.Id == ResumeApiTestData.TemplateRenderTypeId,
                cancellationToken))
        {
            dbContext.Add(ResumeApiTestData.TemplateRenderType());
        }

        if (!await dbContext.Set<Template>().AnyAsync(
                template => template.Id == ResumeApiTestData.TemplateId,
                cancellationToken))
        {
            dbContext.Add(ResumeApiTestData.Template());
        }

        await dbContext.SaveChangesAsync(new IntegrationTestAudit(999), cancellationToken);
        dbContext.ChangeTracker.Clear();
    }
}


