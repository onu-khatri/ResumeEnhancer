using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.TemplateModule.PL.Repositories;
using ResumeEnhancer.TemplateModule.PL.Seeding;
using ResumeEnhancer.TemplateModule.SL.Abstractions.Persistence;

namespace ResumeEnhancer.TemplateModule.PL;

public static class DependencyInjection
{
    public static IServiceCollection AddTemplateModulePersistence(this IServiceCollection services)
    {
        return services.AddTemplateModulePersistence(rootEntitySchema: null);
    }

    public static IServiceCollection AddTemplateModulePersistence(this IServiceCollection services, string? rootEntitySchema)
    {
        if (!services.Any(IsTemplateModuleModelConfigurationRegistered))
        {
            services.AddSingleton<IAppDbContextModelConfiguration>(new TemplateModuleDbContextModelConfiguration(rootEntitySchema));
        }

        services.TryAddEnumerable(ServiceDescriptor.Scoped<IAppDbContextSeeder, TemplateModuleSeeder>());
        services.TryAddScoped<ITemplateRepository, TemplateRepository>();
        services.TryAddScoped<ITemplateSetupDataRepository, TemplateSetupDataRepository>();

        return services;
    }

    private static bool IsTemplateModuleModelConfigurationRegistered(ServiceDescriptor serviceDescriptor) =>
        serviceDescriptor.ServiceType == typeof(IAppDbContextModelConfiguration)
        && (serviceDescriptor.ImplementationType == typeof(TemplateModuleDbContextModelConfiguration)
            || serviceDescriptor.ImplementationInstance is TemplateModuleDbContextModelConfiguration);
}
