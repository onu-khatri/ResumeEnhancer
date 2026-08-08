using Microsoft.EntityFrameworkCore;

namespace Persistence;

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
        CancellationToken cancellationToken = default)
    {
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
