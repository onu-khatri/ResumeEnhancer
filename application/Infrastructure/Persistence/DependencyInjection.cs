using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddAppDbContext(
        this IServiceCollection services,
        Action<IServiceProvider, DbContextOptionsBuilder> configureOptions)
    {
        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            configureOptions(serviceProvider, options);
        });

        return services;
    }
}
