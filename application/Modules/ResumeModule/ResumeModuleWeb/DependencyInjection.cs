using FluentValidation;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using ResumeEnhancer.ResumeModule.SL;
using ResumeEnhancer.ResumeModule.SL.Contracts;
using ResumeEnhancer.ResumeModule.Web.Validation.Resumes;

namespace ResumeEnhancer.ResumeModule.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddResumeModuleWeb(this IServiceCollection services)
    {
        services.AddResumeModuleApplication();
        services.AddMediator(options =>
        {
            options.Assemblies = [typeof(CreateResumeCommand)];
            options.ServiceLifetime = ServiceLifetime.Scoped;
        });
        services.AddValidatorsFromAssemblyContaining<CreateResumeRequestValidator>();

        return services;
    }
}

