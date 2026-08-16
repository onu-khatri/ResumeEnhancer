using Microsoft.AspNetCore.Http;
using ResumeEnhancer.Core.WebLibrary.Http;

namespace ResumeEnhancer.TemplateModule.Web.MiniApis;

internal static class TemplateEndpointHeaders
{
    private const string AuditUserIdHeader = "X-Audit-UserId";

    public static int? ReadAuditUserId(HttpContext httpContext) =>
        HttpRequestHeaderReader.ReadOptionalInt32(httpContext, AuditUserIdHeader);
}
