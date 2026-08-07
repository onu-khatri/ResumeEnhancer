using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class AppDbContext : DbContext
{
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
}
