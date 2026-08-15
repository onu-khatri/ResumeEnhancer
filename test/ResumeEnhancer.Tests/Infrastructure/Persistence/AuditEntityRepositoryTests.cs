using Microsoft.EntityFrameworkCore;
using Shouldly;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.Tests.Unit.TestInfrastructure;
using ResumeEnhancer.ResumeModule.DM.Entities;

namespace ResumeEnhancer.Tests.Unit.Infrastructure.Persistence;

public sealed class AuditEntityRepositoryTests
{
    [Fact]
    public async Task AddFindExistsAndQuery_MethodsOperateOnSet()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new AuditEntityRepository<Resume>(scope.DbContext);
        var resume = new Resume { Title = "Title", UserId = ResumeTestData.UserId };

        await repository.AddAsync(resume, cancellationToken);
        await scope.DbContext.SaveChangesAsync(new TestAudit(1), cancellationToken);

        (await repository.FindAsync(resume.Id, cancellationToken)).ShouldBeSameAs(resume);
        (await repository.ExistsAsync(resume.Id, cancellationToken)).ShouldBeTrue();
        (await repository.ExistsAsync(item => item.Title == "Title", cancellationToken))
            .ShouldBeTrue();
        repository.GetQuery(resume.Id).Single().ShouldBeSameAs(resume);
        repository.GetQuery([resume.Id, resume.Id]).Single().ShouldBeSameAs(resume);
        repository.GetQuery([]).ShouldBeEmpty();
    }

    [Fact]
    public async Task ExistsAsync_IdCollection_HandlesEmptyDuplicateAndMissingIds()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new AuditEntityRepository<Resume>(scope.DbContext);
        scope.DbContext.AddRange(
            new Resume { Title = "One", UserId = ResumeTestData.UserId },
            new Resume { Title = "Two", UserId = ResumeTestData.UserId });
        await scope.DbContext.SaveChangesAsync(new TestAudit(1), cancellationToken);
        var ids = await scope.DbContext.Set<Resume>()
            .OrderBy(resume => resume.Id)
            .Select(resume => resume.Id)
            .ToArrayAsync(cancellationToken);

        (await repository.ExistsAsync(Array.Empty<int>(), cancellationToken)).ShouldBeTrue();
        (await repository.ExistsAsync([ids[0], ids[0], ids[1]], cancellationToken)).ShouldBeTrue();
        (await repository.ExistsAsync([ids[0], 999], cancellationToken)).ShouldBeFalse();
    }

    [Fact]
    public async Task FindAsync_WithPagingAndLoader_ReturnsFilteredPageWithIncludes()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        scope.DbContext.AddRange(
            ResumeTestData.ResumeGraph(id: 1, title: "A"),
            ResumeTestData.ResumeGraph(id: 2, title: "B"),
            ResumeTestData.ResumeGraph(id: 3, title: "C", userId: ResumeTestData.OtherUserId));
        await scope.DbContext.SaveChangesAsync(new TestAudit(1), cancellationToken);
        scope.DbContext.ChangeTracker.Clear();
        var repository = new AuditEntityRepository<Resume>(scope.DbContext);
        var loader = new ModelLoader<Resume>()
            .Build(navigator => navigator.Load(resume => resume.Education));

        var page = await repository.FindAsync(
            pageNumber: 1,
            pageSize: 1,
            filter: resume => resume.UserId == ResumeTestData.UserId,
            modelLoader: loader,
            cancellationToken);

        page.TotalCount.ShouldBe(2);
        page.Items.ShouldHaveSingleItem().Education.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task FindAsync_InvalidPaging_ThrowsArgumentOutOfRangeException()
    {
        using var scope = new SqliteAppDbContextScope();
        var repository = new AuditEntityRepository<Resume>(scope.DbContext);

        await Should.ThrowAsync<ArgumentOutOfRangeException>(
            () => repository.FindAsync(0, 10, null, cancellationToken: TestContext.Current.CancellationToken));
        await Should.ThrowAsync<ArgumentOutOfRangeException>(
            () => repository.FindAsync(1, 0, null, cancellationToken: TestContext.Current.CancellationToken));
        await Should.ThrowAsync<ArgumentOutOfRangeException>(
            () => repository.FindAsync(1, AuditEntityRepository<Resume>.MaxPageSize + 1, null, cancellationToken: TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task UpdateDeleteAttachDetachAndSpecification_MethodsChangeEntityState()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new AuditEntityRepository<Resume>(scope.DbContext);
        var resume = new Resume { Title = "Title", UserId = ResumeTestData.UserId };
        await repository.AddAsync(resume, cancellationToken);
        await scope.DbContext.SaveChangesAsync(new TestAudit(1), cancellationToken);

        resume.Title = "Updated";
        repository.Update(resume);
        await scope.DbContext.SaveChangesAsync(new TestAudit(2), cancellationToken);
        repository.Detach(resume);
        scope.DbContext.Entry(resume).State.ShouldBe(EntityState.Detached);
        repository.Attach(resume);
        scope.DbContext.Entry(resume).State.ShouldNotBe(EntityState.Detached);
        repository.FindBySpecification(new TitleSpecification("Updated")).Single().Id.ShouldBe(resume.Id);
        repository.Delete(resume);
        await scope.DbContext.SaveChangesAsync(new TestAudit(3), cancellationToken);

        (await repository.ExistsAsync(resume.Id, cancellationToken)).ShouldBeFalse();
    }

    [Fact]
    public void NullArguments_ThrowArgumentNullException()
    {
        using var scope = new SqliteAppDbContextScope();
        var repository = new AuditEntityRepository<Resume>(scope.DbContext);

        Should.Throw<ArgumentNullException>(() => repository.GetQuery(null!));
        Should.Throw<ArgumentNullException>(() => repository.Update((Resume)null!));
        Should.Throw<ArgumentNullException>(() => repository.Update((IList<Resume>)null!));
        Should.Throw<ArgumentNullException>(() => repository.Delete((Resume)null!));
        Should.Throw<ArgumentNullException>(() => repository.Delete((IList<Resume>)null!));
        Should.Throw<ArgumentNullException>(() => repository.DeleteWhere(null!));
        Should.Throw<ArgumentNullException>(() => repository.Attach(null!));
        Should.Throw<ArgumentNullException>(() => repository.Detach((Resume)null!));
        Should.Throw<ArgumentNullException>(() => repository.Detach((IList<Resume>)null!));
        Should.Throw<ArgumentNullException>(() => repository.FindBySpecification(null!));
    }

    private sealed class TitleSpecification : QuerySpecification<Resume>
    {
        public TitleSpecification(string title)
        {
            Criteria = resume => resume.Title == title;
        }
    }
}


