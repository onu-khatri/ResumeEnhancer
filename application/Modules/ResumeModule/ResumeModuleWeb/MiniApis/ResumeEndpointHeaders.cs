using Microsoft.AspNetCore.Http;
using ResumeEnhancer.Core.WebLibrary.Authorization;

namespace ResumeEnhancer.ResumeModule.Web.MiniApis;

internal static class ResumeEndpointHeaders
{
    public static int? GetPrincipalUserId(HttpContext httpContext) =>
        httpContext.User.GetSubjectId();

    public static int? GetPrincipalAuditUserId(HttpContext httpContext) =>
        httpContext.User.GetAuditActorId();
}

