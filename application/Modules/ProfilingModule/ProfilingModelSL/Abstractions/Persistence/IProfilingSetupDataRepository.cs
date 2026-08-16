using ResumeEnhancer.ProfilingModule.DM.Entities;

namespace ResumeEnhancer.ProfilingModule.SL.Abstractions.Persistence;

public interface IProfilingSetupDataRepository
{
    Task<IReadOnlyList<Role>> ListRolesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AccessProfile>> ListAccessProfilesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserAddressTypeSetup>> ListUserAddressTypesAsync(CancellationToken cancellationToken = default);
}
