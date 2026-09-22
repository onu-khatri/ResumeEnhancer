using ResumeEnhancer.ProfilingModule.SL.Abstractions.Persistence;
using ResumeEnhancer.ProfilingModule.SL.Integrations;

namespace ResumeEnhancer.ProfilingModule.SL.Services;

internal sealed class ProfilingAuthorizationService(IProfilingRepository repository)
    : IProfilingAuthorizationService
{
    public Task<ProfilingAuthorizationSnapshot?> GetUserAuthorizationAsync(
        int userId,
        CancellationToken cancellationToken = default
    ) => repository.GetUserAuthorizationAsync(userId, cancellationToken);

    public Task<ProfilingAuthorizationSnapshot?> GetGuestAuthorizationAsync(
        CancellationToken cancellationToken = default
    ) => repository.GetGuestAuthorizationAsync(cancellationToken);
}
