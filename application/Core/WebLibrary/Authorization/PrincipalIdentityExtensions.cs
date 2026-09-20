using System.Security.Claims;

namespace ResumeEnhancer.Core.WebLibrary.Authorization;

public static class PrincipalIdentityExtensions
{
    public static int? GetSubjectId(this ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);

        var value = principal.FindFirst("sub")?.Value
            ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return int.TryParse(
            value,
            System.Globalization.NumberStyles.None,
            System.Globalization.CultureInfo.InvariantCulture,
            out var subjectId) && subjectId > 0
            ? subjectId
            : null;
    }

    public static int? GetAuditActorId(this ClaimsPrincipal principal) =>
        principal.GetSubjectId();
}
