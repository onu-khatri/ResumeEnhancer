using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ResumeEnhancer.ResumeModule.SL;
using ResumeEnhancer.ResumeModule.Web.Validation.Resumes;

namespace ResumeEnhancer.ResumeModule.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddResumeModuleWeb(this IServiceCollection services)
    {
        services.AddResumeModuleApplication();
        services.AddValidatorsFromAssemblyContaining<CreateResumeRequestValidator>();

        return services;
    }

    public static Type[] GetResumeModuleMediatorAssemblies() =>
    [
        typeof(ResumeModuleWebAssembly),
        typeof(ResumeModuleSLAssembly)
    ];
}

