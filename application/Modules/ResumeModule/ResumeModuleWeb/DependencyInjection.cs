using FluentValidation;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using ResumeModuleSL;
using ResumeModuleSL.Contracts;
using ResumeModuleWeb.Validation.Resumes;

namespace ResumeModuleWeb;

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
