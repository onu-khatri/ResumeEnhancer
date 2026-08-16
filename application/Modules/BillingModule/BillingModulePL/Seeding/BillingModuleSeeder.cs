using Microsoft.EntityFrameworkCore;
using ResumeEnhancer.BillingModule.DM.Entities;
using ResumeEnhancer.Infrastructure.Persistence;

namespace ResumeEnhancer.BillingModule.PL.Seeding;

public sealed class BillingModuleSeeder : IAppDbContextSeeder
{
    public async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (!await dbContext.Set<BillingPlan>().AnyAsync(cancellationToken))
        {
            dbContext.Set<BillingPlan>().AddRange(
                new BillingPlan
                {
                    Code = "FREE",
                    Description = "Free starter plan",
                    DisplayName = "Free",
                    Order = 1,
                    Price = 0,
                    Currency = "USD",
                    BillingInterval = "Monthly",
                    Guid = Guid.NewGuid(),
                    IsDeactivated = false
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
                    Guid = Guid.NewGuid(),
                    IsDeactivated = false
                });
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
