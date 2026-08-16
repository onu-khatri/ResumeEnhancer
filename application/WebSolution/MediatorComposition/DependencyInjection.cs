using Mediator;
using Microsoft.Extensions.DependencyInjection;
using ResumeEnhancer.BillingModule.SL;
using ResumeEnhancer.BillingModule.Web;
using ResumeEnhancer.ProfilingModule.SL;
using ResumeEnhancer.ProfilingModule.Web;
using ResumeEnhancer.ResumeModule.SL;
using ResumeEnhancer.ResumeModule.Web;
using ResumeEnhancer.TemplateModule.SL;
using ResumeEnhancer.TemplateModule.Web;

namespace ResumeEnhancer.WebSolution.MediatorComposition;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationMediator(this IServiceCollection services)
    {
        services.AddMediator(options =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
            options.Assemblies =
            [
                typeof(ProfilingModuleWebAssembly),
                typeof(ProfilingModuleSLAssembly),
                typeof(BillingModuleWebAssembly),
                typeof(BillingModuleSLAssembly),
                typeof(TemplateModuleWebAssembly),
                typeof(TemplateModuleSLAssembly),
                typeof(ResumeModuleWebAssembly),
                typeof(ResumeModuleSLAssembly)
            ];
        });

        return services;
    }
}
