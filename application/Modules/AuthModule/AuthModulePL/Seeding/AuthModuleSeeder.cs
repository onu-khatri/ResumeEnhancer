using ResumeEnhancer.AuthModule.DM.Entities;
using ResumeEnhancer.Infrastructure.Persistence;

namespace ResumeEnhancer.AuthModule.PL.Seeding;

public sealed class AuthModuleSeeder : IAppDbContextSeeder
{
    public async Task SeedAsync(
        AppDbContext dbContext,
        CancellationToken cancellationToken = default
    )
    {
        await dbContext
            .Set<AuthConsentVersion>()
            .SeedSetupDataAsync(
                [
                    new AuthConsentVersion
                    {
                        Code = "Terms",
                        Guid = Guid.Parse("33333333-3333-3333-3333-333333333301"),
                        Type = "terms",
                        VersionId = "terms-v1",
                        Required = true,
                    },
                    new AuthConsentVersion
                    {
                        Code = "Privacy",
                        Guid = Guid.Parse("33333333-3333-3333-3333-333333333302"),
                        Type = "privacy",
                        VersionId = "privacy-v1",
                        Required = true,
                    },
                    new AuthConsentVersion
                    {
                        Code = "Marketing",
                        Guid = Guid.Parse("33333333-3333-3333-3333-333333333303"),
                        Type = "marketing",
                        VersionId = "marketing-v1",
                        Required = false,
                    },
                ],
                (existing, seed) =>
                {
                    var changed = false;
                    if (existing.Type != seed.Type)
                    {
                        existing.Type = seed.Type;
                        changed = true;
                    }
                    if (existing.VersionId != seed.VersionId)
                    {
                        existing.VersionId = seed.VersionId;
                        changed = true;
                    }
                    if (existing.Required != seed.Required)
                    {
                        existing.Required = seed.Required;
                        changed = true;
                    }
                    if (existing.Active != seed.Active)
                    {
                        existing.Active = seed.Active;
                        changed = true;
                    }
                    return changed;
                },
                cancellationToken
            );
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
