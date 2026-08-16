using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ResumeEnhancer.ResumeModule.SL.Integrations;
using ResumeEnhancer.ResumeModule.SL.Services;

namespace ResumeEnhancer.ResumeModule.SL;

public static class DependencyInjection
{
    public static IServiceCollection AddResumeModuleApplication(this IServiceCollection services)
    {
        services.TryAddScoped<IResumeLookupService, ResumeLookupService>();

        return services;
    }
}

