using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ResumeEnhancer.BillingModule.Web.MiniApis;

public static class BillingMinimalApis
{
    public static IEndpointRouteBuilder MapBillingModuleApis(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapBillingAccountApis();
        endpoints.MapBillingPlanApis();
        endpoints.MapBillingSubscriptionApis();

        return endpoints;
    }
}
