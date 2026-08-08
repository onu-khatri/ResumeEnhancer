using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Persistence;
using ResumeModulePL.Seeding;

namespace ResumeModulePL;

public static class DependencyInjection
{
    public static IServiceCollection AddResumeModulePersistence(this IServiceCollection services)
    {
        return services.AddResumeModulePersistence(rootEntitySchema: null);
    }

    public static IServiceCollection AddResumeModulePersistence(
        this IServiceCollection services,
        string? rootEntitySchema)
    {
        if (!services.Any(IsResumeModuleModelConfigurationRegistered))
        {
            services.AddSingleton<IAppDbContextModelConfiguration>(
                new ResumeModuleDbContextModelConfiguration(rootEntitySchema));
        }

        services.TryAddEnumerable(
            ServiceDescriptor.Scoped<IAppDbContextSeeder, ResumeModuleSeeder>());

        return services;
    }

    private static bool IsResumeModuleModelConfigurationRegistered(ServiceDescriptor serviceDescriptor) =>
        serviceDescriptor.ServiceType == typeof(IAppDbContextModelConfiguration)
        && (serviceDescriptor.ImplementationType == typeof(ResumeModuleDbContextModelConfiguration)
            || serviceDescriptor.ImplementationInstance is ResumeModuleDbContextModelConfiguration);
}
