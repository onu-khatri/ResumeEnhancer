using ResumeEnhancer.Core.DomainLibrary.DomainModel;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using ResumeEnhancer.Infrastructure.Caching;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.ProfilingModule.DM.Entities;
using ResumeEnhancer.ProfilingModule.PL;
using ResumeEnhancer.ResumeModule.PL;
using ResumeEnhancer.TemplateModule.DM.Entities;
using ResumeEnhancer.TemplateModule.PL;

namespace ResumeEnhancer.Tests.Unit.TestInfrastructure;

internal sealed class SqliteAppDbContextScope : IDisposable
{
    private readonly SqliteConnection _connection;

    public SqliteAppDbContextScope()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        var services = new ServiceCollection()
            .AddSingleton(dbContextOptions)
            .AddScoped<AppDbContext>(_ => new TestAppDbContext(dbContextOptions));

        services.TryAddScoped<IUnitOfWork<AppDbContext>, UnitOfWork<AppDbContext>>();
        services.TryAddScoped<IUnitOfWorkFactory<AppDbContext>, UnitOfWorkFactory<AppDbContext>>();
        services.TryAddScoped(typeof(IAuditEntityRepository<>), typeof(AuditEntityRepository<>));
        services.TryAddTransient(typeof(IModelLoader<>), typeof(ModelLoader<>));
        services.TryAddSingleton(CreateCacheProvider());
        services.AddResumeModulePersistence();

        Services = services.BuildServiceProvider();

        DbContext.Database.EnsureCreated();
        SeedReferenceData();
    }

    public ServiceProvider Services { get; }

    public AppDbContext DbContext => Services.GetRequiredService<AppDbContext>();

    public IUnitOfWork<AppDbContext> UnitOfWork =>
        Services.GetRequiredService<IUnitOfWork<AppDbContext>>();

    public void Dispose()
    {
        Services.Dispose();
        _connection.Dispose();
    }

    private static ICacheProvider CreateCacheProvider()
    {
        var cacheProvider = Substitute.For<ICacheProvider>();
        cacheProvider.RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        return cacheProvider;
    }

    private void SeedReferenceData()
    {
        if (DbContext.Set<User>().Any())
        {
            return;
        }

        DbContext.AddRange(
            ResumeTestData.User(
                ResumeTestData.UserId,
                email: $"user-{ResumeTestData.UserId}@example.com"),
            ResumeTestData.User(
                ResumeTestData.OtherUserId,
                email: $"user-{ResumeTestData.OtherUserId}@example.com"),
            ResumeTestData.TemplateCategory(),
            ResumeTestData.TemplateRenderType(),
            ResumeTestData.Template());

        DbContext.SaveChanges();
        DbContext.ChangeTracker.Clear();
    }

    private sealed class TestAppDbContext(DbContextOptions<AppDbContext> options)
        : AppDbContext(
            options,
            [
                new ProfilingModuleDbContextModelConfiguration(),
                new ResumeModuleDbContextModelConfiguration(),
                new TemplateModuleDbContextModelConfiguration()
            ])
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                         .Where(entityType => typeof(AuditEntity).IsAssignableFrom(entityType.ClrType)))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property<byte[]>(nameof(AuditEntity.App_Version))
                    .HasDefaultValue(new byte[] { 1 });
            }
        }
    }
}


