using System.Net;
using System.Net.Http.Json;
using ResumeEnhancer.BillingModule.AM.Requests;
using ResumeEnhancer.TestUtilities.IntegrationSupport;
using Shouldly;
using ResumeEnhancer.Tests.Integration.Modules.BillingModule.TestSupport;

namespace ResumeEnhancer.Tests.Integration.Modules.BillingModule;

[Collection("Sequential_BillingModule")]
public sealed class BillingPlanIntegrationTests(BillingModuleIntegrationTestFixture fixture)
{
    public static TheoryData<string, string?, bool, HttpStatusCode> PlanUpdateCases => new()
    {
        { "anonymous", null, false, HttpStatusCode.Unauthorized },
        { "authenticated-without-admin-capability", "OtherCapability", false, HttpStatusCode.Forbidden },
        { "admin-without-cascade", "ViewAdminPortal", false, HttpStatusCode.OK },
        { "admin-with-cascade", "ViewAdminPortal", true, HttpStatusCode.OK },
    };

    [Theory]
    [MemberData(nameof(PlanUpdateCases))]
    public async Task UpdatePlan_enforces_authorization_and_cascade_option(
        string _, string? capability, bool cascade, HttpStatusCode expected)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await fixture.ResetAndSeedAsync(cancellationToken);
        if (capability is not null)
            await fixture.CreateSetupper().SetupAccessAsync(1, privileges: capability);

        using var client = fixture.Utilities.CreateClient();
        using var response = await client.PutAsJsonAsync(
            "/api/billing/plans/1",
            new UpdateBillingPlanRequest
            {
                Code = "FREE",
                Description = "Free starter plan",
                DisplayName = "Free",
                Price = 0,
                CurrencyId = 1,
                BillingIntervalId = 1,
                AccessProfileId = 2,
                CascadeExistingSubscriptions = cascade
            },
            cancellationToken);

        response.StatusCode.ShouldBe(expected);
    }

    [Fact]
    public async Task Authorized_cascade_request_is_safe_to_retry()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await fixture.ResetAndSeedAsync(cancellationToken);
        await fixture.CreateSetupper().SetupAccessAsync(1, privileges: "ViewAdminPortal");
        var request = new UpdateBillingPlanRequest
        {
            Code = "FREE",
            Description = "Free starter plan",
            DisplayName = "Free",
            Price = 0,
            CurrencyId = 1,
            BillingIntervalId = 1,
            AccessProfileId = 2,
            CascadeExistingSubscriptions = true
        };

        using var first = await fixture.Utilities.CreateClient().PutAsJsonAsync("/api/billing/plans/1", request, cancellationToken);
        using var second = await fixture.Utilities.CreateClient().PutAsJsonAsync("/api/billing/plans/1", request, cancellationToken);

        first.StatusCode.ShouldBe(HttpStatusCode.OK);
        second.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
