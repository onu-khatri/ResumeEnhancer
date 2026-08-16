using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ResumeEnhancer.ProfilingModule.Web.MiniApis;

public static class ProfilingMinimalApis
{
    public static IEndpointRouteBuilder MapProfilingModuleApis(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapUserApis();
        endpoints.MapRoleApis();
        endpoints.MapAccessProfileApis();

        return endpoints;
    }
}
