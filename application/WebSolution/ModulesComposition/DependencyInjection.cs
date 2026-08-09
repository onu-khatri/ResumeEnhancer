using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using ResumeModulePL;
using ResumeModuleWeb;
using ResumeModuleWeb.MiniApis;

namespace ModulesComposition;

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
