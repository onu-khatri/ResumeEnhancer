using Microsoft.EntityFrameworkCore;
using ResumeEnhancer.BillingModule.DM.Entities;
using ResumeEnhancer.BillingModule.SL.Abstractions.Persistence;
using ResumeEnhancer.Infrastructure.Caching;
using ResumeEnhancer.Infrastructure.Persistence;

namespace ResumeEnhancer.BillingModule.PL.Repositories;

public sealed class BillingRepository : IBillingRepository
{
    private static readonly string[] SetupCacheKeys =
    [
        BillingSetupDataRepository.BillingPlansCacheKey
    ];

    private readonly IUnitOfWork<AppDbContext> _unitOfWork;
    private readonly ICacheProvider _cacheProvider;

    public BillingRepository(IUnitOfWork<AppDbContext> unitOfWork, ICacheProvider cacheProvider)
    {
        _unitOfWork = unitOfWork;
        _cacheProvider = cacheProvider;
    }

    public async Task<BillingAccount> AddBillingAccountAsync(BillingAccount account, int? auditUserId, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.GetRepo<BillingAccount>().AddAsync(account, cancellationToken);
        await SaveAsync(auditUserId, cancellationToken);
        return account;
    }

    public async Task<BillingAccount?> GetBillingAccountAsync(int billingAccountId, bool track = false, CancellationToken cancellationToken = default)
    {
        var query = _unitOfWork.GetRepo<BillingAccount>().Query();
        if (!track)
        {
            query = query.AsNoTracking();
        }

        return await query.SingleOrDefaultAsync(entity => entity.Id == billingAccountId, cancellationToken);
    }

    public async Task<IReadOnlyList<BillingAccount>> ListBillingAccountsAsync(CancellationToken cancellationToken = default) =>
        await _unitOfWork.GetRepo<BillingAccount>().Query().AsNoTracking().OrderBy(entity => entity.AccountNumber).ToListAsync(cancellationToken);

    public async Task DeleteBillingAccountAsync(BillingAccount account, int? auditUserId, CancellationToken cancellationToken = default)
    {
        _unitOfWork.DbContext.Remove(account);
        await SaveAsync(auditUserId, cancellationToken);
    }

    public async Task<BillingPlan> AddBillingPlanAsync(BillingPlan plan, int? auditUserId, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.GetRepo<BillingPlan>().AddAsync(plan, cancellationToken);
        await SaveAsync(auditUserId, cancellationToken);
        return plan;
    }

    public async Task<BillingPlan?> GetBillingPlanAsync(int billingPlanId, bool track = false, CancellationToken cancellationToken = default)
    {
        var query = _unitOfWork.GetRepo<BillingPlan>().Query();
        if (!track)
        {
            query = query.AsNoTracking();
        }

        return await query.SingleOrDefaultAsync(entity => entity.Id == billingPlanId, cancellationToken);
    }

    public async Task<IReadOnlyList<BillingPlan>> ListBillingPlansAsync(CancellationToken cancellationToken = default) =>
        await _unitOfWork.GetRepo<BillingPlan>().Query().AsNoTracking().OrderBy(entity => entity.Code).ToListAsync(cancellationToken);

    public async Task DeleteBillingPlanAsync(BillingPlan plan, int? auditUserId, CancellationToken cancellationToken = default)
    {
        _unitOfWork.DbContext.Remove(plan);
        await SaveAsync(auditUserId, cancellationToken);
    }

    public async Task<BillingSubscription> AddBillingSubscriptionAsync(BillingSubscription subscription, int? auditUserId, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.GetRepo<BillingSubscription>().AddAsync(subscription, cancellationToken);
        await SaveAsync(auditUserId, cancellationToken);
        return subscription;
    }

    public async Task<BillingSubscription?> GetBillingSubscriptionAsync(int billingSubscriptionId, bool track = false, CancellationToken cancellationToken = default)
    {
        var query = _unitOfWork.GetRepo<BillingSubscription>().Query();
        if (!track)
        {
            query = query.AsNoTracking();
        }

        return await query.SingleOrDefaultAsync(entity => entity.Id == billingSubscriptionId, cancellationToken);
    }

    public async Task<IReadOnlyList<BillingSubscription>> ListBillingSubscriptionsAsync(CancellationToken cancellationToken = default) =>
        await _unitOfWork.GetRepo<BillingSubscription>().Query().AsNoTracking().OrderByDescending(entity => entity.StartDateUtc).ToListAsync(cancellationToken);

    public async Task DeleteBillingSubscriptionAsync(BillingSubscription subscription, int? auditUserId, CancellationToken cancellationToken = default)
    {
        _unitOfWork.DbContext.Remove(subscription);
        await SaveAsync(auditUserId, cancellationToken);
    }

    public async Task SaveAsync(int? auditUserId, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.SaveAsync(new RepositoryAudit(auditUserId), cancellationToken);
        await InvalidateSetupCacheAsync(cancellationToken);
    }

    private async Task InvalidateSetupCacheAsync(CancellationToken cancellationToken)
    {
        foreach (var cacheKey in SetupCacheKeys)
        {
            await _cacheProvider.RemoveAsync(cacheKey, cancellationToken);
        }
    }

    private sealed class RepositoryAudit : IAudit
    {
        public RepositoryAudit(int? userId) => UserId = userId;
        public int? UserId { get; }
    }
}
