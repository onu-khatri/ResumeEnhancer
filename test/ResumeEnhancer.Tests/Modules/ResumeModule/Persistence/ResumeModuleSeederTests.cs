using Microsoft.EntityFrameworkCore;
using Shouldly;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.Tests.Unit.TestInfrastructure;
using ResumeEnhancer.ResumeModule.DM.Entities;
using ResumeEnhancer.ResumeModule.DM.Enums;
using ResumeEnhancer.ResumeModule.PL.Seeding;

namespace ResumeEnhancer.Tests.Unit.Modules.ResumeModule.Persistence;

public sealed class ResumeModuleSeederTests
{
    [Fact]
    public void ResumeSectionSetupSeedData_Create_ReturnsCompleteUniqueVisibleSectionList()
    {
        var seeds = ResumeSectionSetupSeedData.Create();

        seeds.Length.ShouldBe(Enum.GetValues<ResumeSectionType>().Length);
        seeds.Select(seed => seed.Code).Distinct(StringComparer.Ordinal).Count().ShouldBe(seeds.Length);
        seeds.Select(seed => seed.Guid).Distinct().Count().ShouldBe(seeds.Length);
        seeds.Select(seed => seed.Order).Distinct().Count().ShouldBe(seeds.Length);
        seeds.All(seed => seed.IsVisible).ShouldBeTrue();
        seeds.All(seed => !seed.ObsoleteFlag).ShouldBeTrue();
    }

    [Fact]
    public async Task SeedAsync_EmptyDatabase_AddsAllResumeSectionsWithSeederAudit()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        var seeder = new ResumeModuleSeeder();

        await seeder.SeedAsync(scope.DbContext, cancellationToken);

        var sections = await scope.DbContext.Set<ResumeSectionSetup>()
            .OrderBy(section => section.Order)
            .ToArrayAsync(cancellationToken);
        sections.Length.ShouldBe(Enum.GetValues<ResumeSectionType>().Length);
        sections.First().Code.ShouldBe(nameof(ResumeSectionType.Education));
        sections.Last().Code.ShouldBe(nameof(ResumeSectionType.SocialMediaLinks));
        sections.All(section => section.App_CreateUserId == SeedingUser.UserId).ShouldBeTrue();
        sections.All(section => section.App_UpdateUserId == SeedingUser.UserId).ShouldBeTrue();
    }

    [Fact]
    public async Task SeedAsync_ExistingSectionWithChangedFields_UpdatesBaseAndModuleFields()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        var educationSeed = ResumeSectionSetupSeedData.Create()
            .Single(seed => seed.Code == nameof(ResumeSectionType.Education));
        scope.DbContext.Add(new ResumeSectionSetup
        {
            Code = "Old Education",
            Description = "Old Description",
            Guid = educationSeed.Guid,
            Order = 99,
            IsVisible = false,
            App_UpdateUserId = SeedingUser.UserId
        });
        await scope.DbContext.SaveChangesAsync(cancellationToken);
        scope.DbContext.ChangeTracker.Clear();
        var seeder = new ResumeModuleSeeder();

        await seeder.SeedAsync(scope.DbContext, cancellationToken);

        var education = await scope.DbContext.Set<ResumeSectionSetup>()
            .SingleAsync(section => section.Guid == educationSeed.Guid, cancellationToken);
        education.Code.ShouldBe(nameof(ResumeSectionType.Education));
        education.Description.ShouldBe("Education");
        education.Order.ShouldBe(1);
        education.IsVisible.ShouldBeTrue();
        education.App_UpdateUserId.ShouldBe(SeedingUser.UserId);
    }

    [Fact]
    public async Task SeedAppDbContextAsync_InvokesRegisteredResumeModuleSeeder()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;

        await scope.Services.SeedAppDbContextAsync(cancellationToken);

        var count = await scope.DbContext.Set<ResumeSectionSetup>().CountAsync(cancellationToken);
        count.ShouldBe(Enum.GetValues<ResumeSectionType>().Length);
    }
}


