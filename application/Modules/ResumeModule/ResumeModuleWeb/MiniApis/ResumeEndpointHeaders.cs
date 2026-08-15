using Microsoft.AspNetCore.Http;
using ResumeEnhancer.Core.WebLibrary.Http;

namespace ResumeEnhancer.ResumeModule.Web.MiniApis;

internal static class ResumeEndpointHeaders
{
    private const string AuditUserIdHeader = "X-Audit-UserId";
    private const string UserIdHeader = "X-User-Id";

    public static string? ReadUserId(HttpContext httpContext) =>
        HttpRequestHeaderReader.ReadOptional(httpContext, UserIdHeader);

    public static int? ReadAuditUserId(HttpContext httpContext) =>
        HttpRequestHeaderReader.ReadOptionalInt32(httpContext, AuditUserIdHeader);
}

