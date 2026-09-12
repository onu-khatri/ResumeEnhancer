using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResumeEnhancer.BillingModule.DM.Entities;

namespace ResumeEnhancer.BillingModule.PL.Configurations;

public sealed class BillingAccountConfiguration : IEntityTypeConfiguration<BillingAccount>
{
    public void Configure(EntityTypeBuilder<BillingAccount> builder)
    {
        builder.Property(entity => entity.AccountNumber).HasMaxLength(50).IsRequired();
        builder.Property(entity => entity.ExternalReference).HasMaxLength(100);

        builder.HasIndex(entity => entity.AccountNumber).IsUnique();

        builder
            .HasOne(entity => entity.User)
            .WithMany()
            .HasForeignKey(entity => entity.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(entity => entity.Status)
            .WithMany(status => status.BillingAccounts)
            .HasForeignKey(entity => entity.StatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class BillingPlanConfiguration : IEntityTypeConfiguration<BillingPlan>
{
    public void Configure(EntityTypeBuilder<BillingPlan> builder)
    {
        builder.Property(entity => entity.DisplayName).HasMaxLength(200).IsRequired();
        builder.Property(entity => entity.Order).IsRequired();
        builder.Property(entity => entity.Price).HasColumnType("decimal(18,2)");
        builder.Property(entity => entity.IsDeactivated).HasDefaultValue(false);
        builder.Property(entity => entity.AccessProfileId).IsRequired();

        builder
            .HasOne(entity => entity.AccessProfile)
            .WithMany()
            .HasForeignKey(entity => entity.AccessProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(entity => entity.Currency)
            .WithMany(currency => currency.BillingPlans)
            .HasForeignKey(entity => entity.CurrencyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(entity => entity.BillingInterval)
            .WithMany(interval => interval.BillingPlans)
            .HasForeignKey(entity => entity.BillingIntervalId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class BillingSubscriptionConfiguration : IEntityTypeConfiguration<BillingSubscription>
{
    public void Configure(EntityTypeBuilder<BillingSubscription> builder)
    {
        builder
            .HasOne(entity => entity.BillingAccount)
            .WithMany(account => account.Subscriptions)
            .HasForeignKey(entity => entity.BillingAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(entity => entity.BillingPlan)
            .WithMany(plan => plan.Subscriptions)
            .HasForeignKey(entity => entity.BillingPlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(entity => entity.User)
            .WithMany()
            .HasForeignKey(entity => entity.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(entity => entity.Status)
            .WithMany(status => status.BillingSubscriptions)
            .HasForeignKey(entity => entity.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
