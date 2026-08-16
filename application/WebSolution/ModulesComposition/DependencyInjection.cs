using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using ResumeEnhancer.BillingModule.PL;
using ResumeEnhancer.BillingModule.Web;
using ResumeEnhancer.BillingModule.Web.MiniApis;
using ResumeEnhancer.ProfilingModule.PL;
using ResumeEnhancer.ProfilingModule.Web;
using ResumeEnhancer.ProfilingModule.Web.MiniApis;
using ResumeEnhancer.ResumeModule.PL;
using ResumeEnhancer.ResumeModule.Web;
using ResumeEnhancer.ResumeModule.Web.MiniApis;
using ResumeEnhancer.TemplateModule.PL;
using ResumeEnhancer.TemplateModule.Web;
using ResumeEnhancer.TemplateModule.Web.MiniApis;
using ResumeEnhancer.WebSolution.MediatorComposition;

namespace ResumeEnhancer.WebSolution.ModulesComposition;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationModules(this IServiceCollection services)
    {
        services.AddApplicationMediator();

        services.AddProfilingModulePersistence();
        services.AddProfilingModuleWeb();
        services.AddBillingModulePersistence();
        services.AddBillingModuleWeb();
        services.AddTemplateModulePersistence();
        services.AddTemplateModuleWeb();
        services.AddResumeModulePersistence();
        services.AddResumeModuleWeb();

        return services;
    }

    public static IEndpointRouteBuilder MapApplicationModuleApis(this IEndpointRouteBuilder endpoints)
    {
        ProfilingMinimalApis.MapProfilingModuleApis(endpoints);
        BillingMinimalApis.MapBillingModuleApis(endpoints);
        TemplateMinimalApis.MapTemplateModuleApis(endpoints);
        ResumeMinimalApis.MapResumeModuleApis(endpoints);

        return endpoints;
    }
}

