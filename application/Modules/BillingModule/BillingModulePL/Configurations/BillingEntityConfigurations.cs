using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResumeEnhancer.BillingModule.DM.Entities;

namespace ResumeEnhancer.BillingModule.PL.Configurations;

public sealed class BillingAccountConfiguration : IEntityTypeConfiguration<BillingAccount>
{
    public void Configure(EntityTypeBuilder<BillingAccount> builder)
    {
        builder.Property(entity => entity.AccountNumber).HasMaxLength(50).IsRequired();
        builder.Property(entity => entity.Status).HasMaxLength(50).IsRequired();
        builder.Property(entity => entity.ExternalReference).HasMaxLength(100);

        builder.HasIndex(entity => entity.AccountNumber).IsUnique();

        builder.HasOne(entity => entity.User)
            .WithMany()
            .HasForeignKey(entity => entity.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class BillingPlanConfiguration : IEntityTypeConfiguration<BillingPlan>
{
    public void Configure(EntityTypeBuilder<BillingPlan> builder)
    {
        builder.Property(entity => entity.DisplayName).HasMaxLength(200).IsRequired();
        builder.Property(entity => entity.Order).IsRequired();
        builder.Property(entity => entity.Currency).HasMaxLength(10).IsRequired();
        builder.Property(entity => entity.BillingInterval).HasMaxLength(50).IsRequired();
        builder.Property(entity => entity.Price).HasColumnType("decimal(18,2)");
        builder.Property(entity => entity.IsDeactivated).HasDefaultValue(false);
    }
}

public sealed class BillingSubscriptionConfiguration : IEntityTypeConfiguration<BillingSubscription>
{
    public void Configure(EntityTypeBuilder<BillingSubscription> builder)
    {
        builder.Property(entity => entity.Status).HasMaxLength(50).IsRequired();

        builder.HasOne(entity => entity.BillingAccount)
            .WithMany(account => account.Subscriptions)
            .HasForeignKey(entity => entity.BillingAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(entity => entity.BillingPlan)
            .WithMany(plan => plan.Subscriptions)
            .HasForeignKey(entity => entity.BillingPlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(entity => entity.Resume)
            .WithMany()
            .HasForeignKey(entity => entity.ResumeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
