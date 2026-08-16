using FluentValidation;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using ResumeEnhancer.ProfilingModule.SL;
using ResumeEnhancer.ProfilingModule.SL.Contracts;
using ResumeEnhancer.ProfilingModule.Web.Validation;

namespace ResumeEnhancer.ProfilingModule.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddProfilingModuleWeb(this IServiceCollection services)
    {
        services.AddProfilingModuleApplication();
        services.AddMediator(options =>
        {
            options.Assemblies = [typeof(CreateUserCommand)];
            options.ServiceLifetime = ServiceLifetime.Scoped;
        });
        services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();

        return services;
    }
}
