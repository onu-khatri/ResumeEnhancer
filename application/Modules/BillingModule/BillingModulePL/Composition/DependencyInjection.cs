using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ResumeEnhancer.BillingModule.PL.Repositories;
using ResumeEnhancer.BillingModule.PL.Seeding;
using ResumeEnhancer.BillingModule.SL.Abstractions.Persistence;
using ResumeEnhancer.Infrastructure.Persistence;

namespace ResumeEnhancer.BillingModule.PL;

public static class DependencyInjection
{
    public static IServiceCollection AddBillingModulePersistence(this IServiceCollection services)
    {
        return services.AddBillingModulePersistence(rootEntitySchema: null);
    }

    public static IServiceCollection AddBillingModulePersistence(this IServiceCollection services, string? rootEntitySchema)
    {
        if (!services.Any(IsBillingModuleModelConfigurationRegistered))
        {
            services.AddSingleton<IAppDbContextModelConfiguration>(new BillingModuleDbContextModelConfiguration(rootEntitySchema));
        }

        services.TryAddEnumerable(ServiceDescriptor.Scoped<IAppDbContextSeeder, BillingModuleSeeder>());
        services.TryAddScoped<IBillingRepository, BillingRepository>();
        services.TryAddScoped<IBillingSetupDataRepository, BillingSetupDataRepository>();

        return services;
    }

    private static bool IsBillingModuleModelConfigurationRegistered(ServiceDescriptor serviceDescriptor) =>
        serviceDescriptor.ServiceType == typeof(IAppDbContextModelConfiguration)
        && (serviceDescriptor.ImplementationType == typeof(BillingModuleDbContextModelConfiguration)
            || serviceDescriptor.ImplementationInstance is BillingModuleDbContextModelConfiguration);
}
