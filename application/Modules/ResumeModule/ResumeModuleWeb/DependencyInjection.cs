using Microsoft.Extensions.DependencyInjection;
using ResumeModuleSL;

namespace ResumeModuleWeb;

public static class DependencyInjection
{
    public static IServiceCollection AddResumeModuleWeb(this IServiceCollection services)
    {
        services.AddResumeModuleApplication();

        return services;
    }
}
