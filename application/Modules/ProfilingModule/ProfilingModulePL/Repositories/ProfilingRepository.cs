using Microsoft.EntityFrameworkCore;
using ResumeEnhancer.Infrastructure.Caching;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.ProfilingModule.DM.Entities;
using ResumeEnhancer.ProfilingModule.SL.Abstractions.Persistence;
using ResumeEnhancer.ProfilingModule.SL.Integrations;

namespace ResumeEnhancer.ProfilingModule.PL.Repositories;

public sealed class ProfilingRepository(
    IUnitOfWork<AppDbContext> unitOfWork,
    ICacheProvider cacheProvider
) : IProfilingRepository
{
    private static readonly string[] SetupCacheKeys =
    [
        ProfilingSetupDataRepository.RolesCacheKey,
        ProfilingSetupDataRepository.AccessProfilesCacheKey,
        ProfilingSetupDataRepository.UserAddressTypesCacheKey,
    ];

    private readonly IUnitOfWork<AppDbContext> _unitOfWork = unitOfWork;
    private readonly ICacheProvider _cacheProvider = cacheProvider;

    public async Task<User> AddUserAsync(
        User user,
        int? auditUserId,
        CancellationToken cancellationToken = default
    )
    {
        await _unitOfWork.GetRepo<User>().AddAsync(user, cancellationToken);
        await SaveAsync(auditUserId, cancellationToken);
        return user;
    }

    public async Task<User> AddUserForRegistrationAsync(
        ProfileRegistrationInput input,
        CancellationToken cancellationToken = default
    )
    {
        var user = new User
        {
            FirstName = input.FirstName,
            LastName = input.LastName,
            Email = input.Email,
        };
        await _unitOfWork.GetRepo<User>().AddAsync(user, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);
        return user;
    }

    public Task AddRegistrationBaselineAsync(
        RegistrationBaselineInput input,
        CancellationToken cancellationToken = default
    )
    {
        return AddRegistrationBaselineEntitiesAsync(input, cancellationToken);
    }

    public async Task<AccessShapeSnapshot> GetAccessShapeAsync(
        int accessProfileId,
        CancellationToken cancellationToken = default
    )
    {
        var capabilities = await _unitOfWork
            .GetRepo<AccessProfileRole>()
            .Query()
            .Where(x => x.AccessProfileId == accessProfileId)
            .Select(x => x.Role!.Capability)
            .Where(x => x != "")
            .ToListAsync(cancellationToken);
        return new AccessShapeSnapshot(capabilities.ToHashSet(StringComparer.Ordinal));
    }

    public async Task<AccessShapeSnapshot> GetStarterAccessShapeAsync(
        CancellationToken cancellationToken = default
    )
    {
        var starterId = await _unitOfWork
            .GetRepo<AccessProfile>()
            .Query()
            .Where(x => x.Code == "Guest")
            .Select(x => (int?)x.Id)
            .SingleOrDefaultAsync(cancellationToken);
        return starterId is null
            ? new AccessShapeSnapshot(new HashSet<string>(StringComparer.Ordinal))
            : await GetAccessShapeAsync(starterId.Value, cancellationToken);
    }

    public async Task<StarterAccessProfileSnapshot?> GetStarterAccessProfileAsync(
        CancellationToken cancellationToken = default
    )
    {
        var starterId = await _unitOfWork
            .GetRepo<AccessProfile>()
            .Query()
            .Where(x => x.Code == "Guest")
            .Select(x => (int?)x.Id)
            .SingleOrDefaultAsync(cancellationToken);
        return starterId is null ? null : new StarterAccessProfileSnapshot(starterId.Value);
    }

    public async Task<User?> GetUserAsync(
        int userId,
        bool track = false,
        CancellationToken cancellationToken = default
    )
    {
        IQueryable<User> query = _unitOfWork
            .GetRepo<User>()
            .Query()
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

    public async Task<IReadOnlyList<User>> ListUsersAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await _unitOfWork
            .GetRepo<User>()
            .Query()
            .Include(user => user.UserAddresses)
                .ThenInclude(address => address.AddressType)
            .AsNoTracking()
            .OrderBy(user => user.FirstName)
            .ThenBy(user => user.LastName)
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteUserAsync(
        User user,
        int? auditUserId,
        CancellationToken cancellationToken = default
    )
    {
        _unitOfWork.GetRepo<User>().Delete(user);
        await SaveAsync(auditUserId, cancellationToken);
    }

    public async Task SyncUserAccessProfilesAsync(
        User user,
        IReadOnlyCollection<int> accessProfileIds,
        CancellationToken cancellationToken = default
    )
    {
        var targetIds = accessProfileIds.Where(id => id > 0).Distinct().ToHashSet();
        var relationRepository = _unitOfWork.GetRepo<UserAccessProfile>();
        var existingRelations = await relationRepository
            .Query()
            .Where(item => item.UserId == user.Id)
            .ToListAsync(cancellationToken);
        var existing = existingRelations.ToDictionary(item => item.AccessProfileId);

        var relationsToDelete = existingRelations
            .Where(item => !targetIds.Contains(item.AccessProfileId))
            .ToList();
        if (relationsToDelete.Count > 0)
        {
            relationRepository.Delete(relationsToDelete);
        }

        if (targetIds.Count == 0)
        {
            return;
        }

        var validIds = await _unitOfWork
            .GetRepo<AccessProfile>()
            .Query()
            .Where(profile => targetIds.Contains(profile.Id))
            .Select(profile => profile.Id)
            .ToListAsync(cancellationToken);

        foreach (var accessProfileId in validIds)
        {
            if (!existing.ContainsKey(accessProfileId))
            {
                await relationRepository.AddAsync(
                    new UserAccessProfile { UserId = user.Id, AccessProfileId = accessProfileId },
                    cancellationToken
                );
            }
        }
    }

    public async Task<bool> UserExistsAsync(
        int userId,
        CancellationToken cancellationToken = default
    )
    {
        return await _unitOfWork.GetRepo<User>().ExistsAsync(userId, cancellationToken);
    }

    public async Task<Role> AddRoleAsync(
        Role role,
        int? auditUserId,
        CancellationToken cancellationToken = default
    )
    {
        await _unitOfWork.GetRepo<Role>().AddAsync(role, cancellationToken);
        await SaveAsync(auditUserId, cancellationToken);
        return role;
    }

    public async Task<Role?> GetRoleAsync(
        int roleId,
        bool track = false,
        CancellationToken cancellationToken = default
    )
    {
        var query = _unitOfWork.GetRepo<Role>().Query();
        if (!track)
        {
            query = query.AsNoTracking();
        }

        return await query.SingleOrDefaultAsync(role => role.Id == roleId, cancellationToken);
    }

    public async Task<IReadOnlyList<Role>> ListRolesAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await _unitOfWork
            .GetRepo<Role>()
            .Query()
            .AsNoTracking()
            .OrderBy(role => role.Code)
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteRoleAsync(
        Role role,
        int? auditUserId,
        CancellationToken cancellationToken = default
    )
    {
        _unitOfWork.GetRepo<Role>().Delete(role);
        await SaveAsync(auditUserId, cancellationToken);
    }

    public async Task<AccessProfile> AddAccessProfileAsync(
        AccessProfile accessProfile,
        int? auditUserId,
        CancellationToken cancellationToken = default
    )
    {
        await _unitOfWork.GetRepo<AccessProfile>().AddAsync(accessProfile, cancellationToken);
        await SaveAsync(auditUserId, cancellationToken);
        return accessProfile;
    }

    public async Task<AccessProfile?> GetAccessProfileAsync(
        int accessProfileId,
        bool track = false,
        CancellationToken cancellationToken = default
    )
    {
        IQueryable<AccessProfile> query = _unitOfWork
            .GetRepo<AccessProfile>()
            .Query()
            .Include(accessProfile => accessProfile.AccessProfileRoles)
                .ThenInclude(item => item.Role);

        if (!track)
        {
            query = query.AsNoTracking();
        }

        return await query.SingleOrDefaultAsync(
            accessProfile => accessProfile.Id == accessProfileId,
            cancellationToken
        );
    }

    public async Task<IReadOnlyList<AccessProfile>> ListAccessProfilesAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await _unitOfWork
            .GetRepo<AccessProfile>()
            .Query()
            .AsNoTracking()
            .OrderBy(item => item.Code)
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteAccessProfileAsync(
        AccessProfile accessProfile,
        int? auditUserId,
        CancellationToken cancellationToken = default
    )
    {
        _unitOfWork.GetRepo<AccessProfile>().Delete(accessProfile);
        await SaveAsync(auditUserId, cancellationToken);
    }

    public async Task SyncAccessProfileRolesAsync(
        AccessProfile accessProfile,
        IReadOnlyCollection<int> roleIds,
        CancellationToken cancellationToken = default
    )
    {
        var targetIds = roleIds.Where(id => id > 0).Distinct().ToHashSet();
        var relationRepository = _unitOfWork.GetRepo<AccessProfileRole>();
        var existingRelations = await relationRepository
            .Query()
            .Where(item => item.AccessProfileId == accessProfile.Id)
            .ToListAsync(cancellationToken);
        var existing = existingRelations.ToDictionary(item => item.RoleId);

        var relationsToDelete = existingRelations
            .Where(item => !targetIds.Contains(item.RoleId))
            .ToList();
        if (relationsToDelete.Count > 0)
        {
            relationRepository.Delete(relationsToDelete);
        }

        if (targetIds.Count == 0)
        {
            return;
        }

        var validIds = await _unitOfWork
            .GetRepo<Role>()
            .Query()
            .Where(role => targetIds.Contains(role.Id))
            .Select(role => role.Id)
            .ToListAsync(cancellationToken);

        foreach (var roleId in validIds)
        {
            if (!existing.ContainsKey(roleId))
            {
                await relationRepository.AddAsync(
                    new AccessProfileRole
                    {
                        Guid = Guid.NewGuid(),
                        Code = $"{accessProfile.Code}:{roleId}",
                        AccessProfileId = accessProfile.Id,
                        RoleId = roleId,
                    },
                    cancellationToken
                );
            }
        }
    }

    public async Task SaveAsync(int? auditUserId, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.SaveAsync(new RepositoryAudit(auditUserId), cancellationToken);
        await InvalidateSetupCacheAsync(cancellationToken);
    }

    private async Task AddRegistrationBaselineEntitiesAsync(
        RegistrationBaselineInput input,
        CancellationToken cancellationToken
    )
    {
        await _unitOfWork
            .GetRepo<UserPreference>()
            .AddAsync(
                new UserPreference
                {
                    UserId = input.UserId,
                    Locale = "en",
                    OnboardingCompleted = false,
                },
                cancellationToken
            );
        await _unitOfWork
            .GetRepo<UserEntitlement>()
            .AddAsync(
                new UserEntitlement
                {
                    UserId = input.UserId,
                    BillingSubscriptionId = input.BillingSubscriptionId,
                    AccessProfileId = input.AccessProfileId,
                    Enabled = true,
                    Source = "plan",
                },
                cancellationToken
            );
    }

    private async Task InvalidateSetupCacheAsync(CancellationToken cancellationToken)
    {
        foreach (var cacheKey in SetupCacheKeys)
        {
            await _cacheProvider.RemoveAsync(cacheKey, cancellationToken);
        }
    }

    private sealed class RepositoryAudit(int? userId) : IAudit
    {
        public int? UserId { get; } = userId;
    }
}
