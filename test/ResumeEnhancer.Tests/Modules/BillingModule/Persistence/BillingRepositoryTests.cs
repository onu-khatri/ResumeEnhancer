using Microsoft.EntityFrameworkCore;
using NSubstitute;
using ResumeEnhancer.BillingModule.DM.Entities;
using ResumeEnhancer.BillingModule.PL.Repositories;
using ResumeEnhancer.Infrastructure.Caching;
using ResumeEnhancer.Tests.Unit.TestInfrastructure;
using Shouldly;

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
        var account = new BillingAccount
        {
            UserId = ResumeTestData.UserId,
            AccountNumber = "ACC-002",
            StatusId = 1,
            ExternalReference = "ext",
        };
        var plan = ResumeTestData.BillingPlan(id: 2, code: "PRO", order: 2);

        var addedAccount = await repository.AddBillingAccountAsync(account, 77, cancellationToken);
        var addedPlan = await repository.AddBillingPlanAsync(plan, 77, cancellationToken);
        var addedSubscription = await repository.AddBillingSubscriptionAsync(
            new BillingSubscription
            {
                BillingAccountId = addedAccount.Id,
                UserId = ResumeTestData.UserId,
                BillingPlanId = addedPlan.Id,
                StatusId = 1,
                StartDateUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            77,
            cancellationToken
        );

        scope.DbContext.ChangeTracker.Clear();

        (
            await repository.GetBillingAccountAsync(
                addedAccount.Id,
                track: false,
                cancellationToken
            )
        )!.AccountNumber.ShouldBe("ACC-002");
        scope
            .DbContext.Entry(
                (
                    await repository.GetBillingAccountAsync(
                        addedAccount.Id,
                        track: false,
                        cancellationToken
                    )
                )!
            )
            .State.ShouldBe(EntityState.Detached);
        (
            await repository.GetBillingPlanAsync(addedPlan.Id, track: true, cancellationToken)
        )!.Code.ShouldBe("PRO");
        (
            await repository.GetBillingSubscriptionAsync(
                addedSubscription.Id,
                track: true,
                cancellationToken
            )
        )!.UserId.ShouldBe(ResumeTestData.UserId);
        (await repository.ListBillingAccountsAsync(cancellationToken)).ShouldContain(item =>
            item.AccountNumber == "ACC-002"
        );
        (await repository.ListBillingPlansAsync(cancellationToken))
            .Select(item => item.Code)
            .ShouldContain("PRO");
        (await repository.ListBillingSubscriptionsAsync(cancellationToken))
            .Select(item => item.Id)
            .ShouldContain(addedSubscription.Id);

        await repository.DeleteBillingSubscriptionAsync(
            (
                await repository.GetBillingSubscriptionAsync(
                    addedSubscription.Id,
                    true,
                    cancellationToken
                )
            )!,
            88,
            cancellationToken
        );
        await repository.DeleteBillingPlanAsync(
            (await repository.GetBillingPlanAsync(addedPlan.Id, true, cancellationToken))!,
            88,
            cancellationToken
        );
        await repository.DeleteBillingAccountAsync(
            (await repository.GetBillingAccountAsync(addedAccount.Id, true, cancellationToken))!,
            88,
            cancellationToken
        );

        (
            await scope
                .DbContext.Set<BillingSubscription>()
                .AnyAsync(item => item.Id == addedSubscription.Id, cancellationToken)
        ).ShouldBeFalse();
        (
            await scope
                .DbContext.Set<BillingPlan>()
                .AnyAsync(item => item.Id == addedPlan.Id, cancellationToken)
        ).ShouldBeFalse();
        (
            await scope
                .DbContext.Set<BillingAccount>()
                .AnyAsync(item => item.Id == addedAccount.Id, cancellationToken)
        ).ShouldBeFalse();
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
                cancellationToken
            )
            .Returns(call =>
                call.Arg<Func<CancellationToken, Task<IReadOnlyList<BillingPlan>>>>()(
                    cancellationToken
                )
            );
        scope.DbContext.Add(ResumeTestData.BillingPlan(id: 3, code: "BASIC", order: 0));
        await scope.DbContext.SaveChangesAsync(new TestAudit(1), cancellationToken);

        var result = await new BillingSetupDataRepository(
            scope.UnitOfWork,
            cacheProvider
        ).ListBillingPlansAsync(cancellationToken);

        result.Select(item => item.Code).ShouldBe(["BASIC", "FREE"]);
    }

    [Fact]
    public async Task BillingRepository_MissingEntitiesAndEmptyDatabaseReturnExpectedResults()
    {
        using var scope = new SqliteAppDbContextScope();
        var repository = new BillingRepository(scope.UnitOfWork, CreateCacheProvider());
        var cancellationToken = TestContext.Current.CancellationToken;

        (await repository.GetBillingAccountAsync(999, cancellationToken: cancellationToken)).ShouldBeNull();
        (await repository.GetBillingPlanAsync(999, cancellationToken: cancellationToken)).ShouldBeNull();
        (await repository.GetBillingSubscriptionAsync(999, cancellationToken: cancellationToken)).ShouldBeNull();
        (await repository.ListBillingAccountsAsync(cancellationToken)).ShouldBeEmpty();
        (await repository.ListBillingSubscriptionsAsync(cancellationToken)).ShouldBeEmpty();
    }

    [Fact]
    public async Task BillingRepository_ListsOnlyActiveAndUnendedSubscriptionsForPlan()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        var plan = ResumeTestData.BillingPlan(id: 2, code: "PRO", order: 2);
        scope.DbContext.Add(plan);
        scope.DbContext.AddRange(
            new BillingSubscriptionStatus { Id = 2, Code = "Cancelled", Description = "Cancelled", DisplayName = "Cancelled", Guid = Guid.NewGuid(), Order = 2 },
            new BillingAccount { Id = 2, UserId = ResumeTestData.OtherUserId, AccountNumber = "ACC-OTHER", StatusId = 1 },
            new BillingAccount { Id = 3, UserId = ResumeTestData.UserId, AccountNumber = "ACC-TARGET", StatusId = 1 });
        await scope.DbContext.SaveChangesAsync(cancellationToken);

        scope.DbContext.AddRange(
            new BillingSubscription { BillingAccountId = 3, UserId = ResumeTestData.UserId, BillingPlanId = 2, StatusId = 1, EndDateUtc = null },
            new BillingSubscription { BillingAccountId = 3, UserId = ResumeTestData.UserId, BillingPlanId = 2, StatusId = 1, EndDateUtc = DateTime.UtcNow.AddDays(1) },
            new BillingSubscription { BillingAccountId = 3, UserId = ResumeTestData.UserId, BillingPlanId = 2, StatusId = 1, EndDateUtc = DateTime.UtcNow.AddDays(-1) },
            new BillingSubscription { BillingAccountId = 3, UserId = ResumeTestData.UserId, BillingPlanId = 2, StatusId = 2, EndDateUtc = null },
            new BillingSubscription { BillingAccountId = 2, UserId = ResumeTestData.OtherUserId, BillingPlanId = 1, StatusId = 1, EndDateUtc = null });
        await scope.DbContext.SaveChangesAsync(cancellationToken);

        var result = await new BillingRepository(scope.UnitOfWork, CreateCacheProvider())
            .ListActiveSubscriptionsForPlanAsync(2, cancellationToken);

        result.Count.ShouldBe(2);
        result.ShouldAllBe(subscription => subscription.StatusId == 1 && subscription.BillingPlanId == 2);
    }

    [Fact]
    public async Task BillingRepository_CancellationIsPropagated()
    {
        using var scope = new SqliteAppDbContextScope();
        var repository = new BillingRepository(scope.UnitOfWork, CreateCacheProvider());
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Should.ThrowAsync<OperationCanceledException>(() =>
            repository.ListBillingPlansAsync(cancellation.Token));
    }

    [Fact]
    public async Task BillingRepository_CacheFailurePropagatesAfterSave()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        var cacheProvider = CreateCacheProvider();
        cacheProvider.RemoveAsync("billing:setup:plans", Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw new InvalidOperationException("cache unavailable"));
        var repository = new BillingRepository(scope.UnitOfWork, cacheProvider);

        await Should.ThrowAsync<InvalidOperationException>(() =>
            repository.AddBillingPlanAsync(ResumeTestData.BillingPlan(id: 2, code: "PRO"), 10, cancellationToken));
        (await scope.DbContext.Set<BillingPlan>().AnyAsync(plan => plan.Code == "PRO", cancellationToken))
            .ShouldBeTrue();
    }

    private static ICacheProvider CreateCacheProvider()
    {
        var cacheProvider = Substitute.For<ICacheProvider>();
        cacheProvider
            .RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        return cacheProvider;
    }
}
