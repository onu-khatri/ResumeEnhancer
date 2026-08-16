using Microsoft.AspNetCore.Http;
using ResumeEnhancer.Core.WebLibrary.Http;

namespace ResumeEnhancer.BillingModule.Web.MiniApis;

internal static class BillingEndpointHeaders
{
    private const string AuditUserIdHeader = "X-Audit-UserId";

    public static int? ReadAuditUserId(HttpContext httpContext) =>
        HttpRequestHeaderReader.ReadOptionalInt32(httpContext, AuditUserIdHeader);
}
