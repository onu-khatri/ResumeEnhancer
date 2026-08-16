using ResumeEnhancer.ProfilingModule.DM.Entities;

namespace ResumeEnhancer.ProfilingModule.SL.Abstractions.Persistence;

public interface IProfilingRepository
{
    Task<User> AddUserAsync(User user, int? auditUserId, CancellationToken cancellationToken = default);
    Task<User?> GetUserAsync(int userId, bool track = false, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> ListUsersAsync(CancellationToken cancellationToken = default);
    Task DeleteUserAsync(User user, int? auditUserId, CancellationToken cancellationToken = default);
    Task SyncUserAccessProfilesAsync(User user, IReadOnlyCollection<int> accessProfileIds, CancellationToken cancellationToken = default);
    Task<bool> UserExistsAsync(int userId, CancellationToken cancellationToken = default);

    Task<Role> AddRoleAsync(Role role, int? auditUserId, CancellationToken cancellationToken = default);
    Task<Role?> GetRoleAsync(int roleId, bool track = false, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Role>> ListRolesAsync(CancellationToken cancellationToken = default);
    Task DeleteRoleAsync(Role role, int? auditUserId, CancellationToken cancellationToken = default);

    Task<AccessProfile> AddAccessProfileAsync(AccessProfile accessProfile, int? auditUserId, CancellationToken cancellationToken = default);
    Task<AccessProfile?> GetAccessProfileAsync(int accessProfileId, bool track = false, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AccessProfile>> ListAccessProfilesAsync(CancellationToken cancellationToken = default);
    Task DeleteAccessProfileAsync(AccessProfile accessProfile, int? auditUserId, CancellationToken cancellationToken = default);
    Task SyncAccessProfileRolesAsync(AccessProfile accessProfile, IReadOnlyCollection<int> roleIds, CancellationToken cancellationToken = default);

    Task SaveAsync(int? auditUserId, CancellationToken cancellationToken = default);
}
