using ResumeEnhancer.BillingModule.DM.Entities;

namespace ResumeEnhancer.BillingModule.SL.Abstractions.Persistence;

public interface IBillingSetupDataRepository
{
    Task<IReadOnlyList<BillingPlan>> ListBillingPlansAsync(CancellationToken cancellationToken = default);
}
