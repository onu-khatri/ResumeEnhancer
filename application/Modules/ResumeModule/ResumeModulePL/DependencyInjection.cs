using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Persistence;
using ResumeModulePL.Seeding;

namespace ResumeModulePL;

public static class DependencyInjection
{
    public static IServiceCollection AddResumeModulePersistence(this IServiceCollection services)
    {
        services.TryAddEnumerable(
            ServiceDescriptor.Singleton<IAppDbContextModelConfiguration, ResumeModuleDbContextModelConfiguration>());

        services.TryAddEnumerable(
            ServiceDescriptor.Scoped<IAppDbContextSeeder, ResumeModuleSeeder>());

        return services;
    }
}
