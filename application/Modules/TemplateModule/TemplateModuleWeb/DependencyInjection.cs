using FluentValidation;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using ResumeEnhancer.TemplateModule.SL;
using ResumeEnhancer.TemplateModule.SL.Contracts;
using ResumeEnhancer.TemplateModule.Web.Validation;

namespace ResumeEnhancer.TemplateModule.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddTemplateModuleWeb(this IServiceCollection services)
    {
        services.AddTemplateModuleApplication();
        services.AddMediator(options =>
        {
            options.Assemblies = [typeof(CreateTemplateCommand)];
            options.ServiceLifetime = ServiceLifetime.Scoped;
        });
        services.AddValidatorsFromAssemblyContaining<CreateTemplateRequestValidator>();

        return services;
    }
}
