using ResumeEnhancer.AuthModule.DM.Entities;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.AuthModule.SL.Abstractions;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace ResumeEnhancer.AuthModule.PL.Seeding;

public sealed class AuthModuleSeeder(IAuthProtectedKeyMaterialStore? keyMaterialStore = null, TimeProvider? timeProvider = null) : IAppDbContextSeeder
{
    public async Task SeedAsync(
        AppDbContext dbContext,
        CancellationToken cancellationToken = default
    )
    {
        await dbContext
            .Set<AuthChallengePurpose>()
            .SeedSetupDataAsync(
                [
                    new AuthChallengePurpose
                    {
                        Code = "password-reset",
                        Description = "Password reset challenge",
                        Guid = Guid.Parse("33333333-3333-3333-3333-333333333311"),
                    },
                    new AuthChallengePurpose
                    {
                        Code = "email-verification",
                        Description = "Email verification challenge",
                        Guid = Guid.Parse("33333333-3333-3333-3333-333333333312"),
                    },
                ],
                (existing, seed) =>
                {
                    var changed = existing.Description != seed.Description || existing.ObsoleteFlag != seed.ObsoleteFlag;
                    existing.Description = seed.Description;
                    existing.ObsoleteFlag = seed.ObsoleteFlag;
                    return changed;
                },
                cancellationToken
            );
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
        if (keyMaterialStore is not null && !await dbContext.Set<AuthSigningKeyMetadata>().AnyAsync(cancellationToken))
        {
            using var rsa = RSA.Create(2048);
            dbContext.Add(new AuthSigningKeyMetadata
            {
                KeyIdentifier = "primary",
                ProtectedMaterial = keyMaterialStore.Protect(rsa),
                IsActive = true,
                LifecycleVersion = 1,
                ActivatedAtUtc = (timeProvider ?? TimeProvider.System).GetUtcNow().UtcDateTime,
            });
        }
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
