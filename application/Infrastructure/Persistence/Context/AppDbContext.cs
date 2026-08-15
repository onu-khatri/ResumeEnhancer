using System.ComponentModel.DataAnnotations;
using ResumeEnhancer.Core.DomainLibrary.DomainModel;
using Microsoft.EntityFrameworkCore;

namespace ResumeEnhancer.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    private const int MaxConcurrencyRetryCount = 3;

    private readonly IReadOnlyCollection<IAppDbContextModelConfiguration> _modelConfigurations;

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : this(options, [])
    {
    }

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        IEnumerable<IAppDbContextModelConfiguration> modelConfigurations)
        : base(options)
    {
        _modelConfigurations = modelConfigurations.ToArray();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var modelConfiguration in _modelConfigurations)
        {
            modelConfiguration.Configure(modelBuilder);
        }
    }

    public override async Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default) =>
        await SaveChangesWithPipelineAsync(
            auditUser: null,
            acceptAllChangesOnSuccess,
            cancellationToken);

    public async Task<int> SaveChangesAsync(
        IAudit auditUser,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(auditUser);

        return await SaveChangesWithPipelineAsync(
            auditUser,
            acceptAllChangesOnSuccess: true,
            cancellationToken);
    }

    public async Task<int> SaveChangesAsync(
        IAudit auditUser,
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(auditUser);

        return await SaveChangesWithPipelineAsync(
            auditUser,
            acceptAllChangesOnSuccess,
            cancellationToken);
    }

    private async Task<int> SaveChangesWithPipelineAsync(
        IAudit? auditUser,
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken)
    {
        ApplyAuditValues(auditUser);
        ValidateTrackedEntities();

        for (var retryCount = 0; ; retryCount++)
        {
            try
            {
                return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            }
            catch (DbUpdateConcurrencyException concurrencyException)
                when (retryCount < MaxConcurrencyRetryCount)
            {
                if (!await TryRefreshOriginalValuesAsync(concurrencyException, cancellationToken))
                {
                    throw;
                }
            }
        }
    }

    private void ApplyAuditValues(IAudit? auditUser)
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<IAuditEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.App_CreateDate = entry.Entity.App_CreateDate == default
                    ? utcNow
                    : entry.Entity.App_CreateDate;
                entry.Entity.App_UpdateDate = utcNow;

                if (auditUser?.UserId is { } createUserId)
                {
                    entry.Entity.App_CreateUserId ??= createUserId;
                    entry.Entity.App_UpdateUserId = createUserId;
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.App_UpdateDate = utcNow;

                if (auditUser?.UserId is { } updateUserId)
                {
                    entry.Entity.App_UpdateUserId = updateUserId;
                }

                entry.Property(nameof(IAuditEntity.App_CreateDate)).IsModified = false;
                entry.Property(nameof(IAuditEntity.App_CreateUserId)).IsModified = false;
            }
        }
    }

    private void ValidateTrackedEntities()
    {
        foreach (var entry in ChangeTracker.Entries()
                     .Where(entry => entry.State is EntityState.Added or EntityState.Modified))
        {
            var validationContext = new ValidationContext(entry.Entity);

            Validator.ValidateObject(
                entry.Entity,
                validationContext,
                validateAllProperties: true);
        }
    }

    private static async Task<bool> TryRefreshOriginalValuesAsync(
        DbUpdateConcurrencyException concurrencyException,
        CancellationToken cancellationToken)
    {
        foreach (var entry in concurrencyException.Entries)
        {
            var databaseValues = await entry.GetDatabaseValuesAsync(cancellationToken);

            if (databaseValues is null)
            {
                return false;
            }

            entry.OriginalValues.SetValues(databaseValues);
        }

        return true;
    }
}

