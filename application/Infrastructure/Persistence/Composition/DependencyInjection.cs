using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ResumeEnhancer.Infrastructure.Persistence;

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
        services.TryAddScoped<IUnitOfWork<AppDbContext>, UnitOfWork<AppDbContext>>();
        services.TryAddScoped<IUnitOfWorkFactory<AppDbContext>, UnitOfWorkFactory<AppDbContext>>();
        services.TryAddScoped(typeof(IAuditEntityRepository<>), typeof(AuditEntityRepository<>));
        services.TryAddTransient(typeof(IModelLoader<>), typeof(ModelLoader<>));

        return services;
    }
}

