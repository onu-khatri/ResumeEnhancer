using ResumeEnhancer.ProfilingModule.SL.Abstractions.Persistence;
using ResumeEnhancer.ProfilingModule.SL.Integrations;

namespace ResumeEnhancer.ProfilingModule.SL.Services;

internal sealed class UserLookupService : IUserLookupService
{
    private readonly IProfilingRepository _repository;

    public UserLookupService(IProfilingRepository repository) => _repository = repository;

    public Task<bool> UserExistsAsync(int userId, CancellationToken cancellationToken = default) =>
        _repository.UserExistsAsync(userId, cancellationToken);
}
