namespace ResumeEnhancer.ProfilingModule.SL.Integrations;

public interface IUserLookupService
{
    Task<bool> UserExistsAsync(int userId, CancellationToken cancellationToken = default);

    Task<ProfilingUserStateSnapshot?> GetUserStateAsync(
        int userId,
        CancellationToken cancellationToken = default
    );
}
