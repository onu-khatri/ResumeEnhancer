using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shouldly;
using ResumeEnhancer.BillingModule.DM.Entities;
using ResumeEnhancer.BillingModule.PL.Repositories;
using ResumeEnhancer.Infrastructure.Caching;
using ResumeEnhancer.Tests.Unit.TestInfrastructure;

namespace ResumeEnhancer.Tests.Unit.Modules.BillingModule.Persistence;

public sealed class BillingRepositoryTests
{
    [Fact]
    public async Task BillingRepository_CoversCrudForAccountsPlansAndSubscriptions()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        var cacheProvider = CreateCacheProvider();
        var repository = new BillingRepository(scope.UnitOfWork, cacheProvider);
        var account = new BillingAccount { UserId = ResumeTestData.UserId, AccountNumber = "ACC-002", Status = "Active", ExternalReference = "ext" };
        var plan = ResumeTestData.BillingPlan(id: 2, code: "PRO", order: 2);

        var addedAccount = await repository.AddBillingAccountAsync(account, 77, cancellationToken);
        var addedPlan = await repository.AddBillingPlanAsync(plan, 77, cancellationToken);
        var addedSubscription = await repository.AddBillingSubscriptionAsync(
            new BillingSubscription
            {
                BillingAccountId = addedAccount.Id,
                BillingPlanId = addedPlan.Id,
                ResumeId = null,
                Status = "Active",
                StartDateUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            77,
            cancellationToken);

        scope.DbContext.ChangeTracker.Clear();

        (await repository.GetBillingAccountAsync(addedAccount.Id, track: false, cancellationToken))!.AccountNumber.ShouldBe("ACC-002");
        scope.DbContext.Entry((await repository.GetBillingAccountAsync(addedAccount.Id, track: false, cancellationToken))!).State.ShouldBe(EntityState.Detached);
        (await repository.GetBillingPlanAsync(addedPlan.Id, track: true, cancellationToken))!.Code.ShouldBe("PRO");
        (await repository.GetBillingSubscriptionAsync(addedSubscription.Id, track: true, cancellationToken))!.ResumeId.ShouldBeNull();
        (await repository.ListBillingAccountsAsync(cancellationToken)).ShouldContain(item => item.AccountNumber == "ACC-002");
        (await repository.ListBillingPlansAsync(cancellationToken)).Select(item => item.Code).ShouldContain("PRO");
        (await repository.ListBillingSubscriptionsAsync(cancellationToken)).Select(item => item.Id).ShouldContain(addedSubscription.Id);

        await repository.DeleteBillingSubscriptionAsync((await repository.GetBillingSubscriptionAsync(addedSubscription.Id, true, cancellationToken))!, 88, cancellationToken);
        await repository.DeleteBillingPlanAsync((await repository.GetBillingPlanAsync(addedPlan.Id, true, cancellationToken))!, 88, cancellationToken);
        await repository.DeleteBillingAccountAsync((await repository.GetBillingAccountAsync(addedAccount.Id, true, cancellationToken))!, 88, cancellationToken);

        (await scope.DbContext.Set<BillingSubscription>().AnyAsync(item => item.Id == addedSubscription.Id, cancellationToken)).ShouldBeFalse();
        (await scope.DbContext.Set<BillingPlan>().AnyAsync(item => item.Id == addedPlan.Id, cancellationToken)).ShouldBeFalse();
        (await scope.DbContext.Set<BillingAccount>().AnyAsync(item => item.Id == addedAccount.Id, cancellationToken)).ShouldBeFalse();
        await cacheProvider.Received().RemoveAsync("billing:setup:plans", cancellationToken);
    }

    [Fact]
    public async Task BillingSetupDataRepository_ListsPlansThroughCacheFactory()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        var cacheProvider = Substitute.For<ICacheProvider>();
        cacheProvider
            .GetOrSetAsync<IReadOnlyList<BillingPlan>>(
                "billing:setup:plans",
                Arg.Any<Func<CancellationToken, Task<IReadOnlyList<BillingPlan>>>>(),
                Arg.Any<CacheEntryOptions?>(),
                cancellationToken)
            .Returns(call => call.Arg<Func<CancellationToken, Task<IReadOnlyList<BillingPlan>>>>()(cancellationToken));
        scope.DbContext.Add(ResumeTestData.BillingPlan(id: 3, code: "BASIC", order: 0));
        await scope.DbContext.SaveChangesAsync(new TestAudit(1), cancellationToken);

        var result = await new BillingSetupDataRepository(scope.UnitOfWork, cacheProvider).ListBillingPlansAsync(cancellationToken);

        result.Select(item => item.Code).ShouldBe(["BASIC", "FREE"]);
    }

    private static ICacheProvider CreateCacheProvider()
    {
        var cacheProvider = Substitute.For<ICacheProvider>();
        cacheProvider.RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        return cacheProvider;
    }
}
