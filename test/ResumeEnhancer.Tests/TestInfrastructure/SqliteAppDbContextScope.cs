using ResumeEnhancer.Core.DomainLibrary.DomainModel;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using ResumeEnhancer.Infrastructure.Caching;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.ProfilingModule.DM.Entities;
using ResumeEnhancer.ProfilingModule.DM.Enums;
using ResumeEnhancer.ProfilingModule.PL;
using ResumeEnhancer.BillingModule.PL;
using ResumeEnhancer.BillingModule.DM.Entities;
using ResumeEnhancer.ResumeModule.PL;
using ResumeEnhancer.TemplateModule.DM.Entities;
using ResumeEnhancer.TemplateModule.PL;
using ResumeEnhancer.AuthModule.PL;
using ResumeEnhancer.AuthModule.DM.Entities;
using ResumeEnhancer.AuthModule.SL.Options;

namespace ResumeEnhancer.Tests.Unit.TestInfrastructure;

internal sealed class SqliteAppDbContextScope : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly string _connectionString;

    public SqliteAppDbContextScope(string? connectionString = null)
    {
        _connectionString = connectionString ?? $"Data Source=AuthTests-{Guid.NewGuid():N};Mode=Memory;Cache=Shared;Default Timeout=30";
        _connection = new SqliteConnection(_connectionString);
        _connection.Open();

        ConnectionString = _connectionString;

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
        services.AddDataProtection();
        services.AddSingleton(new AuthSecurityOptions());
        services.AddAuthModulePersistence();

        Services = services.BuildServiceProvider();

        DbContext.Database.EnsureCreated();
        SeedReferenceData();
    }

    public ServiceProvider Services { get; }

    public string ConnectionString { get; }

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
            ResumeTestData.UserAddressType(ResumeTestData.BillingAddressTypeId, nameof(UserAddressType.Billing), 1),
            ResumeTestData.UserAddressType(ResumeTestData.CommunicationAddressTypeId, nameof(UserAddressType.Communication), 2),
            ResumeTestData.Role(),
            ResumeTestData.AccessProfile(),
            new AccessProfileSource
            {
                Id = 1,
                Code = "plan",
                Description = "Billing plan",
                DisplayName = "Billing plan",
                Order = 1,
                Guid = Guid.NewGuid()
            },
            new AccessProfileSource
            {
                Id = 2,
                Code = "admin",
                Description = "Administrator",
                DisplayName = "Administrator",
                Order = 2,
                Guid = Guid.NewGuid()
            },
            new Currency
            {
                Id = 1,
                Code = "USD",
                Description = "United States dollar",
                DisplayName = "US Dollar",
                Order = 1,
                Guid = Guid.NewGuid()
            },
            new BillingInterval
            {
                Id = 1,
                Code = "Monthly",
                Description = "Monthly",
                DisplayName = "Monthly",
                Order = 1,
                Guid = Guid.NewGuid()
            },
            new BillingAccountStatus
            {
                Id = 1,
                Code = "Active",
                Description = "Active",
                DisplayName = "Active",
                Order = 1,
                Guid = Guid.NewGuid()
            },
            new BillingSubscriptionStatus
            {
                Id = 1,
                Code = "Active",
                Description = "Active",
                DisplayName = "Active",
                Order = 1,
                Guid = Guid.NewGuid()
            },
            new AuthChallengePurpose
            {
                Id = 1,
                Code = "password-reset",
                Description = "Password reset challenge",
                Guid = Guid.NewGuid()
            },
            new AuthChallengePurpose
            {
                Id = 2,
                Code = "email-verification",
                Description = "Email verification challenge",
                Guid = Guid.NewGuid()
            });

        DbContext.SaveChanges();
        DbContext.AddRange(
            ResumeTestData.BillingPlan(),
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
                new BillingModuleDbContextModelConfiguration(),
                new AuthModuleDbContextModelConfiguration(),
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


