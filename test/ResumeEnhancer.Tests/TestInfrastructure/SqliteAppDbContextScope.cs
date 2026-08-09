using DomainLibrary.DomainModel;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Persistence;
using ResumeModulePL;

namespace ResumeEnhancer.Tests.TestInfrastructure;

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
        services.AddResumeModulePersistence();

        Services = services.BuildServiceProvider();

        DbContext.Database.EnsureCreated();
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

    private sealed class TestAppDbContext(DbContextOptions<AppDbContext> options)
        : AppDbContext(options, [new ResumeModuleDbContextModelConfiguration()])
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
