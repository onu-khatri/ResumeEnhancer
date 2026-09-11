namespace ResumeEnhancer.BillingModule.SL.Integrations;

public sealed record BillingRegistrationSnapshot(
    int BillingSubscriptionId,
    int BillingPlanId,
    int AccessProfileId,
    string PlanCode
);

public sealed record BillingSubscriptionSnapshot(
    int BillingSubscriptionId,
    int BillingPlanId,
    int? AccessProfileId,
    string PlanCode
);

public interface IBillingRegistrationService
{
    public Task<BillingRegistrationSnapshot?> AddStarterRegistrationBillingAsync(
        int userId,
        int accessProfileId,
        CancellationToken cancellationToken = default
    );
    public Task<IReadOnlyList<BillingSubscriptionSnapshot>> ListActiveSubscriptionSnapshotsAsync(
        int userId,
        CancellationToken cancellationToken = default
    );
}
