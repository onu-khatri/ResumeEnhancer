using System.Collections.Frozen;
using ResumeEnhancer.ProfilingModule.SL.Integrations;

namespace ResumeEnhancer.TestUtilities.IntegrationSupport;

internal sealed class TestProfilingAuthorizationService(TestAuthenticationState state)
    : IProfilingAuthorizationService
{
    public Task<ProfilingAuthorizationSnapshot?> GetUserAuthorizationAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!int.TryParse(state.UserId, out var currentUserId) || currentUserId != userId)
        {
            return Task.FromResult<ProfilingAuthorizationSnapshot?>(null);
        }

        var privileges = state.Privileges.ToFrozenSet(StringComparer.Ordinal);
        return Task.FromResult<ProfilingAuthorizationSnapshot?>(new ProfilingAuthorizationSnapshot(
            userId,
            FrozenSet<string>.Empty,
            privileges,
            privileges));
    }

    public Task<ProfilingAuthorizationSnapshot?> GetGuestAuthorizationAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult<ProfilingAuthorizationSnapshot?>(new ProfilingAuthorizationSnapshot(
            0,
            new[] { "Guest" }.ToFrozenSet(StringComparer.Ordinal),
            new[] { "TemplateView" }.ToFrozenSet(StringComparer.Ordinal),
            new[] { "TemplateView" }.ToFrozenSet(StringComparer.Ordinal)));
    }
}
