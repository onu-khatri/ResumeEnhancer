using ResumeEnhancer.ProfilingModule.DM.Entities;

namespace ResumeEnhancer.Tests.Unit.Modules.AuthModule;

public sealed class ProfilingEntitlementOwnershipTests
{
    [Fact]
    public void Entitlements_can_represent_the_same_user_on_multiple_subscriptions()
    {
        var entitlements = new[]
        {
            new UserEntitlement
            {
                UserId = 7,
                AccessProfileId = 2,
                BillingSubscriptionId = 101,
                Enabled = true,
            },
            new UserEntitlement
            {
                UserId = 7,
                AccessProfileId = 3,
                BillingSubscriptionId = 102,
                Enabled = true,
            },
        };

        Assert.Equal(
            2,
            entitlements.Select(x => (x.UserId, x.BillingSubscriptionId)).Distinct().Count()
        );
        Assert.All(entitlements, entitlement => Assert.True(entitlement.AccessProfileId > 0));
    }
}
