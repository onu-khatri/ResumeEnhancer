using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ResumeEnhancer.Infrastructure.Persistence.Limiting;

public sealed class DbCacheLimiterEntryConfiguration : IEntityTypeConfiguration<DbCacheLimiterEntry>
{
    public void Configure(EntityTypeBuilder<DbCacheLimiterEntry> builder)
    {
        builder.ToTable("S_DbCacheLimiter", "dbo");
        builder.Property(x => x.KeyHash).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Count).IsRequired();
        builder.Property(x => x.WindowStartedUtc).IsRequired();
        builder.Property(x => x.ExpiresAtUtc).IsRequired();
        builder.HasIndex(x => x.KeyHash).IsUnique();
        builder.HasIndex(x => x.ExpiresAtUtc);
    }
}
