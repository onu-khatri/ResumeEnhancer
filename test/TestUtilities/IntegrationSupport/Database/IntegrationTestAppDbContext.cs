using DomainLibrary.DomainModel;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace ResumeEnhancer.TestUtilities.IntegrationSupport;

internal sealed class IntegrationTestAppDbContext(
    DbContextOptions<AppDbContext> options,
    IEnumerable<IAppDbContextModelConfiguration> modelConfigurations)
    : AppDbContext(options, modelConfigurations)
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
