using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ResumeEnhancer.ProfilingModule.SL;
using ResumeEnhancer.ProfilingModule.Web.Validation;

namespace ResumeEnhancer.ProfilingModule.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddProfilingModuleWeb(this IServiceCollection services)
    {
        services.AddProfilingModuleApplication();
        services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();

        return services;
    }

    public static Type[] GetProfilingModuleMediatorAssemblies() =>
    [
        typeof(ProfilingModuleWebAssembly),
        typeof(ProfilingModuleSLAssembly)
    ];
}
