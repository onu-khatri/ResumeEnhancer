using Microsoft.EntityFrameworkCore;
using ResumeEnhancer.BillingModule.DM.Entities;
using ResumeEnhancer.BillingModule.SL.Abstractions.Persistence;
using ResumeEnhancer.Infrastructure.Caching;
using ResumeEnhancer.Infrastructure.Persistence;

namespace ResumeEnhancer.BillingModule.PL.Repositories;

public sealed class BillingSetupDataRepository(
    IUnitOfWork<AppDbContext> unitOfWork,
    ICacheProvider cacheProvider) : IBillingSetupDataRepository
{
    internal const string BillingPlansCacheKey = "billing:setup:plans";

    private static readonly CacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12)
    };

    public Task<IReadOnlyList<BillingPlan>> ListBillingPlansAsync(CancellationToken cancellationToken = default) =>
        cacheProvider.GetOrSetAsync<IReadOnlyList<BillingPlan>>(
            BillingPlansCacheKey,
            async token => (IReadOnlyList<BillingPlan>)await unitOfWork.GetRepo<BillingPlan>()
                .Query()
                .AsNoTracking()
                .OrderBy(plan => plan.Order)
                .ThenBy(plan => plan.Code)
                .ToListAsync(token),
            CacheOptions,
            cancellationToken);
}
