using System.Collections.Frozen;

namespace ResumeEnhancer.ProfilingModule.SL.Integrations;

public sealed record ProfilingUserStateSnapshot(int UserId, bool IsDeactivated, bool IsDeleted);

public sealed record ProfilingAuthorizationSnapshot(
    int UserId,
    IReadOnlySet<string> AccessProfileCodes,
    IReadOnlySet<string> RoleCodes,
    IReadOnlySet<string> Capabilities,
    bool IsDeactivated = false,
    bool IsDeleted = false
)
{
    public static ProfilingAuthorizationSnapshot Empty(int userId) =>
        new(
            userId,
            FrozenSet<string>.Empty,
            FrozenSet<string>.Empty,
            FrozenSet<string>.Empty
        );
}

public interface IProfilingAuthorizationService
{
    Task<ProfilingAuthorizationSnapshot?> GetUserAuthorizationAsync(
        int userId,
        CancellationToken cancellationToken = default
    );

    Task<ProfilingAuthorizationSnapshot?> GetGuestAuthorizationAsync(
        CancellationToken cancellationToken = default
    );
}
