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
    }
}

public sealed class RefreshSessionConfiguration : IEntityTypeConfiguration<RefreshSession>
{
    public void Configure(EntityTypeBuilder<RefreshSession> b)
    {
        b.Property(x => x.TokenHash).HasMaxLength(128).IsRequired();
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
