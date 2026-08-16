using Microsoft.EntityFrameworkCore;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.ProfilingModule.DM.Entities;
using ResumeEnhancer.ProfilingModule.DM.Enums;

namespace ResumeEnhancer.ProfilingModule.PL.Seeding;

public sealed class ProfilingModuleSeeder : IAppDbContextSeeder
{
    public async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        await dbContext.Set<UserAddressTypeSetup>().SeedSetupDataAsync(
            CreateAddressTypes(),
            (existing, seed) =>
            {
                var hasChanges = false;

                if (existing.DisplayName != seed.DisplayName)
                {
                    existing.DisplayName = seed.DisplayName;
                    hasChanges = true;
                }

                if (existing.Order != seed.Order)
                {
                    existing.Order = seed.Order;
                    hasChanges = true;
                }

                return hasChanges;
            },
            cancellationToken);

        if (!await dbContext.Set<Role>().AnyAsync(cancellationToken))
        {
            dbContext.Set<Role>().AddRange(
                new Role { Code = "EveryAction", Description = "Administrative access", DisplayName = "Admin", Order = 1, Guid = Guid.NewGuid() },
                new Role { Code = "GuestOnlyView", Description = "Guest view access", DisplayName = "User", Order = 2, Guid = Guid.NewGuid() },
                new Role { Code = "ViewAdminPortal", Description = "View Admin Portal access", DisplayName = "Limited", Order = 3, Guid = Guid.NewGuid() });
        }

        if (!await dbContext.Set<AccessProfile>().AnyAsync(cancellationToken))
        {
            dbContext.Set<AccessProfile>().AddRange(
                new AccessProfile { Code = "Administrator", Description = "Default Admin profile", DisplayName = "Default", Order = 1, Guid = Guid.NewGuid() },
                new AccessProfile { Code = "Guest", Description = "Guest profile", DisplayName = "Guest", Order = 2, Guid = Guid.NewGuid() },
                new AccessProfile { Code = "LimitedUser", Description = "Limited User profile", DisplayName = "Limited", Order = 3, Guid = Guid.NewGuid() });
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static UserAddressTypeSetup[] CreateAddressTypes() =>
    [
        new UserAddressTypeSetup
        {
            Code = nameof(UserAddressType.Billing),
            Description = "Billing address type",
            DisplayName = "Billing",
            Order = 1,
            Guid = Guid.Parse("22222222-2222-2222-2222-222222222001"),
            ObsoleteFlag = false
        },
        new UserAddressTypeSetup
        {
            Code = nameof(UserAddressType.Communication),
            Description = "Communication address type",
            DisplayName = "Communication",
            Order = 2,
            Guid = Guid.Parse("22222222-2222-2222-2222-222222222002"),
            ObsoleteFlag = false
        }
    ];
}
