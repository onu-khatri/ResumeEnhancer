using ResumeEnhancer.ProfilingModule.SL.Abstractions.Persistence;
using ResumeEnhancer.ProfilingModule.SL.Integrations;

namespace ResumeEnhancer.ProfilingModule.SL.Services;

internal sealed class ProfilingRegistrationService(IProfilingRepository repository)
    : IProfilingRegistrationService
{
    public async Task<ProfileRegistrationSnapshot> CreateRegistrationUserAsync(
        ProfileRegistrationInput input,
        CancellationToken cancellationToken = default
    )
    {
        var user = await repository.AddUserForRegistrationAsync(input, cancellationToken);
        return new ProfileRegistrationSnapshot(user.Id);
    }

    public Task AddRegistrationBaselineAsync(
        RegistrationBaselineInput input,
        CancellationToken cancellationToken = default
    )
    {
        return repository.AddRegistrationBaselineAsync(input, cancellationToken);
    }

    public Task<AccessShapeSnapshot> GetAccessShapeAsync(
        int accessProfileId,
        CancellationToken cancellationToken = default
    )
    {
        return repository.GetAccessShapeAsync(accessProfileId, cancellationToken);
    }

    public Task<AccessShapeSnapshot> GetStarterAccessShapeAsync(
        CancellationToken cancellationToken = default
    )
    {
        return repository.GetStarterAccessShapeAsync(cancellationToken);
    }

    public Task<StarterAccessProfileSnapshot?> GetStarterAccessProfileAsync(
        CancellationToken cancellationToken = default
    )
    {
        return repository.GetStarterAccessProfileAsync(cancellationToken);
    }
}
