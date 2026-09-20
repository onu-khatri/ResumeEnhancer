using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResumeEnhancer.AuthModule.DM.Entities;

namespace ResumeEnhancer.AuthModule.PL.Configurations;

public sealed class AuthenticationIdentityConfiguration
    : IEntityTypeConfiguration<AuthenticationIdentity>
{
    public void Configure(EntityTypeBuilder<AuthenticationIdentity> b)
    {
        b.Property(x => x.NormalizedEmail).HasMaxLength(320).IsRequired();
        b.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
        b.HasIndex(x => x.NormalizedEmail).IsUnique();
        b.HasIndex(x => x.UserId).IsUnique();
        b.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        b.Property(x => x.FailedLoginAttempts).IsRequired();
        b.HasIndex(x => new { x.LockedUntilUtc, x.LastFailedLoginAtUtc });
    }
}

public sealed class RefreshSessionConfiguration : IEntityTypeConfiguration<RefreshSession>
{
    public void Configure(EntityTypeBuilder<RefreshSession> b)
    {
        b.Property(x => x.TokenHash).HasMaxLength(128).IsRequired();
        b.Property(x => x.RevocationReason).HasMaxLength(80);
        b.HasIndex(x => x.TokenHash).IsUnique();
        b.HasIndex(x => new
        {
            x.UserId,
            x.FamilyId,
            x.RevokedAtUtc,
        });
        b.HasIndex(x => x.ExpiresAtUtc);
    }
}

public sealed class PasswordHistoryEntryConfiguration : IEntityTypeConfiguration<PasswordHistoryEntry>
{
    public void Configure(EntityTypeBuilder<PasswordHistoryEntry> b)
    {
        b.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
        b.Property(x => x.CreatedAtUtc).IsRequired();
        b.HasIndex(x => new { x.AuthenticationIdentityId, x.CreatedAtUtc });
        b.HasOne(x => x.AuthenticationIdentity)
            .WithMany(x => x.PasswordHistory)
            .HasForeignKey(x => x.AuthenticationIdentityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class AuthChallengeConfiguration : IEntityTypeConfiguration<AuthChallenge>
{
    public void Configure(EntityTypeBuilder<AuthChallenge> b)
    {
        b.Property(x => x.TokenHash).HasMaxLength(500).IsRequired();
        b.Property(x => x.IssuedAtUtc).IsRequired();
        b.Property(x => x.ExpiresAtUtc).IsRequired();
        b.Property(x => x.IpAddress).HasMaxLength(100);
        b.Property(x => x.UserAgent).HasMaxLength(300);
        b.HasIndex(x => new { x.AuthenticationIdentityId, x.AuthChallengePurposeId, x.TokenHash }).IsUnique();
        b.HasIndex(x => new { x.AuthChallengePurposeId, x.TokenHash, x.ConsumedAtUtc, x.ExpiresAtUtc });
        b.HasOne(x => x.AuthenticationIdentity)
            .WithMany(x => x.Challenges)
            .HasForeignKey(x => x.AuthenticationIdentityId)
            .OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.AuthChallengePurpose)
            .WithMany()
            .HasForeignKey(x => x.AuthChallengePurposeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class AuthChallengePurposeConfiguration : IEntityTypeConfiguration<AuthChallengePurpose>
{
    public void Configure(EntityTypeBuilder<AuthChallengePurpose> b)
    {
        b.Property(x => x.Code).HasMaxLength(100).IsRequired();
        b.Property(x => x.Description).HasMaxLength(1000).IsRequired();
        b.HasIndex(x => x.Code).IsUnique();
    }
}

public sealed class AuthSigningKeyMetadataConfiguration : IEntityTypeConfiguration<AuthSigningKeyMetadata>
{
    public void Configure(EntityTypeBuilder<AuthSigningKeyMetadata> b)
    {
        b.Property(x => x.KeyIdentifier).HasMaxLength(128).IsRequired();
        b.Property(x => x.ProtectedMaterial).HasMaxLength(8192).IsRequired();
        b.Property(x => x.ActivatedAtUtc).IsRequired();
        b.Property(x => x.IsActive).IsRequired();
        b.Property(x => x.LifecycleVersion).IsRequired();
        b.HasIndex(x => x.KeyIdentifier).IsUnique();
        b.HasIndex(x => new { x.RetiredAtUtc, x.InvalidatedAtUtc, x.ActivatedAtUtc });
        b.HasIndex(x => x.IsActive).IsUnique()
            .HasFilter("[IsActive] = 1 AND [RetiredAtUtc] IS NULL AND [InvalidatedAtUtc] IS NULL");
    }
}

public sealed class ConsentRecordConfiguration : IEntityTypeConfiguration<ConsentRecord>
{
    public void Configure(EntityTypeBuilder<ConsentRecord> b)
    {
        b.Property(x => x.Type).HasMaxLength(30).IsRequired();
        b.Property(x => x.VersionId).HasMaxLength(100).IsRequired();
        b.HasIndex(x => new
            {
                x.UserId,
                x.Type,
                x.VersionId,
            })
            .IsUnique();
    }
}

public sealed class AuthAuditEventConfiguration : IEntityTypeConfiguration<AuthAuditEvent>
{
    public void Configure(EntityTypeBuilder<AuthAuditEvent> b)
    {
        b.Property(x => x.EventType).HasMaxLength(80).IsRequired();
        b.Property(x => x.MetadataJson).HasMaxLength(2000).IsRequired();
        b.HasIndex(x => new { x.UserId, x.EventType });
    }
}

public sealed class AuthOutboxMessageConfiguration : IEntityTypeConfiguration<AuthOutboxMessage>
{
    public void Configure(EntityTypeBuilder<AuthOutboxMessage> b)
    {
        b.Property(x => x.Type).HasMaxLength(80).IsRequired();
        b.Property(x => x.PayloadJson).HasMaxLength(2000).IsRequired();
        b.HasIndex(x => new
        {
            x.ProcessedAtUtc,
            x.AvailableAtUtc,
            x.LeaseExpiresAtUtc,
        });
    }
}

public sealed class AuthRegistrationIdempotencyConfiguration
    : IEntityTypeConfiguration<AuthRegistrationIdempotency>
{
    public void Configure(EntityTypeBuilder<AuthRegistrationIdempotency> b)
    {
        b.Property(x => x.NormalizedEmail).HasMaxLength(320).IsRequired();
        b.Property(x => x.IdempotencyKey).HasMaxLength(100).IsRequired();
        b.Property(x => x.RequestHash).HasMaxLength(64).IsRequired();
        b.Property(x => x.ResponseJson).HasMaxLength(12000).IsRequired();
        b.HasIndex(x => new { x.NormalizedEmail, x.IdempotencyKey }).IsUnique();
    }
}

public sealed class AuthConsentVersionConfiguration : IEntityTypeConfiguration<AuthConsentVersion>
{
    public void Configure(EntityTypeBuilder<AuthConsentVersion> b)
    {
        b.Property(x => x.Type).HasMaxLength(30).IsRequired();
        b.Property(x => x.VersionId).HasMaxLength(100).IsRequired();
        b.HasIndex(x => new { x.Type, x.VersionId }).IsUnique();
    }
}
