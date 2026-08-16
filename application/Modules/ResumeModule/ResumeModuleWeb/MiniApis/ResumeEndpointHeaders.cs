using Microsoft.AspNetCore.Http;
using ResumeEnhancer.Core.WebLibrary.Http;

namespace ResumeEnhancer.ResumeModule.Web.MiniApis;

internal static class ResumeEndpointHeaders
{
    private const string AuditUserIdHeader = "X-Audit-UserId";
    private const string UserIdHeader = "X-User-Id";

    public static int? ReadUserId(HttpContext httpContext) =>
        HttpRequestHeaderReader.ReadOptionalInt32(httpContext, UserIdHeader);

    public static int? ReadAuditUserId(HttpContext httpContext) =>
        HttpRequestHeaderReader.ReadOptionalInt32(httpContext, AuditUserIdHeader);
}

