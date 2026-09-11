using ResumeEnhancer.AuthModule.SL.Abstractions;
using ResumeEnhancer.BillingModule.SL.Integrations;
using ResumeEnhancer.ProfilingModule.SL.Integrations;

namespace ResumeEnhancer.AuthModule.SL.Services;

internal sealed class SubscriptionEntitlementResolver(
    IBillingRegistrationService billing,
    IProfilingRegistrationService profiling
) : IEntitlementResolver
{
    public async Task<IReadOnlySet<string>> ResolveAsync(
        int userId,
        CancellationToken cancellationToken = default
    )
    {
        var subscriptions = await billing.ListActiveSubscriptionSnapshotsAsync(
            userId,
            cancellationToken
        );
        var capabilities = new HashSet<string>(StringComparer.Ordinal);
        foreach (var subscription in subscriptions)
        {
            if (subscription.AccessProfileId is null)
                continue;
            var accessShape = await profiling.GetAccessShapeAsync(
                subscription.AccessProfileId.Value,
                cancellationToken
            );
            capabilities.UnionWith(accessShape.Capabilities);
        }
        if (capabilities.Count > 0)
            return capabilities;
        return (await profiling.GetStarterAccessShapeAsync(cancellationToken)).Capabilities;
    }
}
