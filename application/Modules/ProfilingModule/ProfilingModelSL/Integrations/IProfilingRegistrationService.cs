namespace ResumeEnhancer.ProfilingModule.SL.Integrations;

public sealed record ProfileRegistrationInput(string FirstName, string LastName, string Email);

public sealed record ProfileRegistrationSnapshot(int UserId);

public sealed record RegistrationBaselineInput(
    int UserId,
    int BillingSubscriptionId,
    int AccessProfileId
);

public sealed record AccessShapeSnapshot(IReadOnlySet<string> Capabilities);

public sealed record StarterAccessProfileSnapshot(int AccessProfileId);

public interface IProfilingRegistrationService
{
    public Task<ProfileRegistrationSnapshot> CreateRegistrationUserAsync(
        ProfileRegistrationInput input,
        CancellationToken cancellationToken = default
    );
    public Task AddRegistrationBaselineAsync(
        RegistrationBaselineInput input,
        CancellationToken cancellationToken = default
    );
    public Task<AccessShapeSnapshot> GetAccessShapeAsync(
        int accessProfileId,
        CancellationToken cancellationToken = default
    );
    public Task<AccessShapeSnapshot> GetStarterAccessShapeAsync(
        CancellationToken cancellationToken = default
    );
    public Task<StarterAccessProfileSnapshot?> GetStarterAccessProfileAsync(
        CancellationToken cancellationToken = default
    );
}
