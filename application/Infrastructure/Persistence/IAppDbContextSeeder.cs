namespace Persistence;

public interface IAppDbContextSeeder
{
    Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default);
}
