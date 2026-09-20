using Microsoft.AspNetCore.Http;
using ResumeEnhancer.Core.WebLibrary.Authorization;

namespace ResumeEnhancer.BillingModule.Web.MiniApis;

internal static class BillingEndpointHeaders
{
    public static int? GetPrincipalAuditUserId(HttpContext httpContext) =>
        httpContext.User.GetAuditActorId();
}
