using System.Security.Claims;
using ResumeEnhancer.Core.DomainLibrary.DomainModel;

namespace ResumeEnhancer.TestUtilities.IntegrationSupport;

public sealed record TestAuthenticatedAccess(
    string UserId,
    int AuditUserId,
    int AccessProfileId,
    IReadOnlyCollection<string> Privileges);

public sealed class TestAuthenticatedEntity : AuditEntity
{
    public string ExternalUserId { get; set; } = string.Empty;

    public IReadOnlyCollection<string> Privileges { get; set; } = [];
}

internal sealed class TestAuthenticationState
{
    private readonly object _lock = new();
    private TestAuthenticatedAccess? _current;

    public string? UserId
    {
        get
        {
            lock (_lock)
            {
                return _current?.UserId;
            }
        }
    }

    public int? AuditUserId
    {
        get
        {
            lock (_lock)
            {
                return _current?.AuditUserId;
            }
        }
    }

    public void Set(TestAuthenticatedAccess access)
    {
        ArgumentNullException.ThrowIfNull(access);

        lock (_lock)
        {
            _current = access;
        }
    }

    public IReadOnlyList<Claim> CreateClaims()
    {
        lock (_lock)
        {
            if (_current is null)
            {
                return [];
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, _current.UserId),
                new(ClaimTypes.Name, _current.UserId),
                new("audit_user_id", _current.AuditUserId.ToString()),
                new("access_profile_id", _current.AccessProfileId.ToString())
            };

            claims.AddRange(_current.Privileges.Select(privilege => new Claim("privilege", privilege)));

            return claims;
        }
    }
}

