using Microsoft.AspNetCore.Http;
using ResumeEnhancer.Core.WebLibrary.Authorization;

namespace ResumeEnhancer.TemplateModule.Web.MiniApis;

internal static class TemplateEndpointHeaders
{
    public static int? GetPrincipalAuditUserId(HttpContext httpContext) =>
        httpContext.User.GetAuditActorId();
}
