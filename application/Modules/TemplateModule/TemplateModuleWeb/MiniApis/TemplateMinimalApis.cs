using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ResumeEnhancer.TemplateModule.Web.MiniApis;

public static class TemplateMinimalApis
{
    public static IEndpointRouteBuilder MapTemplateModuleApis(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapTemplateCategoryApis();
        endpoints.MapTemplateApis();

        return endpoints;
    }
}
