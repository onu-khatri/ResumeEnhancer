using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ResumeEnhancer.TemplateModule.SL.Integrations;
using ResumeEnhancer.TemplateModule.SL.Services;

namespace ResumeEnhancer.TemplateModule.SL;

public static class DependencyInjection
{
    public static IServiceCollection AddTemplateModuleApplication(this IServiceCollection services)
    {
        services.TryAddScoped<ITemplateLookupService, TemplateLookupService>();

        return services;
    }
}
