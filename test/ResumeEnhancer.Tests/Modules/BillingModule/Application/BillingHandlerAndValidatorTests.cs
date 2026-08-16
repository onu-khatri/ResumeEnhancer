using NSubstitute;
using Shouldly;
using ResumeEnhancer.BillingModule.AM.Requests;
using ResumeEnhancer.BillingModule.DM.Entities;
using ResumeEnhancer.BillingModule.SL.Abstractions.Persistence;
using ResumeEnhancer.BillingModule.SL.Handlers;
using ResumeEnhancer.BillingModule.Web.Validation;
using ResumeEnhancer.ProfilingModule.SL.Integrations;
using ResumeEnhancer.ResumeModule.SL.Integrations;

namespace ResumeEnhancer.Tests.Unit.Modules.BillingModule.Application;

public sealed class BillingHandlerAndValidatorTests
{
    [Fact]
    public async Task BillingAccountHandlers_CoverCreateUpdateDeleteGetAndListFlows()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = Substitute.For<IBillingRepository>();
        var userLookup = Substitute.For<IUserLookupService>();
        var list = new[] { new BillingAccount { Id = 4, UserId = 21, AccountNumber = "ACC-02", Status = "Active" } };
        userLookup.UserExistsAsync(21, cancellationToken).Returns(true);
        repository.GetBillingAccountAsync(4, true, cancellationToken)
            .Returns(new BillingAccount { Id = 4, UserId = 21, AccountNumber = "ACC-01", Status = "Draft" });
        repository.GetBillingAccountAsync(9, true, cancellationToken).Returns((BillingAccount?)null);
        repository.GetBillingAccountAsync(4, false, cancellationToken)
            .Returns(new BillingAccount { Id = 4, UserId = 21, AccountNumber = "ACC-01", Status = "Active" });
        repository.ListBillingAccountsAsync(cancellationToken).Returns(list);

        var created = await new CreateBillingAccountCommandHandler(repository, userLookup).Handle(
            new(new CreateBillingAccountRequest
            {
                UserId = 21,
                AccountNumber = " ACC-77 ",
                Status = " Active ",
                ExternalReference = " ref "
            }, 7),
            cancellationToken);
        var missingUser = await new CreateBillingAccountCommandHandler(repository, userLookup).Handle(
            new(new CreateBillingAccountRequest { UserId = 999, AccountNumber = "ACC-X", Status = "Active" }, 7),
            cancellationToken);
        var updated = await new UpdateBillingAccountCommandHandler(repository, userLookup).Handle(
            new(4, new UpdateBillingAccountRequest
            {
                UserId = 21,
                AccountNumber = " ACC-99 ",
                Status = " Suspended ",
                ExternalReference = " ext "
            }, 8),
            cancellationToken);
        var updateMissing = await new UpdateBillingAccountCommandHandler(repository, userLookup).Handle(
            new(9, new UpdateBillingAccountRequest { UserId = 21, AccountNumber = "ACC-99", Status = "Active" }, 8),
            cancellationToken);
        var deleted = await new DeleteBillingAccountCommandHandler(repository).Handle(new(4, 9), cancellationToken);
        var deletedMissing = await new DeleteBillingAccountCommandHandler(repository).Handle(new(9, 9), cancellationToken);
        var detail = await new GetBillingAccountQueryHandler(repository).Handle(new(4), cancellationToken);
        var items = await new ListBillingAccountsQueryHandler(repository).Handle(new(), cancellationToken);

        created.AccountNumber.ShouldBe("ACC-77");
        created.ExternalReference.ShouldBe("ref");
        missingUser.Id.ShouldBe(0);
        updated.ShouldNotBeNull();
        updated.AccountNumber.ShouldBe("ACC-99");
        updateMissing.ShouldBeNull();
        deleted.ShouldBeTrue();
        deletedMissing.ShouldBeFalse();
        detail!.Id.ShouldBe(4);
        items.ShouldHaveSingleItem();
        await repository.Received(1).AddBillingAccountAsync(
            Arg.Is<BillingAccount>(item => item.AccountNumber == "ACC-77" && item.ExternalReference == "ref"),
            7,
            cancellationToken);
        await repository.Received(1).SaveAsync(8, cancellationToken);
        await repository.Received(1).DeleteBillingAccountAsync(Arg.Any<BillingAccount>(), 9, cancellationToken);
    }

    [Fact]
    public async Task BillingPlanHandlers_CoverCreateUpdateDeleteGetAndListFlows()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = Substitute.For<IBillingRepository>();
        repository.GetBillingPlanAsync(5, true, cancellationToken)
            .Returns(new BillingPlan { Id = 5, Code = "BASIC", Description = "Desc", DisplayName = "Basic", Currency = "USD", BillingInterval = "Monthly", Price = 5m });
        repository.GetBillingPlanAsync(99, true, cancellationToken).Returns((BillingPlan?)null);
        repository.GetBillingPlanAsync(5, false, cancellationToken)
            .Returns(new BillingPlan { Id = 5, Code = "PRO", Description = "Desc", DisplayName = "Pro", Currency = "USD", BillingInterval = "Yearly", Price = 50m });
        repository.ListBillingPlansAsync(cancellationToken).Returns(
            new[] { new BillingPlan { Id = 5, Code = "PRO", DisplayName = "Pro", Currency = "USD", Price = 50m } });

        var created = await new CreateBillingPlanCommandHandler(repository).Handle(
            new(new CreateBillingPlanRequest
            {
                Code = " PRO ",
                Description = " Description ",
                DisplayName = " Pro ",
                Price = 29m,
                Currency = " USD ",
                BillingInterval = " Monthly ",
                IsDeactivated = true
            }, 3),
            cancellationToken);
        var updated = await new UpdateBillingPlanCommandHandler(repository).Handle(
            new(5, new UpdateBillingPlanRequest
            {
                Code = " ENT ",
                Description = " Updated ",
                DisplayName = " Enterprise ",
                Price = 99m,
                Currency = " INR ",
                BillingInterval = " Yearly ",
                IsDeactivated = true,
                ObsoleteFlag = true
            }, 4),
            cancellationToken);
        var updateMissing = await new UpdateBillingPlanCommandHandler(repository).Handle(
            new(99, new UpdateBillingPlanRequest { Code = "X", Description = "Y", DisplayName = "Z" }, 4),
            cancellationToken);
        var deleted = await new DeleteBillingPlanCommandHandler(repository).Handle(new(5, 4), cancellationToken);
        var deletedMissing = await new DeleteBillingPlanCommandHandler(repository).Handle(new(99, 4), cancellationToken);
        var detail = await new GetBillingPlanQueryHandler(repository).Handle(new(5), cancellationToken);
        var items = await new ListBillingPlansQueryHandler(repository).Handle(new(), cancellationToken);

        created.Code.ShouldBe("PRO");
        updated!.Code.ShouldBe("ENT");
        updated.ObsoleteFlag.ShouldBeTrue();
        updateMissing.ShouldBeNull();
        deleted.ShouldBeTrue();
        deletedMissing.ShouldBeFalse();
        detail!.DisplayName.ShouldBe("Pro");
        items.ShouldHaveSingleItem();
    }

    [Fact]
    public async Task BillingSubscriptionHandlers_CoverCreateUpdateDeleteGetAndListFlows()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = Substitute.For<IBillingRepository>();
        var resumeLookup = Substitute.For<IResumeLookupService>();
        resumeLookup.ResumeExistsAsync(12, cancellationToken).Returns(true);
        repository.GetBillingSubscriptionAsync(6, true, cancellationToken)
            .Returns(new BillingSubscription { Id = 6, BillingAccountId = 1, BillingPlanId = 2, ResumeId = 12, Status = "Active", StartDateUtc = DateTime.UtcNow });
        repository.GetBillingSubscriptionAsync(98, true, cancellationToken).Returns((BillingSubscription?)null);
        repository.GetBillingSubscriptionAsync(6, false, cancellationToken)
            .Returns(new BillingSubscription { Id = 6, BillingAccountId = 1, BillingPlanId = 2, ResumeId = 12, Status = "Paused", StartDateUtc = DateTime.UtcNow });
        repository.ListBillingSubscriptionsAsync(cancellationToken).Returns(
            new[] { new BillingSubscription { Id = 6, BillingAccountId = 1, BillingPlanId = 2, ResumeId = 12, Status = "Paused", StartDateUtc = DateTime.UtcNow } });

        var created = await new CreateBillingSubscriptionCommandHandler(repository, resumeLookup).Handle(
            new(new CreateBillingSubscriptionRequest
            {
                BillingAccountId = 1,
                BillingPlanId = 2,
                ResumeId = 12,
                Status = " Active ",
                StartDateUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }, 7),
            cancellationToken);
        var invalidResume = await new CreateBillingSubscriptionCommandHandler(repository, resumeLookup).Handle(
            new(new CreateBillingSubscriptionRequest { BillingAccountId = 1, BillingPlanId = 2, ResumeId = 999, Status = "Active" }, 7),
            cancellationToken);
        var updated = await new UpdateBillingSubscriptionCommandHandler(repository, resumeLookup).Handle(
            new(6, new UpdateBillingSubscriptionRequest
            {
                BillingAccountId = 3,
                BillingPlanId = 4,
                ResumeId = 12,
                Status = " Ended ",
                StartDateUtc = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc),
                EndDateUtc = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc)
            }, 8),
            cancellationToken);
        var updateMissing = await new UpdateBillingSubscriptionCommandHandler(repository, resumeLookup).Handle(
            new(98, new UpdateBillingSubscriptionRequest { BillingAccountId = 1, BillingPlanId = 1, Status = "Active" }, 8),
            cancellationToken);
        var deleted = await new DeleteBillingSubscriptionCommandHandler(repository).Handle(new(6, 5), cancellationToken);
        var deletedMissing = await new DeleteBillingSubscriptionCommandHandler(repository).Handle(new(98, 5), cancellationToken);
        var detail = await new GetBillingSubscriptionQueryHandler(repository).Handle(new(6), cancellationToken);
        var items = await new ListBillingSubscriptionsQueryHandler(repository).Handle(new(), cancellationToken);

        created!.Status.ShouldBe("Active");
        invalidResume.ShouldBeNull();
        updated!.Status.ShouldBe("Ended");
        updateMissing.ShouldBeNull();
        deleted.ShouldBeTrue();
        deletedMissing.ShouldBeFalse();
        detail!.Id.ShouldBe(6);
        items.ShouldHaveSingleItem();
    }

    [Fact]
    public void BillingValidators_RejectInvalidRequests()
    {
        new CreateBillingAccountRequestValidator()
            .Validate(new CreateBillingAccountRequest { UserId = 0, AccountNumber = "", Status = "", ExternalReference = new string('x', 101) })
            .Errors.Count.ShouldBeGreaterThan(0);
        new UpdateBillingAccountRequestValidator()
            .Validate(new UpdateBillingAccountRequest { UserId = 0, AccountNumber = "", Status = "", ExternalReference = new string('x', 101) })
            .Errors.Count.ShouldBeGreaterThan(0);
        new CreateBillingPlanRequestValidator()
            .Validate(new CreateBillingPlanRequest { Code = "", Description = "", DisplayName = "", Price = -1, Currency = "", BillingInterval = "" })
            .Errors.Count.ShouldBeGreaterThan(0);
        new UpdateBillingPlanRequestValidator()
            .Validate(new UpdateBillingPlanRequest { Code = "", Description = "", DisplayName = "", Price = -1, Currency = "", BillingInterval = "" })
            .Errors.Count.ShouldBeGreaterThan(0);
        new CreateBillingSubscriptionRequestValidator()
            .Validate(new CreateBillingSubscriptionRequest { BillingAccountId = 0, BillingPlanId = 0, ResumeId = 0, Status = "" })
            .Errors.Count.ShouldBeGreaterThan(0);
        new UpdateBillingSubscriptionRequestValidator()
            .Validate(new UpdateBillingSubscriptionRequest { BillingAccountId = 0, BillingPlanId = 0, ResumeId = 0, Status = "" })
            .Errors.Count.ShouldBeGreaterThan(0);
    }
}
