using ResumeEnhancer.BillingModule.DM.Entities;

namespace ResumeEnhancer.BillingModule.SL.Abstractions.Persistence;

public interface IBillingRepository
{
    Task<BillingAccount> AddBillingAccountAsync(BillingAccount account, int? auditUserId, CancellationToken cancellationToken = default);
    Task<BillingAccount?> GetBillingAccountAsync(int billingAccountId, bool track = false, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BillingAccount>> ListBillingAccountsAsync(CancellationToken cancellationToken = default);
    Task DeleteBillingAccountAsync(BillingAccount account, int? auditUserId, CancellationToken cancellationToken = default);

    Task<BillingPlan> AddBillingPlanAsync(BillingPlan plan, int? auditUserId, CancellationToken cancellationToken = default);
    Task<BillingPlan?> GetBillingPlanAsync(int billingPlanId, bool track = false, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BillingPlan>> ListBillingPlansAsync(CancellationToken cancellationToken = default);
    Task DeleteBillingPlanAsync(BillingPlan plan, int? auditUserId, CancellationToken cancellationToken = default);

    Task<BillingSubscription> AddBillingSubscriptionAsync(BillingSubscription subscription, int? auditUserId, CancellationToken cancellationToken = default);
    Task<BillingSubscription?> GetBillingSubscriptionAsync(int billingSubscriptionId, bool track = false, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BillingSubscription>> ListBillingSubscriptionsAsync(CancellationToken cancellationToken = default);
    Task DeleteBillingSubscriptionAsync(BillingSubscription subscription, int? auditUserId, CancellationToken cancellationToken = default);

    Task SaveAsync(int? auditUserId, CancellationToken cancellationToken = default);
}
