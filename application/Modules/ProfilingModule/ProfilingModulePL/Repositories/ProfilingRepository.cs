using Microsoft.EntityFrameworkCore;
using ResumeEnhancer.Infrastructure.Caching;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.ProfilingModule.DM.Entities;
using ResumeEnhancer.ProfilingModule.SL.Abstractions.Persistence;

namespace ResumeEnhancer.ProfilingModule.PL.Repositories;

public sealed class ProfilingRepository : IProfilingRepository
{
    private static readonly string[] SetupCacheKeys =
    [
        ProfilingSetupDataRepository.RolesCacheKey,
        ProfilingSetupDataRepository.AccessProfilesCacheKey,
        ProfilingSetupDataRepository.UserAddressTypesCacheKey
    ];

    private readonly IUnitOfWork<AppDbContext> _unitOfWork;
    private readonly ICacheProvider _cacheProvider;

    public ProfilingRepository(IUnitOfWork<AppDbContext> unitOfWork, ICacheProvider cacheProvider)
    {
        _unitOfWork = unitOfWork;
        _cacheProvider = cacheProvider;
    }

    public async Task<User> AddUserAsync(User user, int? auditUserId, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.GetRepo<User>().AddAsync(user, cancellationToken);
        await SaveAsync(auditUserId, cancellationToken);
        return user;
    }

    public async Task<User?> GetUserAsync(int userId, bool track = false, CancellationToken cancellationToken = default)
    {
        IQueryable<User> query = _unitOfWork.GetRepo<User>().Query()
            .Include(user => user.UserAddresses)
                .ThenInclude(address => address.AddressType)
            .Include(user => user.UserAccessProfiles)
                .ThenInclude(item => item.AccessProfile);

        if (!track)
        {
            query = query.AsNoTracking();
        }

        return await query.SingleOrDefaultAsync(user => user.Id == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<User>> ListUsersAsync(CancellationToken cancellationToken = default) =>
        await _unitOfWork.GetRepo<User>()
            .Query()
            .Include(user => user.UserAddresses)
                .ThenInclude(address => address.AddressType)
            .AsNoTracking()
            .OrderBy(user => user.FirstName)
            .ThenBy(user => user.LastName)
            .ToListAsync(cancellationToken);

    public async Task DeleteUserAsync(User user, int? auditUserId, CancellationToken cancellationToken = default)
    {
        _unitOfWork.DbContext.Remove(user);
        await SaveAsync(auditUserId, cancellationToken);
    }

    public async Task SyncUserAccessProfilesAsync(User user, IReadOnlyCollection<int> accessProfileIds, CancellationToken cancellationToken = default)
    {
        await EnsureUserCollectionsLoadedAsync(user, cancellationToken);
        var targetIds = accessProfileIds.Where(id => id > 0).Distinct().ToHashSet();
        var existing = user.UserAccessProfiles.ToDictionary(item => item.AccessProfileId);

        foreach (var relation in user.UserAccessProfiles.Where(item => !targetIds.Contains(item.AccessProfileId)).ToArray())
        {
            _unitOfWork.DbContext.Remove(relation);
        }

        if (targetIds.Count == 0)
        {
            return;
        }

        var validIds = await _unitOfWork.GetRepo<AccessProfile>()
            .Query()
            .Where(profile => targetIds.Contains(profile.Id))
            .Select(profile => profile.Id)
            .ToListAsync(cancellationToken);

        foreach (var accessProfileId in validIds)
        {
            if (!existing.ContainsKey(accessProfileId))
            {
                user.UserAccessProfiles.Add(new UserAccessProfile
                {
                    UserId = user.Id,
                    AccessProfileId = accessProfileId
                });
            }
        }
    }

    public async Task<bool> UserExistsAsync(int userId, CancellationToken cancellationToken = default) =>
        await _unitOfWork.GetRepo<User>().ExistsAsync(userId, cancellationToken);

    public async Task<Role> AddRoleAsync(Role role, int? auditUserId, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.GetRepo<Role>().AddAsync(role, cancellationToken);
        await SaveAsync(auditUserId, cancellationToken);
        return role;
    }

    public async Task<Role?> GetRoleAsync(int roleId, bool track = false, CancellationToken cancellationToken = default)
    {
        var query = _unitOfWork.GetRepo<Role>().Query();
        if (!track)
        {
            query = query.AsNoTracking();
        }

        return await query.SingleOrDefaultAsync(role => role.Id == roleId, cancellationToken);
    }

    public async Task<IReadOnlyList<Role>> ListRolesAsync(CancellationToken cancellationToken = default) =>
        await _unitOfWork.GetRepo<Role>().Query().AsNoTracking().OrderBy(role => role.Code).ToListAsync(cancellationToken);

    public async Task DeleteRoleAsync(Role role, int? auditUserId, CancellationToken cancellationToken = default)
    {
        _unitOfWork.DbContext.Remove(role);
        await SaveAsync(auditUserId, cancellationToken);
    }

    public async Task<AccessProfile> AddAccessProfileAsync(AccessProfile accessProfile, int? auditUserId, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.GetRepo<AccessProfile>().AddAsync(accessProfile, cancellationToken);
        await SaveAsync(auditUserId, cancellationToken);
        return accessProfile;
    }

    public async Task<AccessProfile?> GetAccessProfileAsync(int accessProfileId, bool track = false, CancellationToken cancellationToken = default)
    {
        IQueryable<AccessProfile> query = _unitOfWork.GetRepo<AccessProfile>().Query()
            .Include(accessProfile => accessProfile.AccessProfileRoles)
                .ThenInclude(item => item.Role);

        if (!track)
        {
            query = query.AsNoTracking();
        }

        return await query.SingleOrDefaultAsync(accessProfile => accessProfile.Id == accessProfileId, cancellationToken);
    }

    public async Task<IReadOnlyList<AccessProfile>> ListAccessProfilesAsync(CancellationToken cancellationToken = default) =>
        await _unitOfWork.GetRepo<AccessProfile>().Query().AsNoTracking().OrderBy(item => item.Code).ToListAsync(cancellationToken);

    public async Task DeleteAccessProfileAsync(AccessProfile accessProfile, int? auditUserId, CancellationToken cancellationToken = default)
    {
        _unitOfWork.DbContext.Remove(accessProfile);
        await SaveAsync(auditUserId, cancellationToken);
    }

    public async Task SyncAccessProfileRolesAsync(AccessProfile accessProfile, IReadOnlyCollection<int> roleIds, CancellationToken cancellationToken = default)
    {
        await EnsureAccessProfileCollectionsLoadedAsync(accessProfile, cancellationToken);
        var targetIds = roleIds.Where(id => id > 0).Distinct().ToHashSet();
        var existing = accessProfile.AccessProfileRoles.ToDictionary(item => item.RoleId);

        foreach (var relation in accessProfile.AccessProfileRoles.Where(item => !targetIds.Contains(item.RoleId)).ToArray())
        {
            _unitOfWork.DbContext.Remove(relation);
        }

        if (targetIds.Count == 0)
        {
            return;
        }

        var validIds = await _unitOfWork.GetRepo<Role>()
            .Query()
            .Where(role => targetIds.Contains(role.Id))
            .Select(role => role.Id)
            .ToListAsync(cancellationToken);

        foreach (var roleId in validIds)
        {
            if (!existing.ContainsKey(roleId))
            {
                accessProfile.AccessProfileRoles.Add(new AccessProfileRole
                {
                    AccessProfileId = accessProfile.Id,
                    RoleId = roleId
                });
            }
        }
    }

    public async Task SaveAsync(int? auditUserId, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.SaveAsync(new RepositoryAudit(auditUserId), cancellationToken);
        await InvalidateSetupCacheAsync(cancellationToken);
    }

    private async Task EnsureUserCollectionsLoadedAsync(User user, CancellationToken cancellationToken)
    {
        if (!_unitOfWork.DbContext.Entry(user).Collection(item => item.UserAccessProfiles).IsLoaded)
        {
            await _unitOfWork.DbContext.Entry(user).Collection(item => item.UserAccessProfiles).LoadAsync(cancellationToken);
        }
    }

    private async Task EnsureAccessProfileCollectionsLoadedAsync(AccessProfile accessProfile, CancellationToken cancellationToken)
    {
        if (!_unitOfWork.DbContext.Entry(accessProfile).Collection(item => item.AccessProfileRoles).IsLoaded)
        {
            await _unitOfWork.DbContext.Entry(accessProfile).Collection(item => item.AccessProfileRoles).LoadAsync(cancellationToken);
        }
    }

    private async Task InvalidateSetupCacheAsync(CancellationToken cancellationToken)
    {
        foreach (var cacheKey in SetupCacheKeys)
        {
            await _cacheProvider.RemoveAsync(cacheKey, cancellationToken);
        }
    }

    private sealed class RepositoryAudit : IAudit
    {
        public RepositoryAudit(int? userId) => UserId = userId;
        public int? UserId { get; }
    }
}
