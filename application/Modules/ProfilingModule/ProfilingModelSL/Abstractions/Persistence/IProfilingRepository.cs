using ResumeEnhancer.ProfilingModule.DM.Entities;
using ResumeEnhancer.ProfilingModule.SL.Integrations;

namespace ResumeEnhancer.ProfilingModule.SL.Abstractions.Persistence;

public interface IProfilingRepository
{
    public Task<User> AddUserAsync(
        User user,
        int? auditUserId,
        CancellationToken cancellationToken = default
    );
    public Task<User> AddUserForRegistrationAsync(
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
    public Task ReviseAccessProfilesAsync(
        IReadOnlyCollection<AccessProfileRevisionInput> inputs,
        CancellationToken cancellationToken = default
    );
    public Task ProcessPendingAccessProfileRevisionsAsync(
        CancellationToken cancellationToken = default
    );
    public Task<User?> GetUserAsync(
        int userId,
        bool track = false,
        CancellationToken cancellationToken = default
    );
    public Task<IReadOnlyList<User>> ListUsersAsync(CancellationToken cancellationToken = default);
    public Task DeleteUserAsync(
        User user,
        int? auditUserId,
        CancellationToken cancellationToken = default
    );
    public Task SyncUserAccessProfilesAsync(
        User user,
        IReadOnlyCollection<int> accessProfileIds,
        CancellationToken cancellationToken = default
    );
    public Task<bool> UserExistsAsync(int userId, CancellationToken cancellationToken = default);
    public Task<User?> GetUserStateAsync(int userId, CancellationToken cancellationToken = default);
    public Task<ProfilingAuthorizationSnapshot?> GetUserAuthorizationAsync(
        int userId,
        CancellationToken cancellationToken = default
    );
    public Task<ProfilingAuthorizationSnapshot?> GetGuestAuthorizationAsync(
        CancellationToken cancellationToken = default
    );

    public Task<Role> AddRoleAsync(
        Role role,
        int? auditUserId,
        CancellationToken cancellationToken = default
    );
    public Task<Role?> GetRoleAsync(
        int roleId,
        bool track = false,
        CancellationToken cancellationToken = default
    );
    public Task<IReadOnlyList<Role>> ListRolesAsync(CancellationToken cancellationToken = default);
    public Task DeleteRoleAsync(
        Role role,
        int? auditUserId,
        CancellationToken cancellationToken = default
    );

    public Task<AccessProfile> AddAccessProfileAsync(
        AccessProfile accessProfile,
        int? auditUserId,
        CancellationToken cancellationToken = default
    );
    public Task<AccessProfile?> GetAccessProfileAsync(
        int accessProfileId,
        bool track = false,
        CancellationToken cancellationToken = default
    );
    public Task<IReadOnlyList<AccessProfile>> ListAccessProfilesAsync(
        CancellationToken cancellationToken = default
    );
    public Task DeleteAccessProfileAsync(
        AccessProfile accessProfile,
        int? auditUserId,
        CancellationToken cancellationToken = default
    );
    public Task SyncAccessProfileRolesAsync(
        AccessProfile accessProfile,
        IReadOnlyCollection<int> roleIds,
        CancellationToken cancellationToken = default
    );

    public Task SaveAsync(int? auditUserId, CancellationToken cancellationToken = default);
}
