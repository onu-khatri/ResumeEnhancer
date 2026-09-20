using Microsoft.AspNetCore.Http;
using ResumeEnhancer.Core.WebLibrary.Authorization;

namespace ResumeEnhancer.ProfilingModule.Web.MiniApis;

internal static class ProfilingEndpointHeaders
{
    public static int? GetPrincipalAuditUserId(HttpContext httpContext) =>
        httpContext.User.GetAuditActorId();
}
