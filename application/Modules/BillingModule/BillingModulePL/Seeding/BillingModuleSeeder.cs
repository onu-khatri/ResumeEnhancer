using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ResumeEnhancer.BillingModule.DM.Entities;
using ResumeEnhancer.Core.DomainLibrary.DomainModel;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.ProfilingModule.DM.Entities;

namespace ResumeEnhancer.BillingModule.PL.Seeding;

[ExcludeFromCodeCoverage]
public sealed class BillingModuleSeeder : IAppDbContextSeeder
{
    public async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        await dbContext.Set<Currency>().SeedSetupDataAsync(
            [
                new Currency { Code = "USD", Description = "United States dollar", DisplayName = "US Dollar", Order = 1, Guid = Guid.Parse("44444444-4444-4444-4444-444444444501") },
                new Currency { Code = "EUR", Description = "Euro", DisplayName = "Euro", Order = 2, Guid = Guid.Parse("44444444-4444-4444-4444-444444444502") },
            ], UpdateSetupValues, cancellationToken);

        await dbContext.Set<BillingInterval>().SeedSetupDataAsync(
            [
                new BillingInterval { Code = "Monthly", Description = "Billed every month", DisplayName = "Monthly", Order = 1, Guid = Guid.Parse("44444444-4444-4444-4444-444444444511") },
                new BillingInterval { Code = "Yearly", Description = "Billed every year", DisplayName = "Yearly", Order = 2, Guid = Guid.Parse("44444444-4444-4444-4444-444444444512") },
            ], UpdateSetupValues, cancellationToken);

        await dbContext.Set<BillingAccountStatus>().SeedSetupDataAsync(
            [
                new BillingAccountStatus { Code = "Active", Description = "Account is active", DisplayName = "Active", Order = 1, Guid = Guid.Parse("44444444-4444-4444-4444-444444444521") },
                new BillingAccountStatus { Code = "Suspended", Description = "Account is suspended", DisplayName = "Suspended", Order = 2, Guid = Guid.Parse("44444444-4444-4444-4444-444444444522") },
                new BillingAccountStatus { Code = "Closed", Description = "Account is closed", DisplayName = "Closed", Order = 3, Guid = Guid.Parse("44444444-4444-4444-4444-444444444523") },
            ], UpdateSetupValues, cancellationToken);

        await dbContext.Set<BillingSubscriptionStatus>().SeedSetupDataAsync(
            [
                new BillingSubscriptionStatus { Code = "Active", Description = "Subscription is active", DisplayName = "Active", Order = 1, Guid = Guid.Parse("44444444-4444-4444-4444-444444444531") },
                new BillingSubscriptionStatus { Code = "Cancelled", Description = "Subscription is cancelled", DisplayName = "Cancelled", Order = 2, Guid = Guid.Parse("44444444-4444-4444-4444-444444444532") },
                new BillingSubscriptionStatus { Code = "Expired", Description = "Subscription is expired", DisplayName = "Expired", Order = 3, Guid = Guid.Parse("44444444-4444-4444-4444-444444444533") },
            ], UpdateSetupValues, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        var usdId = await GetSetupIdAsync<Currency>(dbContext, "USD", cancellationToken);
        var monthlyId = await GetSetupIdAsync<BillingInterval>(dbContext, "Monthly", cancellationToken);
        var guestProfileId = await GetSetupIdAsync<AccessProfile>(dbContext, "Guest", cancellationToken);
        var limitedUserProfileId = await GetSetupIdAsync<AccessProfile>(dbContext, "LimitedUser", cancellationToken);

        await dbContext.Set<BillingPlan>().SeedSetupDataAsync(
            [
                new BillingPlan { Code = "FREE", Description = "Free starter plan", DisplayName = "Free", Order = 1, Price = 0, CurrencyId = usdId, BillingIntervalId = monthlyId, AccessProfileId = guestProfileId, Guid = Guid.Parse("44444444-4444-4444-4444-444444444401"), IsDeactivated = false },
                new BillingPlan { Code = "PRO", Description = "Professional plan", DisplayName = "Pro", Order = 2, Price = 19.99m, CurrencyId = usdId, BillingIntervalId = monthlyId, AccessProfileId = limitedUserProfileId, Guid = Guid.Parse("44444444-4444-4444-4444-444444444402"), IsDeactivated = false },
            ],
            (existing, seed) =>
            {
                var changed = UpdateSetupValues(existing, seed);
                if (existing.DisplayName != seed.DisplayName) { existing.DisplayName = seed.DisplayName; changed = true; }
                if (existing.Price != seed.Price) { existing.Price = seed.Price; changed = true; }
                if (existing.CurrencyId != seed.CurrencyId) { existing.CurrencyId = seed.CurrencyId; changed = true; }
                if (existing.BillingIntervalId != seed.BillingIntervalId) { existing.BillingIntervalId = seed.BillingIntervalId; changed = true; }
                if (existing.AccessProfileId != seed.AccessProfileId) { existing.AccessProfileId = seed.AccessProfileId; changed = true; }
                if (existing.IsDeactivated != seed.IsDeactivated) { existing.IsDeactivated = seed.IsDeactivated; changed = true; }
                return changed;
            }, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static bool UpdateSetupValues<T>(T existing, T seed)
        where T : SetupEntity, IHasOrderedValues
    {
        var changed = false;
        if (existing.Description != seed.Description) { existing.Description = seed.Description; changed = true; }
        if (existing.Order != seed.Order) { existing.Order = seed.Order; changed = true; }
        return changed;
    }

    private static async Task<int> GetSetupIdAsync<T>(AppDbContext dbContext, string code, CancellationToken cancellationToken)
        where T : SetupEntity
    {
        var id = await dbContext.Set<T>()
            .Where(entity => entity.Code == code && !entity.ObsoleteFlag)
            .Select(entity => (int?)entity.Id)
            .SingleOrDefaultAsync(cancellationToken);

        return id ?? throw new InvalidOperationException($"Required billing setup value '{typeof(T).Name}:{code}' was not seeded.");
    }
}
