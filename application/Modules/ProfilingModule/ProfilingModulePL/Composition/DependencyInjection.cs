using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.ProfilingModule.PL.Repositories;
using ResumeEnhancer.ProfilingModule.PL.Seeding;
using ResumeEnhancer.ProfilingModule.SL.Abstractions.Persistence;

namespace ResumeEnhancer.ProfilingModule.PL;

public static class DependencyInjection
{
    public static IServiceCollection AddProfilingModulePersistence(this IServiceCollection services)
    {
        return services.AddProfilingModulePersistence(rootEntitySchema: null);
    }

    public static IServiceCollection AddProfilingModulePersistence(this IServiceCollection services, string? rootEntitySchema)
    {
        if (!services.Any(IsProfilingModuleModelConfigurationRegistered))
        {
            services.AddSingleton<IAppDbContextModelConfiguration>(new ProfilingModuleDbContextModelConfiguration(rootEntitySchema));
        }

        services.TryAddEnumerable(ServiceDescriptor.Scoped<IAppDbContextSeeder, ProfilingModuleSeeder>());
        services.TryAddScoped<IProfilingRepository, ProfilingRepository>();
        services.TryAddScoped<IProfilingSetupDataRepository, ProfilingSetupDataRepository>();

        return services;
    }

    private static bool IsProfilingModuleModelConfigurationRegistered(ServiceDescriptor serviceDescriptor) =>
        serviceDescriptor.ServiceType == typeof(IAppDbContextModelConfiguration)
        && (serviceDescriptor.ImplementationType == typeof(ProfilingModuleDbContextModelConfiguration)
            || serviceDescriptor.ImplementationInstance is ProfilingModuleDbContextModelConfiguration);
}
