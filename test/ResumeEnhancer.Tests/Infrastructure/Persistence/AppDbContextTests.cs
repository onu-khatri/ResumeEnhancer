using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.Tests.Unit.TestInfrastructure;
using ResumeEnhancer.ResumeModule.DM.Entities;
using ResumeEnhancer.ResumeModule.PL;

namespace ResumeEnhancer.Tests.Unit.Infrastructure.Persistence;

public sealed class AppDbContextTests
{
    [Fact]
    public async Task SaveChangesAsync_AddedEntityWithAuditUser_StampsAuditValues()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        var resume = new Resume
        {
            Title = "Title",
            UserId = ResumeTestData.UserId,
            App_CreateDate = default
        };

        scope.DbContext.Add(resume);
        await scope.DbContext.SaveChangesAsync(new TestAudit(42), cancellationToken);

        resume.Id.ShouldBeGreaterThan(0);
        resume.App_CreateUserId.ShouldBe(42);
        resume.App_UpdateUserId.ShouldBe(42);
        resume.App_CreateDate.ShouldNotBe(default);
        resume.App_UpdateDate.ShouldNotBeNull();
    }

    [Fact]
    public async Task SaveChangesAsync_ModifiedEntity_PreservesCreateAuditValues()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        var createDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var resume = new Resume
        {
            Title = "Title",
            UserId = ResumeTestData.UserId,
            App_CreateDate = createDate,
            App_CreateUserId = 1
        };
        scope.DbContext.Add(resume);
        await scope.DbContext.SaveChangesAsync(new TestAudit(1), cancellationToken);

        resume.Title = "Updated";
        resume.App_CreateDate = createDate.AddYears(-1);
        resume.App_CreateUserId = 99;
        await scope.DbContext.SaveChangesAsync(new TestAudit(2), cancellationToken);

        scope.DbContext.Entry(resume).State = EntityState.Detached;
        var saved = await scope.DbContext.Set<Resume>().SingleAsync(cancellationToken);
        saved.App_CreateDate.ShouldBe(createDate);
        saved.App_CreateUserId.ShouldBe(1);
        saved.App_UpdateUserId.ShouldBe(2);
    }

    [Fact]
    public async Task SaveChangesAsync_WithoutAuditAndAcceptAllChangesFalse_PreservesTrackedState()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        var resume = new Resume { Title = "Draft", UserId = ResumeTestData.UserId };
        scope.DbContext.Add(resume);

        var affectedRows = await scope.DbContext.SaveChangesAsync(
            acceptAllChangesOnSuccess: false,
            cancellationToken);

        affectedRows.ShouldBe(1);
        scope.DbContext.Entry(resume).State.ShouldBe(EntityState.Added);
        scope.DbContext.ChangeTracker.AcceptAllChanges();
    }

    [Fact]
    public async Task SaveChangesAsync_WithAuditAndAcceptAllChangesFalse_PreservesTrackedState()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        var resume = new Resume { Title = "Audited Draft", UserId = ResumeTestData.UserId };
        scope.DbContext.Add(resume);

        var affectedRows = await scope.DbContext.SaveChangesAsync(
            new TestAudit(99),
            acceptAllChangesOnSuccess: false,
            cancellationToken);

        affectedRows.ShouldBe(1);
        resume.App_CreateUserId.ShouldBe(99);
        scope.DbContext.Entry(resume).State.ShouldBe(EntityState.Added);
        scope.DbContext.ChangeTracker.AcceptAllChanges();
    }

    [Fact]
    public async Task SaveChangesAsync_TrackedRowDeletedInDatabase_ThrowsConcurrencyException()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        var resume = new Resume { Title = "Original", UserId = ResumeTestData.UserId };
        scope.DbContext.Add(resume);
        await scope.DbContext.SaveChangesAsync(new TestAudit(1), cancellationToken);

        await scope.DbContext.Set<Resume>()
            .Where(item => item.Id == resume.Id)
            .ExecuteDeleteAsync(cancellationToken);
        resume.Title = "Changed";

        await Should.ThrowAsync<DbUpdateConcurrencyException>(
            () => scope.DbContext.SaveChangesAsync(new TestAudit(2), cancellationToken));
    }

    [Fact]
    public async Task SaveChangesAsync_InvalidTrackedEntity_ThrowsValidationException()
    {
        using var scope = new SqliteAppDbContextScope();
        var resume = new Resume
        {
            Title = new string('T', 201),
            UserId = ResumeTestData.UserId
        };
        scope.DbContext.Add(resume);

        await Should.ThrowAsync<ValidationException>(
            () => scope.DbContext.SaveChangesAsync(new TestAudit(1), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task SaveChangesAsync_AuditUserIsNull_ThrowsArgumentNullException()
    {
        using var scope = new SqliteAppDbContextScope();

        await Should.ThrowAsync<ArgumentNullException>(
            () => scope.DbContext.SaveChangesAsync(null!, TestContext.Current.CancellationToken));
    }

    [Fact]
    public void OnModelCreating_ResumeModuleConfiguration_AppliesSchemaTableAndBaseColumns()
    {
        using var scope = new SqliteAppDbContextScope();

        var resume = scope.DbContext.Model.FindEntityType(typeof(Resume))!;
        var sectionSetup = scope.DbContext.Model.FindEntityType(typeof(ResumeSectionSetup))!;

        resume.GetSchema().ShouldBe(ResumeModuleDatabase.Schema);
        resume.GetTableName().ShouldBe("B_Resume");
        resume.FindProperty(nameof(Resume.Title))!.GetMaxLength().ShouldBe(200);
        resume.FindProperty(nameof(Resume.UserId))!.IsNullable.ShouldBeFalse();
        resume.FindProperty(nameof(Resume.App_Version))!.IsConcurrencyToken.ShouldBeTrue();
        sectionSetup.GetTableName().ShouldBe("S_ResumeSectionSetup");
        sectionSetup.FindProperty(nameof(ResumeSectionSetup.Guid))!.IsNullable.ShouldBeFalse();
    }

    [Fact]
    public void AppDbContextResumeModuleExtensions_ReturnDbSets()
    {
        using var scope = new SqliteAppDbContextScope();

        scope.DbContext.Resumes().ShouldBeSameAs(scope.DbContext.Set<Resume>());
        scope.DbContext.PersonalInformation().ShouldBeSameAs(scope.DbContext.Set<PersonalInformation>());
        scope.DbContext.Addresses().ShouldBeSameAs(scope.DbContext.Set<Address>());
        scope.DbContext.Awards().ShouldBeSameAs(scope.DbContext.Set<Award>());
        scope.DbContext.Languages().ShouldBeSameAs(scope.DbContext.Set<Language>());
        scope.DbContext.Hobbies().ShouldBeSameAs(scope.DbContext.Set<Hobby>());
        scope.DbContext.SocialMediaLinks().ShouldBeSameAs(scope.DbContext.Set<SocialMediaLink>());
        scope.DbContext.Education().ShouldBeSameAs(scope.DbContext.Set<Education>());
        scope.DbContext.Certifications().ShouldBeSameAs(scope.DbContext.Set<Certification>());
        scope.DbContext.Skills().ShouldBeSameAs(scope.DbContext.Set<Skill>());
        scope.DbContext.WorkExperiences().ShouldBeSameAs(scope.DbContext.Set<WorkExperience>());
        scope.DbContext.Projects().ShouldBeSameAs(scope.DbContext.Set<Project>());
        scope.DbContext.ResumeSectionSetups().ShouldBeSameAs(scope.DbContext.Set<ResumeSectionSetup>());
    }

    [Fact]
    public void ResumeModuleDatabase_GetSchema_RootMissingOrProvided_ReturnsExpectedSchema()
    {
        ResumeModuleDatabase.GetSchema().ShouldBe("resume");
        ResumeModuleDatabase.GetSchema("identity").ShouldBe("identity_resume");
    }
}


