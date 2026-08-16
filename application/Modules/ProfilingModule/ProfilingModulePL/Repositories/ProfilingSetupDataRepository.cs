using Microsoft.EntityFrameworkCore;
using ResumeEnhancer.Infrastructure.Caching;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.ProfilingModule.DM.Entities;
using ResumeEnhancer.ProfilingModule.SL.Abstractions.Persistence;

namespace ResumeEnhancer.ProfilingModule.PL.Repositories;

public sealed class ProfilingSetupDataRepository(
    IUnitOfWork<AppDbContext> unitOfWork,
    ICacheProvider cacheProvider) : IProfilingSetupDataRepository
{
    internal const string RolesCacheKey = "profiling:setup:roles";
    internal const string AccessProfilesCacheKey = "profiling:setup:access-profiles";
    internal const string UserAddressTypesCacheKey = "profiling:setup:user-address-types";

    private static readonly CacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12)
    };

    public Task<IReadOnlyList<Role>> ListRolesAsync(CancellationToken cancellationToken = default) =>
        cacheProvider.GetOrSetAsync<IReadOnlyList<Role>>(
            RolesCacheKey,
            async token => (IReadOnlyList<Role>)await unitOfWork.GetRepo<Role>()
                .Query()
                .AsNoTracking()
                .OrderBy(role => role.Order)
                .ThenBy(role => role.Code)
                .ToListAsync(token),
            CacheOptions,
            cancellationToken);

    public Task<IReadOnlyList<AccessProfile>> ListAccessProfilesAsync(CancellationToken cancellationToken = default) =>
        cacheProvider.GetOrSetAsync<IReadOnlyList<AccessProfile>>(
            AccessProfilesCacheKey,
            async token => (IReadOnlyList<AccessProfile>)await unitOfWork.GetRepo<AccessProfile>()
                .Query()
                .AsNoTracking()
                .OrderBy(profile => profile.Order)
                .ThenBy(profile => profile.Code)
                .ToListAsync(token),
            CacheOptions,
            cancellationToken);

    public Task<IReadOnlyList<UserAddressTypeSetup>> ListUserAddressTypesAsync(CancellationToken cancellationToken = default) =>
        cacheProvider.GetOrSetAsync<IReadOnlyList<UserAddressTypeSetup>>(
            UserAddressTypesCacheKey,
            async token => (IReadOnlyList<UserAddressTypeSetup>)await unitOfWork.GetRepo<UserAddressTypeSetup>()
                .Query()
                .AsNoTracking()
                .OrderBy(addressType => addressType.Order)
                .ThenBy(addressType => addressType.Id)
                .ToListAsync(token),
            CacheOptions,
            cancellationToken);

}
