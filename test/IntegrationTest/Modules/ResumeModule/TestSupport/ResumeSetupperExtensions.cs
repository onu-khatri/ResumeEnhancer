using Microsoft.EntityFrameworkCore;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.TestUtilities.IntegrationSupport;
using ResumeEnhancer.ResumeModule.DM.Entities;

namespace ResumeEnhancer.Tests.Integration.Modules.ResumeModule;

internal static class ResumeSetupperExtensions
{
    public static async Task<Resume> GenerateResumeAsync(
        this ISetupper setupper,
        string userId = ResumeApiTestData.OwnerUserId,
        string title = "Integration API Resume",
        string? template = "Modern",
        string? photo = "https://example.com/photo.png",
        int auditUserId = 101,
        CancellationToken cancellationToken = default)
    {
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
}


