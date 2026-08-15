using Microsoft.Extensions.DependencyInjection;

namespace ResumeEnhancer.Infrastructure.Persistence;

public static class AppDbContextSeederExtensions
{
    public static async Task SeedAppDbContextAsync(
        this IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var seeders = scope.ServiceProvider.GetServices<IAppDbContextSeeder>();

        foreach (var seeder in seeders)
        {
            await seeder.SeedAsync(dbContext, cancellationToken);
        }
    }
}

