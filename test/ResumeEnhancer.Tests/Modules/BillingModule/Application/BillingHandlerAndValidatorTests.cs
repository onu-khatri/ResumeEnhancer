using NSubstitute;
using ResumeEnhancer.BillingModule.AM.Requests;
using ResumeEnhancer.BillingModule.DM.Entities;
using ResumeEnhancer.BillingModule.SL.Abstractions.Persistence;
using ResumeEnhancer.BillingModule.SL.Handlers;
using ResumeEnhancer.BillingModule.Web.Validation;
using ResumeEnhancer.ProfilingModule.SL.Integrations;
using ResumeEnhancer.ResumeModule.SL.Integrations;
using Shouldly;

namespace ResumeEnhancer.Tests.Unit.Modules.BillingModule.Application;

public sealed class BillingHandlerAndValidatorTests
{
    [Fact]
    public async Task BillingAccountHandlers_CoverCrudFlows()
    {
        var token = TestContext.Current.CancellationToken;
        var repository = Substitute.For<IBillingRepository>();
        var users = Substitute.For<IUserLookupService>();
        users.UserExistsAsync(21, token).Returns(true);
        repository.GetBillingAccountAsync(4, true, token).Returns(Account(4));
        repository.GetBillingAccountAsync(9, true, token).Returns((BillingAccount?)null);
        repository.GetBillingAccountAsync(4, false, token).Returns(Account(4));
        repository.ListBillingAccountsAsync(token).Returns([Account(4)]);

        var created = await new CreateBillingAccountCommandHandler(repository, users).Handle(
            new(new CreateBillingAccountRequest { UserId = 21, AccountNumber = "ACC-1", StatusId = 1 }, 7), token);
        var updated = await new UpdateBillingAccountCommandHandler(repository, users).Handle(
            new(4, new UpdateBillingAccountRequest { UserId = 21, AccountNumber = "ACC-2", StatusId = 1 }, 8), token);
        var missing = await new UpdateBillingAccountCommandHandler(repository, users).Handle(
            new(9, new UpdateBillingAccountRequest { UserId = 21, AccountNumber = "ACC-3", StatusId = 1 }, 8), token);
        var deleted = await new DeleteBillingAccountCommandHandler(repository).Handle(new(4, 9), token);
        var detail = await new GetBillingAccountQueryHandler(repository).Handle(new(4), token);
        var list = await new ListBillingAccountsQueryHandler(repository).Handle(new(), token);

        created.AccountNumber.ShouldBe("ACC-1");
        updated.ShouldNotBeNull();
        missing.ShouldBeNull();
        deleted.ShouldBeTrue();
        detail.ShouldNotBeNull();
        list.ShouldHaveSingleItem();
    }

    [Fact]
    public async Task BillingPlanHandlers_UpdateCascadeAndMissingBranches()
    {
        var token = TestContext.Current.CancellationToken;
        var repository = Substitute.For<IBillingRepository>();
        var profiling = Substitute.For<IProfilingRegistrationService>();
        var existing = Plan(5, 2);
        repository.GetBillingPlanAsync(5, true, token).Returns(existing);
        repository.GetBillingPlanAsync(99, true, token).Returns((BillingPlan?)null);
        repository.GetBillingPlanAsync(5, false, token).Returns(existing);
        repository.ListBillingPlansAsync(token).Returns([existing]);
        repository.ListActiveSubscriptionsForPlanAsync(5, token)
            .Returns([new BillingSubscription { Id = 8, UserId = 21, BillingPlanId = 5 }]);

        var updated = await new UpdateBillingPlanCommandHandler(repository, profiling).Handle(
            new(5,
                new UpdateBillingPlanRequest
                {
                    Code = "PRO", Description = "Pro", DisplayName = "Pro", Price = 10, CurrencyId = 1,
                    BillingIntervalId = 1, AccessProfileId = 3, CascadeExistingSubscriptions = true
                }, 4), token);
        var missing = await new UpdateBillingPlanCommandHandler(repository, profiling).Handle(
            new(99,
                new UpdateBillingPlanRequest
                {
                    Code = "X", Description = "X", DisplayName = "X", CurrencyId = 1, BillingIntervalId = 1,
                    AccessProfileId = 1
                }, 4), token);
        var detail = await new GetBillingPlanQueryHandler(repository).Handle(new(5), token);
        var list = await new ListBillingPlansQueryHandler(repository).Handle(new(), token);

        updated.ShouldNotBeNull();
        missing.ShouldBeNull();
        detail.ShouldNotBeNull();
        list.ShouldHaveSingleItem();
        await profiling.Received(1).ReviseAccessProfilesAsync(
            Arg.Is<IReadOnlyCollection<AccessProfileRevisionInput>>(items =>
                items.Count == 1 && items.Single().UserId == 21), token);
    }

    [Fact]
    public async Task BillingSubscriptionHandlers_CoverCrudFlows()
    {
        var token = TestContext.Current.CancellationToken;
        var repository = Substitute.For<IBillingRepository>();
        repository.GetBillingSubscriptionAsync(6, true, token).Returns(Subscription(6));
        repository.GetBillingSubscriptionAsync(98, true, token).Returns((BillingSubscription?)null);
        repository.GetBillingSubscriptionAsync(6, false, token).Returns(Subscription(6));
        repository.ListBillingSubscriptionsAsync(token).Returns([Subscription(6)]);

        var created = await new CreateBillingSubscriptionCommandHandler(repository).Handle(
            new(
                new CreateBillingSubscriptionRequest
                    { BillingAccountId = 1, UserId = 9, BillingPlanId = 2, StatusId = 1 }, 7), token);
        var updated = await new UpdateBillingSubscriptionCommandHandler(repository).Handle(
            new(6,
                new UpdateBillingSubscriptionRequest
                    { BillingAccountId = 1, UserId = 9, BillingPlanId = 2, StatusId = 1 }, 8), token);
        var missing = await new UpdateBillingSubscriptionCommandHandler(repository).Handle(
            new(98,
                new UpdateBillingSubscriptionRequest
                    { BillingAccountId = 1, UserId = 9, BillingPlanId = 2, StatusId = 1 }, 8), token);
        var deleted = await new DeleteBillingSubscriptionCommandHandler(repository).Handle(new(6, 5), token);
        var detail = await new GetBillingSubscriptionQueryHandler(repository).Handle(new(6), token);
        var list = await new ListBillingSubscriptionsQueryHandler(repository).Handle(new(), token);

        created.ShouldNotBeNull();
        updated.ShouldNotBeNull();
        missing.ShouldBeNull();
        deleted.ShouldBeTrue();
        detail.ShouldNotBeNull();
        list.ShouldHaveSingleItem();
    }

    [Fact]
    public void BillingValidators_RejectInvalidRequests()
    {
        new CreateBillingAccountRequestValidator()
            .Validate(new CreateBillingAccountRequest { UserId = 0, AccountNumber = "", StatusId = 0 }).Errors
            .ShouldNotBeEmpty();
        new UpdateBillingAccountRequestValidator()
            .Validate(new UpdateBillingAccountRequest { UserId = 0, AccountNumber = "", StatusId = 0 }).Errors
            .ShouldNotBeEmpty();
        new CreateBillingPlanRequestValidator().Validate(new CreateBillingPlanRequest
        {
            Code = "", Description = "", DisplayName = "", Price = -1, CurrencyId = 0, BillingIntervalId = 0,
            AccessProfileId = 0
        }).Errors.ShouldNotBeEmpty();
        new UpdateBillingPlanRequestValidator().Validate(new UpdateBillingPlanRequest
        {
            Code = "", Description = "", DisplayName = "", Price = -1, CurrencyId = 0, BillingIntervalId = 0,
            AccessProfileId = 0
        }).Errors.ShouldNotBeEmpty();
        new CreateBillingSubscriptionRequestValidator().Validate(new CreateBillingSubscriptionRequest
            { BillingAccountId = 0, UserId = 0, BillingPlanId = 0, StatusId = 0 }).Errors.ShouldNotBeEmpty();
        new UpdateBillingSubscriptionRequestValidator().Validate(new UpdateBillingSubscriptionRequest
            { BillingAccountId = 0, UserId = 0, BillingPlanId = 0, StatusId = 0 }).Errors.ShouldNotBeEmpty();
    }

    private static BillingAccount Account(int id) => new()
    {
        Id = id, UserId = 21, AccountNumber = "ACC-1", StatusId = 1,
        Status = new BillingAccountStatus { Id = 1, Code = "Active" }
    };

    private static BillingPlan Plan(int id, int accessProfileId) => new()
    {
        Id = id, Code = "PRO", Description = "Pro", DisplayName = "Pro", Price = 10, CurrencyId = 1,
        BillingIntervalId = 1, AccessProfileId = accessProfileId, Currency = new Currency { Id = 1, Code = "USD" },
        BillingInterval = new BillingInterval { Id = 1, Code = "Monthly" }
    };

    private static BillingSubscription Subscription(int id) => new()
    {
        Id = id, BillingAccountId = 1, UserId = 9, BillingPlanId = 2, StatusId = 1,
        Status = new BillingSubscriptionStatus { Id = 1, Code = "Active" }, StartDateUtc = DateTime.UtcNow
    };
}
