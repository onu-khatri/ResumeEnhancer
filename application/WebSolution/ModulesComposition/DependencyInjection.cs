using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using ResumeEnhancer.ResumeModule.PL;
using ResumeEnhancer.ResumeModule.Web;
using ResumeEnhancer.ResumeModule.Web.MiniApis;

namespace ResumeEnhancer.WebSolution.ModulesComposition;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationModules(this IServiceCollection services)
    {
        services.AddResumeModulePersistence();
        services.AddResumeModuleWeb();

        return services;
    }

    public static IEndpointRouteBuilder MapApplicationModuleApis(this IEndpointRouteBuilder endpoints)
    {
        ResumeMinimalApis.MapResumeModuleApis(endpoints);

        return endpoints;
    }
}

