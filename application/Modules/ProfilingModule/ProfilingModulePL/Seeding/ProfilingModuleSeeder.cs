using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.ProfilingModule.DM.Entities;
using ResumeEnhancer.ProfilingModule.DM.Enums;

namespace ResumeEnhancer.ProfilingModule.PL.Seeding;

[ExcludeFromCodeCoverage]
public sealed class ProfilingModuleSeeder : IAppDbContextSeeder
{
    public async Task SeedAsync(
        AppDbContext dbContext,
        CancellationToken cancellationToken = default
    )
    {
        await dbContext
            .Set<AccessProfileSource>()
            .SeedSetupDataAsync(
                [
                    new AccessProfileSource
                    {
                        Code = "plan",
                        Description = "Access profile assigned from a billing plan",
                        DisplayName = "Billing plan",
                        Order = 1,
                        Guid = Guid.Parse("22222222-2222-2222-2222-222222222301"),
                    },
                    new AccessProfileSource
                    {
                        Code = "admin",
                        Description = "Access profile assigned by an administrator",
                        DisplayName = "Administrator",
                        Order = 2,
                        Guid = Guid.Parse("22222222-2222-2222-2222-222222222302"),
                    },
                ],
                (existing, seed) =>
                {
                    var changed = false;
                    if (existing.Description != seed.Description)
                    {
                        existing.Description = seed.Description;
                        changed = true;
                    }
                    if (existing.DisplayName != seed.DisplayName)
                    {
                        existing.DisplayName = seed.DisplayName;
                        changed = true;
                    }
                    if (existing.Order != seed.Order)
                    {
                        existing.Order = seed.Order;
                        changed = true;
                    }
                    return changed;
                },
                cancellationToken
            );

        await dbContext
            .Set<UserAddressTypeSetup>()
            .SeedSetupDataAsync(
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
                cancellationToken
            );

        if (!await dbContext.Set<Role>().AnyAsync(cancellationToken))
        {
            dbContext
                .Set<Role>()
                .AddRange(
                    new Role
                    {
                        Code = "EveryAction",
                        Description = "Administrative access",
                        DisplayName = "Admin",
                        Order = 1,
                        Guid = Guid.NewGuid(),
                    },
                    new Role
                    {
                        Code = "GuestOnlyView",
                        Description = "Guest view access",
                        DisplayName = "User",
                        Order = 2,
                        Guid = Guid.NewGuid(),
                    },
                    new Role
                    {
                        Code = "ViewAdminPortal",
                        Description = "View Admin Portal access",
                        DisplayName = "Limited",
                        Order = 3,
                        Guid = Guid.NewGuid(),
                    }
                );
        }

        if (!await dbContext.Set<AccessProfile>().AnyAsync(cancellationToken))
        {
            dbContext
                .Set<AccessProfile>()
                .AddRange(
                    new AccessProfile
                    {
                        Code = "Administrator",
                        Description = "Default Admin profile",
                        DisplayName = "Default",
                        Order = 1,
                        Guid = Guid.NewGuid(),
                    },
                    new AccessProfile
                    {
                        Code = "Guest",
                        Description = "Guest profile",
                        DisplayName = "Guest",
                        Order = 2,
                        Guid = Guid.NewGuid(),
                    },
                    new AccessProfile
                    {
                        Code = "LimitedUser",
                        Description = "Limited User profile",
                        DisplayName = "Limited",
                        Order = 3,
                        Guid = Guid.NewGuid(),
                    }
                );
        }

        var stableRoles = new[]
        {
            "TemplateView",
            "ResumePublish",
            "PdfExport",
            "ResumeCreate",
            "PremiumTemplates",
        };
        foreach (var code in stableRoles)
        {
            var role = await dbContext
                .Set<Role>()
                .SingleOrDefaultAsync(x => x.Code == code, cancellationToken);
            if (role is null)
            {
                dbContext
                    .Set<Role>()
                    .Add(
                        new Role
                        {
                            Code = code,
                            Capability = code,
                            Description = $"Stable entitlement {code}",
                            DisplayName = code,
                            Order = 10,
                            Guid = StableGuid(code),
                        }
                    );
            }
            else if (role.Capability != code)
            {
                role.Capability = code;
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        var guest = await dbContext
            .Set<AccessProfile>()
            .SingleOrDefaultAsync(x => x.Code == "Guest", cancellationToken);
        if (guest is null)
        {
            return;
        }
        var starterRoleCodes = new[] { "TemplateView", "PdfExport", "ResumeCreate" };
        var roleIds = await dbContext
            .Set<Role>()
            .Where(x => starterRoleCodes.Contains(x.Code))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);
        var attachedRoleIds = await dbContext
            .Set<AccessProfileRole>()
            .Where(x => x.AccessProfileId == guest.Id)
            .Select(x => x.RoleId)
            .ToListAsync(cancellationToken);
        foreach (var roleId in roleIds.Where(x => !attachedRoleIds.Contains(x)))
        {
            dbContext
                .Set<AccessProfileRole>()
                .Add(
                    new AccessProfileRole
                    {
                        Guid = Guid.NewGuid(),
                        Code = $"{guest.Code}:{roleId}",
                        AccessProfileId = guest.Id,
                        RoleId = roleId,
                    }
                );
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static UserAddressTypeSetup[] CreateAddressTypes()
    {
        return
        [
            new UserAddressTypeSetup
            {
                Code = nameof(UserAddressType.Billing),
                Description = "Billing address type",
                DisplayName = "Billing",
                Order = 1,
                Guid = Guid.Parse("22222222-2222-2222-2222-222222222001"),
                ObsoleteFlag = false,
            },
            new UserAddressTypeSetup
            {
                Code = nameof(UserAddressType.Communication),
                Description = "Communication address type",
                DisplayName = "Communication",
                Order = 2,
                Guid = Guid.Parse("22222222-2222-2222-2222-222222222002"),
                ObsoleteFlag = false,
            },
        ];
    }

    private static Guid StableGuid(string code)
    {
        return code switch
        {
            "TemplateView" => Guid.Parse("22222222-2222-2222-2222-222222222101"),
            "ResumePublish" => Guid.Parse("22222222-2222-2222-2222-222222222102"),
            "PdfExport" => Guid.Parse("22222222-2222-2222-2222-222222222103"),
            "ResumeCreate" => Guid.Parse("22222222-2222-2222-2222-222222222104"),
            "PremiumTemplates" => Guid.Parse("22222222-2222-2222-2222-222222222105"),
            _ => throw new ArgumentOutOfRangeException(nameof(code)),
        };
    }
}
