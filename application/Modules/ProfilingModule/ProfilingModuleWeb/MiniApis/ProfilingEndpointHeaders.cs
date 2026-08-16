using Microsoft.AspNetCore.Http;
using ResumeEnhancer.Core.WebLibrary.Http;

namespace ResumeEnhancer.ProfilingModule.Web.MiniApis;

internal static class ProfilingEndpointHeaders
{
    private const string AuditUserIdHeader = "X-Audit-UserId";

    public static int? ReadAuditUserId(HttpContext httpContext) =>
        HttpRequestHeaderReader.ReadOptionalInt32(httpContext, AuditUserIdHeader);
}
