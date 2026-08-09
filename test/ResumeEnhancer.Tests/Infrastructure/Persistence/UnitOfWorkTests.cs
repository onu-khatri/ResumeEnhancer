using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using DomainLibrary.DomainModel;
using Persistence;
using ResumeEnhancer.Tests.TestInfrastructure;
using ResumeModuleDM.Entities;
using ResumeModulePL.Repositories;

namespace ResumeEnhancer.Tests.Infrastructure.Persistence;

public sealed class UnitOfWorkTests
{
    [Fact]
    public void GetRepo_RegisteredRepository_ReturnsCachedInstance()
    {
        using var scope = new SqliteAppDbContextScope();

        var first = scope.UnitOfWork.GetRepo<Resume>();
        var second = scope.UnitOfWork.GetRepo<Resume>();

        first.ShouldBeSameAs(second);
    }

    [Fact]
    public void GetRepoLight_ConcreteTypeNotRegistered_CreatesWithActivatorUtilities()
    {
        using var scope = new SqliteAppDbContextScope();

        var repository = scope.UnitOfWork.GetRepoLight<ResumeRepository>();

        repository.ShouldNotBeNull();
    }

    [Fact]
    public void GetRepoLight_InterfaceNotRegistered_ThrowsInvalidOperationException()
    {
        using var scope = new SqliteAppDbContextScope();

        Should.Throw<InvalidOperationException>(() => scope.UnitOfWork.GetRepoLight<IDisposable>());
    }

    [Fact]
    public async Task CreateTransactionAsync_RelationalProviderWithoutCurrentTransaction_ReturnsRelationalTransaction()
    {
        using var scope = new SqliteAppDbContextScope();

        await using var transaction = await scope.UnitOfWork.CreateTransactionAsync(
            TestContext.Current.CancellationToken);

        transaction.ShouldBeOfType<RelationalDbTransaction>();
    }

    [Fact]
    public async Task CreateTransactionAsync_CurrentTransactionExists_ReturnsNestedTransaction()
    {
        using var scope = new SqliteAppDbContextScope();
        await using var currentTransaction = await scope.DbContext.Database.BeginTransactionAsync(
            TestContext.Current.CancellationToken);

        await using var transaction = await scope.UnitOfWork.CreateTransactionAsync(
            TestContext.Current.CancellationToken);

        transaction.ShouldBeOfType<NestedDbTransaction>();
    }

    [Fact]
    public async Task SaveAsync_WithAndWithoutAudit_DelegatesToDbContext()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        scope.DbContext.Add(new Resume { Title = "One", UserId = ResumeTestData.UserId });

        await scope.UnitOfWork.SaveAsync(new TestAudit(4), cancellationToken);
        scope.DbContext.Add(new Resume { Title = "Two", UserId = ResumeTestData.UserId });
        await scope.UnitOfWork.SaveAsync(cancellationToken);

        (await scope.DbContext.Set<Resume>().CountAsync(cancellationToken)).ShouldBe(2);
    }

    [Fact]
    public void PreloadSetupEntities_AttachesDetachedSetupEntities()
    {
        using var scope = new SqliteAppDbContextScope();
        var setup = new ResumeSectionSetup
        {
            Id = 123,
            Code = "Education",
            Description = "Education",
            Guid = Guid.NewGuid()
        };

        scope.UnitOfWork.PreloadSetupEntities(setup);

        scope.DbContext.Entry(setup).State.ShouldBe(EntityState.Unchanged);
    }

    [Fact]
    public void PreloadSetupEntities_NullArguments_ThrowArgumentNullException()
    {
        using var scope = new SqliteAppDbContextScope();

        Should.Throw<ArgumentNullException>(() => scope.UnitOfWork.PreloadSetupEntities((ISetupData[])null!));
        Should.Throw<ArgumentNullException>(() => scope.UnitOfWork.PreloadSetupEntities((IEnumerable<ISetupData>)null!));
        Should.Throw<ArgumentNullException>(() => scope.UnitOfWork.PreloadSetupEntities(new ISetupData?[] { null! }!));
    }

    [Fact]
    public void DisposedUnitOfWork_PublicMembersThrowObjectDisposedException()
    {
        using var scope = new SqliteAppDbContextScope();
        scope.UnitOfWork.Dispose();

        Should.Throw<ObjectDisposedException>(() => scope.UnitOfWork.GetRepo<Resume>());
        Should.Throw<ObjectDisposedException>(() => scope.UnitOfWork.PreloadSetupEntities([]));
    }

    [Fact]
    public void UnitOfWorkFactory_CurrentAndCreateScope_ReturnRegisteredUnitOfWork()
    {
        var services = new ServiceCollection();
        services.AddAppDbContext((_, options) => options.UseSqlite("Data Source=:memory:"));
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var factory = scope.ServiceProvider.GetRequiredService<IUnitOfWorkFactory<AppDbContext>>();

        factory.Current.ShouldNotBeNull();
        using var unitOfWorkScope = factory.CreateScope();

        unitOfWorkScope.DbContext.ShouldNotBeNull();
        unitOfWorkScope.UnitOfWork.ShouldNotBeNull();
        unitOfWorkScope.ServiceProvider.ShouldNotBeNull();
    }

    [Fact]
    public async Task UnitOfWorkScope_DisposeAndDisposeAsync_DelegateToInnerServiceScope()
    {
        using var dbScope = new SqliteAppDbContextScope();
        var serviceProvider = Substitute.For<IServiceProvider>();
        var serviceScope = Substitute.For<IServiceScope>();
        serviceScope.ServiceProvider.Returns(serviceProvider);
        var unitOfWorkScope = new UnitOfWorkScope<AppDbContext>(
            serviceScope,
            dbScope.DbContext,
            dbScope.UnitOfWork);

        unitOfWorkScope.ServiceProvider.ShouldBeSameAs(serviceProvider);
        unitOfWorkScope.DbContext.ShouldBeSameAs(dbScope.DbContext);
        unitOfWorkScope.UnitOfWork.ShouldBeSameAs(dbScope.UnitOfWork);
        unitOfWorkScope.Dispose();
        await unitOfWorkScope.DisposeAsync();

        serviceScope.Received(2).Dispose();
    }
}
