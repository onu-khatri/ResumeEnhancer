using ResumeEnhancer.BillingModule.DM.Entities;
using ResumeEnhancer.Infrastructure.Persistence;

namespace ResumeEnhancer.BillingModule.PL.Seeding;

public sealed class BillingModuleSeeder : IAppDbContextSeeder
{
    public async Task SeedAsync(
        AppDbContext dbContext,
        CancellationToken cancellationToken = default
    )
    {
        await dbContext
            .Set<BillingPlan>()
            .SeedSetupDataAsync(
                [
                    new BillingPlan
                    {
                        Code = "FREE",
                        Description = "Free starter plan",
                        DisplayName = "Free",
                        Order = 1,
                        Price = 0,
                        Currency = "USD",
                        BillingInterval = "Monthly",
                        Guid = Guid.Parse("44444444-4444-4444-4444-444444444401"),
                        IsDeactivated = false,
                    },
                    new BillingPlan
                    {
                        Code = "PRO",
                        Description = "Professional plan",
                        DisplayName = "Pro",
                        Order = 2,
                        Price = 19.99m,
                        Currency = "USD",
                        BillingInterval = "Monthly",
                        Guid = Guid.Parse("44444444-4444-4444-4444-444444444402"),
                        IsDeactivated = false,
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
                    if (existing.Price != seed.Price)
                    {
                        existing.Price = seed.Price;
                        changed = true;
                    }
                    if (existing.Currency != seed.Currency)
                    {
                        existing.Currency = seed.Currency;
                        changed = true;
                    }
                    if (existing.BillingInterval != seed.BillingInterval)
                    {
                        existing.BillingInterval = seed.BillingInterval;
                        changed = true;
                    }
                    if (existing.IsDeactivated != seed.IsDeactivated)
                    {
                        existing.IsDeactivated = seed.IsDeactivated;
                        changed = true;
                    }
                    return changed;
                },
                cancellationToken
            );

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
