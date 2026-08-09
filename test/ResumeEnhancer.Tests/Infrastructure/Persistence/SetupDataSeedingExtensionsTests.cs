using Microsoft.EntityFrameworkCore;
using Shouldly;
using Persistence;
using ResumeEnhancer.Tests.TestInfrastructure;
using ResumeModuleDM.Entities;
using ResumeModuleDM.Enums;

namespace ResumeEnhancer.Tests.Infrastructure.Persistence;

public sealed class SetupDataSeedingExtensionsTests
{
    [Fact]
    public async Task SeedSetupDataAsync_NewSeeds_AddsRowsAndStampsSeederAudit()
    {
        using var scope = new SqliteAppDbContextScope();
        var seed = Seed("Education", "Education");

        await scope.DbContext.Set<ResumeSectionSetup>().SeedSetupDataAsync(
            [seed],
            (_, _) => false,
            TestContext.Current.CancellationToken);
        await scope.DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var saved = await scope.DbContext.Set<ResumeSectionSetup>().SingleAsync(TestContext.Current.CancellationToken);
        saved.Code.ShouldBe("Education");
        saved.App_CreateUserId.ShouldBe(SeedingUser.UserId);
        saved.App_UpdateUserId.ShouldBe(SeedingUser.UserId);
    }

    [Fact]
    public async Task SeedSetupDataAsync_ExistingManagedRowMissingFromIncoming_MarksObsolete()
    {
        using var scope = new SqliteAppDbContextScope();
        scope.DbContext.Add(Seed("Old", "Old", updateUserId: SeedingUser.UserId));
        await scope.DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        await scope.DbContext.Set<ResumeSectionSetup>().SeedSetupDataAsync(
            [Seed("New", "New", displayOrder: 2)],
            (_, _) => false,
            TestContext.Current.CancellationToken);
        await scope.DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var old = await scope.DbContext.Set<ResumeSectionSetup>()
            .SingleAsync(setup => setup.Code == "Old", TestContext.Current.CancellationToken);
        old.ObsoleteFlag.ShouldBeTrue();
        old.App_UpdateUserId.ShouldBe(SeedingUser.UserId);
    }

    [Fact]
    public async Task SeedSetupDataAsync_ExistingUnmanagedRowMissingFromIncoming_DoesNotMarkObsolete()
    {
        using var scope = new SqliteAppDbContextScope();
        scope.DbContext.Add(Seed("Old", "Old", updateUserId: 123));
        await scope.DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        await scope.DbContext.Set<ResumeSectionSetup>().SeedSetupDataAsync(
            [Seed("New", "New", displayOrder: 2)],
            (_, _) => false,
            TestContext.Current.CancellationToken);
        await scope.DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var old = await scope.DbContext.Set<ResumeSectionSetup>()
            .SingleAsync(setup => setup.Code == "Old", TestContext.Current.CancellationToken);
        old.ObsoleteFlag.ShouldBeFalse();
        old.App_UpdateUserId.ShouldBe(123);
    }

    [Fact]
    public async Task SeedSetupDataAsync_ExistingRowByGuid_UpdatesBaseAndCustomFields()
    {
        using var scope = new SqliteAppDbContextScope();
        var guid = Guid.NewGuid();
        scope.DbContext.Add(Seed("Old", "Old", guid));
        await scope.DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        await scope.DbContext.Set<ResumeSectionSetup>().SeedSetupDataAsync(
            [Seed("New", "New", guid, displayOrder: 7)],
            (existing, seed) =>
            {
                existing.DisplayOrder = seed.DisplayOrder;
                return true;
            },
            TestContext.Current.CancellationToken);
        await scope.DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var saved = await scope.DbContext.Set<ResumeSectionSetup>().SingleAsync(TestContext.Current.CancellationToken);
        saved.Code.ShouldBe("New");
        saved.Description.ShouldBe("New");
        saved.DisplayOrder.ShouldBe(7);
        saved.App_UpdateUserId.ShouldBe(SeedingUser.UserId);
    }

    [Fact]
    public async Task SeedSetupDataAsync_MissingIdentityOrDuplicates_ThrowInvalidOperationException()
    {
        using var scope = new SqliteAppDbContextScope();
        var guid = Guid.NewGuid();

        await Should.ThrowAsync<InvalidOperationException>(
            () => scope.DbContext.Set<ResumeSectionSetup>().SeedSetupDataAsync(
                [Seed("", "Missing code")],
                (_, _) => false,
                TestContext.Current.CancellationToken));
        await Should.ThrowAsync<InvalidOperationException>(
            () => scope.DbContext.Set<ResumeSectionSetup>().SeedSetupDataAsync(
                [Seed("A", "A", guid), Seed("B", "B", guid)],
                (_, _) => false,
                TestContext.Current.CancellationToken));
        await Should.ThrowAsync<InvalidOperationException>(
            () => scope.DbContext.Set<ResumeSectionSetup>().SeedSetupDataAsync(
                [Seed("A", "A"), Seed("a", "B")],
                (_, _) => false,
                TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task SeedSetupDataAsync_GuidAndCodeMatchDifferentRows_ThrowsInvalidOperationException()
    {
        using var scope = new SqliteAppDbContextScope();
        var firstGuid = Guid.NewGuid();
        var secondGuid = Guid.NewGuid();
        scope.DbContext.AddRange(
            Seed("A", "A", firstGuid),
            Seed("B", "B", secondGuid, displayOrder: 2));
        await scope.DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        await Should.ThrowAsync<InvalidOperationException>(
            () => scope.DbContext.Set<ResumeSectionSetup>().SeedSetupDataAsync(
                [Seed("B", "Conflict", firstGuid)],
                (_, _) => false,
                TestContext.Current.CancellationToken));
    }

    private static ResumeSectionSetup Seed(
        string code,
        string description,
        Guid? guid = null,
        int displayOrder = 1,
        int? updateUserId = null) =>
        new()
        {
            Code = code,
            Description = description,
            Guid = guid ?? Guid.NewGuid(),
            DisplayOrder = displayOrder,
            IsVisible = true,
            SectionType = (ResumeSectionType)displayOrder,
            App_UpdateUserId = updateUserId
        };
}
