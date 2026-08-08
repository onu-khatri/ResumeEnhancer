using Microsoft.Extensions.DependencyInjection;
using ResumeModulePL;

namespace ResumeModuleSL;

public static class DependencyInjection
{
    public static IServiceCollection AddResumeModuleApplication(this IServiceCollection services)
    {
        services.AddResumeModulePersistence();

        return services;
    }
}
