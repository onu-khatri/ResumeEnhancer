using Microsoft.AspNetCore.Http;
using WebLibrary.Http;

namespace ResumeModuleWeb.MiniApis;

internal static class ResumeEndpointHeaders
{
    private const string AuditUserIdHeader = "X-Audit-UserId";
    private const string UserIdHeader = "X-User-Id";

    public static string? ReadUserId(HttpContext httpContext) =>
        HttpRequestHeaderReader.ReadOptional(httpContext, UserIdHeader);

    public static int? ReadAuditUserId(HttpContext httpContext) =>
        HttpRequestHeaderReader.ReadOptionalInt32(httpContext, AuditUserIdHeader);
}
