using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ResumeEnhancer.TemplateModule.SL;
using ResumeEnhancer.TemplateModule.Web.Validation;

namespace ResumeEnhancer.TemplateModule.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddTemplateModuleWeb(this IServiceCollection services)
    {
        services.AddTemplateModuleApplication();
        services.AddValidatorsFromAssemblyContaining<CreateTemplateRequestValidator>();

        return services;
    }

    public static Type[] GetTemplateModuleMediatorAssemblies() =>
    [
        typeof(TemplateModuleWebAssembly),
        typeof(TemplateModuleSLAssembly)
    ];
}
