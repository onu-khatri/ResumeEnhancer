using Microsoft.EntityFrameworkCore;
using ResumeEnhancer.BillingModule.DM.Entities;
using ResumeEnhancer.BillingModule.PL.Integrations;
using ResumeEnhancer.Tests.Unit.TestInfrastructure;
using Shouldly;

namespace ResumeEnhancer.Tests.Unit.Modules.BillingModule.Persistence;

public sealed class BillingRegistrationServiceTests
{
    [Fact]
    public async Task AddStarterRegistrationBillingAsync_CreatesAccountSubscriptionAndSnapshot()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;

        var result = await new BillingRegistrationService(scope.UnitOfWork)
            .AddStarterRegistrationBillingAsync(ResumeTestData.UserId, cancellationToken);

        result.ShouldNotBeNull();
        result.BillingPlanId.ShouldBe(ResumeTestData.BillingPlanId);
        result.AccessProfileId.ShouldBe(ResumeTestData.AccessProfileId);
        result.PlanCode.ShouldBe("FREE");
        (await scope.DbContext.Set<BillingAccount>().CountAsync(account => account.UserId == ResumeTestData.UserId, cancellationToken))
            .ShouldBe(1);
        (await scope.DbContext.Set<BillingSubscription>().CountAsync(subscription => subscription.UserId == ResumeTestData.UserId, cancellationToken))
            .ShouldBe(1);
    }

    [Theory]
    [InlineData("missing-plan")]
    [InlineData("invalid-access-profile")]
    [InlineData("missing-account-status")]
    [InlineData("missing-subscription-status")]
    public async Task AddStarterRegistrationBillingAsync_WhenPrerequisiteIsMissing_ReturnsNullAndWritesNothing(string scenario)
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        switch (scenario)
        {
            case "missing-plan":
                scope.DbContext.Remove(await scope.DbContext.Set<BillingPlan>().SingleAsync(plan => plan.Code == "FREE", cancellationToken));
                break;
            case "invalid-access-profile":
                var plan = await scope.DbContext.Set<BillingPlan>().SingleAsync(plan => plan.Code == "FREE", cancellationToken);
                plan.AccessProfileId = 0;
                break;
            case "missing-account-status":
                scope.DbContext.Remove(await scope.DbContext.Set<BillingAccountStatus>().SingleAsync(status => status.Code == "Active", cancellationToken));
                break;
            case "missing-subscription-status":
                scope.DbContext.Remove(await scope.DbContext.Set<BillingSubscriptionStatus>().SingleAsync(status => status.Code == "Active", cancellationToken));
                break;
        }
        if (scenario != "invalid-access-profile")
        {
            await scope.DbContext.SaveChangesAsync(cancellationToken);
        }
        if (scenario != "invalid-access-profile")
        {
            scope.DbContext.ChangeTracker.Clear();
        }

        var result = await new BillingRegistrationService(scope.UnitOfWork)
            .AddStarterRegistrationBillingAsync(ResumeTestData.UserId, cancellationToken);

        result.ShouldBeNull();
        (await scope.DbContext.Set<BillingAccount>().AnyAsync(account => account.UserId == ResumeTestData.UserId, cancellationToken))
            .ShouldBeFalse();
        (await scope.DbContext.Set<BillingSubscription>().AnyAsync(subscription => subscription.UserId == ResumeTestData.UserId, cancellationToken))
            .ShouldBeFalse();
    }

    [Fact]
    public async Task ListActiveSubscriptionSnapshotsAsync_ProjectsOnlyCurrentUserActiveSubscriptions()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        scope.DbContext.AddRange(
            new BillingSubscriptionStatus { Id = 2, Code = "Cancelled", Description = "Cancelled", DisplayName = "Cancelled", Guid = Guid.NewGuid(), Order = 2 },
            ResumeTestData.BillingPlan(id: 2, code: "PRO", order: 2),
            new BillingAccount { Id = 2, UserId = ResumeTestData.UserId, AccountNumber = "ACC-SNAPSHOT", StatusId = 1 },
            new BillingAccount { Id = 3, UserId = ResumeTestData.OtherUserId, AccountNumber = "ACC-OTHER", StatusId = 1 });
        await scope.DbContext.SaveChangesAsync(cancellationToken);
        scope.DbContext.AddRange(
            new BillingSubscription { BillingAccountId = 2, UserId = ResumeTestData.UserId, BillingPlanId = 2, StatusId = 1, EndDateUtc = null },
            new BillingSubscription { BillingAccountId = 2, UserId = ResumeTestData.UserId, BillingPlanId = 2, StatusId = 1, EndDateUtc = DateTime.UtcNow.AddDays(1) },
            new BillingSubscription { BillingAccountId = 2, UserId = ResumeTestData.UserId, BillingPlanId = 2, StatusId = 1, EndDateUtc = DateTime.UtcNow.AddDays(-1) },
            new BillingSubscription { BillingAccountId = 2, UserId = ResumeTestData.UserId, BillingPlanId = 2, StatusId = 2, EndDateUtc = null },
            new BillingSubscription { BillingAccountId = 3, UserId = ResumeTestData.OtherUserId, BillingPlanId = 2, StatusId = 1, EndDateUtc = null });
        await scope.DbContext.SaveChangesAsync(cancellationToken);

        var result = await new BillingRegistrationService(scope.UnitOfWork)
            .ListActiveSubscriptionSnapshotsAsync(ResumeTestData.UserId, cancellationToken);

        result.Count.ShouldBe(2);
        result.ShouldAllBe(snapshot => snapshot.BillingPlanId == 2 && snapshot.AccessProfileId == ResumeTestData.AccessProfileId && snapshot.PlanCode == "PRO");
    }
}
