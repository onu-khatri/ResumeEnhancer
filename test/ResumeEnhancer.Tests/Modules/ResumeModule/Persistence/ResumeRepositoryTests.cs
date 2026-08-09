using Microsoft.EntityFrameworkCore;
using Shouldly;
using ResumeEnhancer.Tests.TestInfrastructure;
using ResumeModuleDM.Entities;
using ResumeModulePL.Repositories;
using ResumeModuleSL.Abstractions.Persistence;

namespace ResumeEnhancer.Tests.Modules.ResumeModule.Persistence;

public sealed class ResumeRepositoryTests
{
    [Fact]
    public async Task AddAsync_ValidResume_SavesGraphAndStampsAuditUser()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = CreateRepository(scope);
        var resume = ResumeTestData.ResumeGraph(id: 0, title: "New Resume");

        var added = await repository.AddAsync(resume, auditUserId: 77, cancellationToken);

        added.ShouldBeSameAs(resume);
        added.Id.ShouldBeGreaterThan(0);
        scope.DbContext.ChangeTracker.Clear();
        var saved = await scope.DbContext.Set<Resume>()
            .Include(item => item.Education)
            .SingleAsync(cancellationToken);
        saved.Title.ShouldBe("New Resume");
        saved.Education.ShouldNotBeEmpty();
        saved.App_CreateUserId.ShouldBe(77);
        saved.App_UpdateUserId.ShouldBe(77);
    }

    [Fact]
    public async Task GetAsync_WithTrackingAndUserFilter_ReturnsIncludedGraphAndExpectedTrackingState()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        await SeedAsync(scope, cancellationToken, ResumeTestData.ResumeGraph(id: 1));
        var repository = CreateRepository(scope);

        var untracked = await repository.GetAsync(1, $" {ResumeTestData.UserId} ", track: false, cancellationToken);
        var tracked = await repository.GetAsync(1, ResumeTestData.UserId, track: true, cancellationToken);
        var missingForUser = await repository.GetAsync(1, ResumeTestData.OtherUserId, cancellationToken: cancellationToken);

        untracked.ShouldNotBeNull();
        untracked!.PersonalInformation.ShouldNotBeNull();
        untracked.Education.ShouldNotBeEmpty();
        scope.DbContext.Entry(untracked).State.ShouldBe(EntityState.Detached);
        tracked.ShouldNotBeNull();
        scope.DbContext.Entry(tracked!).State.ShouldNotBe(EntityState.Detached);
        missingForUser.ShouldBeNull();
    }

    [Fact]
    public async Task ExistsAsync_WithAndWithoutUserFilter_ReturnsExpectedResult()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        await SeedAsync(scope, cancellationToken, ResumeTestData.ResumeGraph(id: 1));
        var repository = CreateRepository(scope);

        (await repository.ExistsAsync(1, cancellationToken: cancellationToken)).ShouldBeTrue();
        (await repository.ExistsAsync(1, $" {ResumeTestData.UserId} ", cancellationToken)).ShouldBeTrue();
        (await repository.ExistsAsync(1, ResumeTestData.OtherUserId, cancellationToken)).ShouldBeFalse();
        (await repository.ExistsAsync(999, cancellationToken: cancellationToken)).ShouldBeFalse();
    }

    [Fact]
    public async Task SearchAsync_FilterCombination_ReturnsMatchingPagedGraph()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        var candidate = ResumeTestData.ResumeGraph(
            id: 1,
            title: "Senior Engineer",
            template: "Modern",
            photo: "photo.png",
            created: new DateTime(2024, 1, 10, 0, 0, 0, DateTimeKind.Utc));
        candidate.WorkExperiences.Single().CompanyName = "Contoso Labs";
        var wrongTemplate = ResumeTestData.ResumeGraph(
            id: 2,
            title: "Designer",
            template: "Classic",
            photo: "photo.png",
            created: new DateTime(2024, 1, 12, 0, 0, 0, DateTimeKind.Utc));
        var wrongUser = ResumeTestData.ResumeGraph(
            id: 3,
            title: "Architect",
            userId: ResumeTestData.OtherUserId,
            template: "Modern",
            photo: "photo.png",
            created: new DateTime(2024, 1, 14, 0, 0, 0, DateTimeKind.Utc));
        await SeedAsync(scope, cancellationToken, candidate, wrongTemplate, wrongUser);
        await SetUpdatedDateAsync(scope, 1, new DateTime(2024, 1, 20, 0, 0, 0, DateTimeKind.Utc), cancellationToken);
        await SetUpdatedDateAsync(scope, 2, new DateTime(2024, 1, 21, 0, 0, 0, DateTimeKind.Utc), cancellationToken);
        var repository = CreateRepository(scope);

        var result = await repository.SearchAsync(
            new ResumeSearchCriteria
            {
                Ids = [1, 1, 2, 3],
                UserId = $" {ResumeTestData.UserId} ",
                ResumeTemplate = " Modern ",
                HasPhoto = true,
                CreatedFromUtc = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedToUtc = new DateTime(2024, 1, 31, 0, 0, 0, DateTimeKind.Utc),
                UpdatedFromUtc = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                UpdatedToUtc = new DateTime(2024, 1, 25, 0, 0, 0, DateTimeKind.Utc),
                SearchText = " Contoso ",
                SortBy = ResumeSortBy.Id,
                SortDirection = ResumeSortDirection.Ascending,
                PageNumber = 1,
                PageSize = 10
            },
            cancellationToken);

        result.TotalCount.ShouldBe(1);
        result.Items.ShouldHaveSingleItem().Id.ShouldBe(1);
        result.Items[0].WorkExperiences.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task SearchAsync_HasPhotoFalseAndEmptyIds_HandleEdgeCases()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        await SeedAsync(
            scope,
            cancellationToken,
            ResumeTestData.ResumeGraph(id: 1, photo: null),
            ResumeTestData.ResumeGraph(id: 2, photo: string.Empty),
            ResumeTestData.ResumeGraph(id: 3, photo: "photo.png"));
        var repository = CreateRepository(scope);

        var noPhoto = await repository.SearchAsync(
            new ResumeSearchCriteria
            {
                HasPhoto = false,
                SortBy = ResumeSortBy.Id,
                SortDirection = ResumeSortDirection.Ascending
            },
            cancellationToken);
        var emptyIds = await repository.SearchAsync(
            new ResumeSearchCriteria { Ids = [], PageNumber = 2, PageSize = 5 },
            cancellationToken);

        noPhoto.Items.Select(resume => resume.Id).ShouldBe([1, 2]);
        emptyIds.TotalCount.ShouldBe(0);
        emptyIds.Items.ShouldBeEmpty();
        emptyIds.PageNumber.ShouldBe(2);
        emptyIds.PageSize.ShouldBe(5);
    }

    [Fact]
    public async Task SearchAsync_SearchTextMatchesSupportedFields()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        var resume = ResumeTestData.ResumeGraph(id: 1, title: "Principal Engineer", template: "Modern");
        resume.Summary = "Platform modernization";
        resume.PersonalInformation!.Email = "profile@example.com";
        resume.PersonalInformation.PhoneNumber = "555-0100";
        resume.Skills.Single().SkillName = "Distributed Systems";
        resume.WorkExperiences.Single().JobTitle = "Technical Lead";
        resume.WorkExperiences.Single().CompanyName = "Northwind";
        resume.Projects.Single().ProjectName = "Hiring Portal";
        resume.Projects.Single().TechnologiesUsed = "PostgreSQL";
        await SeedAsync(scope, cancellationToken, resume);
        var repository = CreateRepository(scope);
        var searchTerms = new[]
        {
            "Principal",
            "modernization",
            "Modern",
            "profile@example.com",
            "555-0100",
            "Distributed",
            "Technical",
            "Northwind",
            "Hiring",
            "PostgreSQL"
        };

        foreach (var searchTerm in searchTerms)
        {
            var result = await repository.SearchAsync(
                new ResumeSearchCriteria { SearchText = searchTerm },
                cancellationToken);

            result.Items.ShouldHaveSingleItem().Id.ShouldBe(1);
        }
    }

    [Fact]
    public async Task SearchAsync_AllSortOptions_ReturnExpectedOrder()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        await SeedAsync(
            scope,
            cancellationToken,
            ResumeTestData.ResumeGraph(
                id: 1,
                title: "Bravo",
                template: "Beta",
                created: new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc)),
            ResumeTestData.ResumeGraph(
                id: 2,
                title: "Alpha",
                template: "Alpha",
                created: new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
            ResumeTestData.ResumeGraph(
                id: 3,
                title: "Charlie",
                template: "Gamma",
                created: new DateTime(2024, 1, 3, 0, 0, 0, DateTimeKind.Utc)));
        await SetUpdatedDateAsync(scope, 1, new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc), cancellationToken);
        await SetUpdatedDateAsync(scope, 2, new DateTime(2024, 1, 3, 0, 0, 0, DateTimeKind.Utc), cancellationToken);
        await SetUpdatedDateAsync(scope, 3, new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), cancellationToken);
        var repository = CreateRepository(scope);

        await SearchIdsShouldBeAsync(repository, ResumeSortBy.Title, ResumeSortDirection.Ascending, [2, 1, 3], cancellationToken);
        await SearchIdsShouldBeAsync(repository, ResumeSortBy.Title, ResumeSortDirection.Descending, [3, 1, 2], cancellationToken);
        await SearchIdsShouldBeAsync(repository, ResumeSortBy.CreatedDate, ResumeSortDirection.Ascending, [2, 1, 3], cancellationToken);
        await SearchIdsShouldBeAsync(repository, ResumeSortBy.CreatedDate, ResumeSortDirection.Descending, [3, 1, 2], cancellationToken);
        await SearchIdsShouldBeAsync(repository, ResumeSortBy.ResumeTemplate, ResumeSortDirection.Ascending, [2, 1, 3], cancellationToken);
        await SearchIdsShouldBeAsync(repository, ResumeSortBy.ResumeTemplate, ResumeSortDirection.Descending, [3, 1, 2], cancellationToken);
        await SearchIdsShouldBeAsync(repository, ResumeSortBy.Id, ResumeSortDirection.Ascending, [1, 2, 3], cancellationToken);
        await SearchIdsShouldBeAsync(repository, ResumeSortBy.Id, ResumeSortDirection.Descending, [3, 2, 1], cancellationToken);
        await SearchIdsShouldBeAsync(repository, ResumeSortBy.UpdatedDate, ResumeSortDirection.Ascending, [3, 1, 2], cancellationToken);
        await SearchIdsShouldBeAsync(repository, ResumeSortBy.UpdatedDate, ResumeSortDirection.Descending, [2, 1, 3], cancellationToken);
    }

    [Fact]
    public async Task SearchAsync_InvalidCriteria_ThrowsExpectedExceptions()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = CreateRepository(scope);

        await Should.ThrowAsync<ArgumentNullException>(
            () => repository.SearchAsync(null!, cancellationToken));
        await Should.ThrowAsync<ArgumentOutOfRangeException>(
            () => repository.SearchAsync(new ResumeSearchCriteria { PageNumber = 0 }, cancellationToken));
        await Should.ThrowAsync<ArgumentOutOfRangeException>(
            () => repository.SearchAsync(new ResumeSearchCriteria { PageSize = 0 }, cancellationToken));
        await Should.ThrowAsync<ArgumentOutOfRangeException>(
            () => repository.SearchAsync(new ResumeSearchCriteria { PageSize = 101 }, cancellationToken));
        await Should.ThrowAsync<ArgumentOutOfRangeException>(
            () => repository.SearchAsync(new ResumeSearchCriteria { Ids = [1, 0] }, cancellationToken));
        await Should.ThrowAsync<ArgumentException>(
            () => repository.SearchAsync(
                new ResumeSearchCriteria
                {
                    CreatedFromUtc = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc),
                    CreatedToUtc = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                cancellationToken));
        await Should.ThrowAsync<ArgumentException>(
            () => repository.SearchAsync(
                new ResumeSearchCriteria
                {
                    UpdatedFromUtc = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedToUtc = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                cancellationToken));
    }

    [Fact]
    public async Task DeleteAsync_WithUserFilter_DeletesAllowedAndReportsForbiddenAndMissing()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        await SeedAsync(
            scope,
            cancellationToken,
            ResumeTestData.ResumeGraph(id: 1, userId: ResumeTestData.UserId),
            ResumeTestData.ResumeGraph(id: 2, userId: ResumeTestData.OtherUserId));
        var repository = CreateRepository(scope);

        var result = await repository.DeleteAsync(
            [1, 2, 999, 1],
            auditUserId: 88,
            userId: $" {ResumeTestData.UserId} ",
            cancellationToken);

        result.RequestedIds.ShouldBe([1, 2, 999]);
        result.DeletedIds.ShouldBe([1]);
        result.ForbiddenIds.ShouldBe([2]);
        result.NotFoundIds.ShouldBe([999]);
        (await scope.DbContext.Set<Resume>().AnyAsync(resume => resume.Id == 1, cancellationToken))
            .ShouldBeFalse();
        (await scope.DbContext.Set<Resume>().AnyAsync(resume => resume.Id == 2, cancellationToken))
            .ShouldBeTrue();
    }

    [Fact]
    public async Task DeleteAsync_WithoutUserFilter_DeletesAllRequestedRows()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        await SeedAsync(
            scope,
            cancellationToken,
            ResumeTestData.ResumeGraph(id: 1, userId: ResumeTestData.UserId),
            ResumeTestData.ResumeGraph(id: 2, userId: ResumeTestData.OtherUserId));
        var repository = CreateRepository(scope);

        var result = await repository.DeleteAsync([1, 2], auditUserId: null, cancellationToken: cancellationToken);

        result.DeletedIds.ShouldBe([1, 2], ignoreOrder: true);
        result.ForbiddenIds.ShouldBeEmpty();
        (await scope.DbContext.Set<Resume>().CountAsync(cancellationToken)).ShouldBe(0);
    }

    [Fact]
    public async Task DeleteAsync_EmptyIds_ReturnsEmptyResultWithoutSaving()
    {
        using var scope = new SqliteAppDbContextScope();
        var repository = CreateRepository(scope);

        var result = await repository.DeleteAsync([], auditUserId: 1, cancellationToken: TestContext.Current.CancellationToken);

        result.RequestedIds.ShouldBeEmpty();
        result.DeletedIds.ShouldBeEmpty();
        result.ForbiddenIds.ShouldBeEmpty();
        result.NotFoundIds.ShouldBeEmpty();
    }

    [Fact]
    public async Task RemoveAndSaveAsync_DeletesTrackedEntity()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        await SeedAsync(scope, cancellationToken, ResumeTestData.ResumeGraph(id: 1));
        var repository = CreateRepository(scope);
        var resume = await repository.GetAsync(1, track: true, cancellationToken: cancellationToken);

        repository.Remove(resume!);
        await repository.SaveAsync(auditUserId: 5, cancellationToken);

        (await scope.DbContext.Set<Resume>().AnyAsync(cancellationToken)).ShouldBeFalse();
    }

    [Fact]
    public async Task PublicMethods_NullOrInvalidArguments_ThrowExpectedExceptions()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = CreateRepository(scope);

        await Should.ThrowAsync<ArgumentNullException>(
            () => repository.AddAsync(null!, auditUserId: 1, cancellationToken));
        Should.Throw<ArgumentNullException>(() => repository.Remove(null!));
        await Should.ThrowAsync<ArgumentNullException>(
            () => repository.DeleteAsync(null!, auditUserId: 1, cancellationToken: cancellationToken));
        await Should.ThrowAsync<ArgumentOutOfRangeException>(
            () => repository.DeleteAsync([1, -1], auditUserId: 1, cancellationToken: cancellationToken));
        await Should.ThrowAsync<ArgumentOutOfRangeException>(
            () => repository.GetAsync(0, cancellationToken: cancellationToken));
        await Should.ThrowAsync<ArgumentOutOfRangeException>(
            () => repository.ExistsAsync(-1, cancellationToken: cancellationToken));
    }

    private static ResumeRepository CreateRepository(SqliteAppDbContextScope scope) =>
        new(scope.UnitOfWork);

    private static async Task SeedAsync(
        SqliteAppDbContextScope scope,
        CancellationToken cancellationToken,
        params Resume[] resumes)
    {
        scope.DbContext.AddRange(resumes);
        await scope.DbContext.SaveChangesAsync(new TestAudit(100), cancellationToken);
        scope.DbContext.ChangeTracker.Clear();
    }

    private static async Task SetUpdatedDateAsync(
        SqliteAppDbContextScope scope,
        int resumeId,
        DateTime updatedDate,
        CancellationToken cancellationToken) =>
        await scope.DbContext.Set<Resume>()
            .Where(resume => resume.Id == resumeId)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(resume => resume.App_UpdateDate, updatedDate),
                cancellationToken);

    private static async Task SearchIdsShouldBeAsync(
        ResumeRepository repository,
        ResumeSortBy sortBy,
        ResumeSortDirection sortDirection,
        int[] expectedIds,
        CancellationToken cancellationToken)
    {
        var result = await repository.SearchAsync(
            new ResumeSearchCriteria
            {
                SortBy = sortBy,
                SortDirection = sortDirection,
                PageSize = 10
            },
            cancellationToken);

        result.Items.Select(resume => resume.Id).ShouldBe(expectedIds);
    }
}
