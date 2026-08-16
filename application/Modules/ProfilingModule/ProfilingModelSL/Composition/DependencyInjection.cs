using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ResumeEnhancer.ProfilingModule.SL.Integrations;
using ResumeEnhancer.ProfilingModule.SL.Services;

namespace ResumeEnhancer.ProfilingModule.SL;

public static class DependencyInjection
{
    public static IServiceCollection AddProfilingModuleApplication(this IServiceCollection services)
    {
        services.TryAddScoped<IUserLookupService, UserLookupService>();

        return services;
    }
}
