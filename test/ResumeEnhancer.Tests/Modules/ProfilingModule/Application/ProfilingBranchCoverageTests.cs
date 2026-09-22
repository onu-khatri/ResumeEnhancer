using Microsoft.EntityFrameworkCore;
using NSubstitute;
using ResumeEnhancer.Infrastructure.Caching;
using ResumeEnhancer.ProfilingModule.AM.Requests;
using ResumeEnhancer.ProfilingModule.DM.Entities;
using ResumeEnhancer.ProfilingModule.SL.Abstractions.Persistence;
using ResumeEnhancer.ProfilingModule.SL.Contracts;
using ResumeEnhancer.ProfilingModule.SL.Handlers;
using ResumeEnhancer.ProfilingModule.SL.Integrations;
using ResumeEnhancer.ProfilingModule.PL.Repositories;
using ResumeEnhancer.Tests.Unit.TestInfrastructure;
using Shouldly;

namespace ResumeEnhancer.Tests.Unit.Modules.ProfilingModule.Application;

public sealed class ProfilingBranchCoverageTests
{
    [Fact]
    public async Task Update_user_handler_removes_empty_addresses_and_adds_missing_addresses()
    {
        var billing = new UserAddressTypeSetup { Id = 1, Code = "Billing" };
        var communication = new UserAddressTypeSetup { Id = 2, Code = "Communication" };
        var user = new User
        {
            UserAddresses =
            [
                new UserAddress { AddressTypeId = billing.Id, AddressType = billing, AddressLine1 = "Existing" }
            ]
        };

        var repository = Substitute.For<IProfilingRepository>();
        var setupRepository = Substitute.For<IProfilingSetupDataRepository>();
        repository.GetUserAsync(7, true, Arg.Any<CancellationToken>()).Returns(user);
        setupRepository.ListUserAddressTypesAsync(Arg.Any<CancellationToken>())
            .Returns([billing, communication]);

        await new UpdateUserCommandHandler(repository, setupRepository).Handle(
            new(7, new UpdateUserRequest
            {
                FirstName = "Alex",
                LastName = "Taylor",
                Email = "alex@example.com",
                BillingAddressLine1 = " ",
                CommunicationCity = " Pune "
            },
            1),
            TestContext.Current.CancellationToken);

        user.UserAddresses.ShouldHaveSingleItem().ShouldSatisfyAllConditions(
            address => address.AddressTypeId.ShouldBe(communication.Id),
            address => address.City.ShouldBe("Pune"),
            address => address.AddressLine1.ShouldBeNull());
    }

    [Fact]
    public async Task Handlers_return_null_for_missing_details_and_forward_cancellation()
    {
        var repository = Substitute.For<IProfilingRepository>();
        var cancellationToken = TestContext.Current.CancellationToken;
        repository.GetRoleAsync(10, false, cancellationToken).Returns((Role?)null);
        repository.GetAccessProfileAsync(10, false, cancellationToken).Returns((AccessProfile?)null);

        (await new GetRoleQueryHandler(repository).Handle(new(10), cancellationToken)).ShouldBeNull();
        (await new GetAccessProfileQueryHandler(repository).Handle(new(10), cancellationToken)).ShouldBeNull();

        await repository.Received(1).GetRoleAsync(10, false, cancellationToken);
        await repository.Received(1).GetAccessProfileAsync(10, false, cancellationToken);
    }

    [Fact]
    public async Task Repository_returns_empty_starter_access_when_guest_is_absent()
    {
        using var scope = new SqliteAppDbContextScope();
        var repository = new ProfilingRepository(scope.UnitOfWork, Substitute.For<ICacheProvider>());

        var shape = await repository.GetStarterAccessShapeAsync(TestContext.Current.CancellationToken);
        var profile = await repository.GetStarterAccessProfileAsync(TestContext.Current.CancellationToken);

        shape.Capabilities.ShouldBeEmpty();
        profile.ShouldBeNull();
    }

    [Fact]
    public async Task Repository_rejects_assignment_when_admin_source_is_missing()
    {
        using var scope = new SqliteAppDbContextScope();
        var adminSource = await scope.DbContext.Set<AccessProfileSource>().SingleAsync(source => source.Code == "admin");
        adminSource.ObsoleteFlag = true;
        await scope.DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new ProfilingRepository(scope.UnitOfWork, Substitute.For<ICacheProvider>());

        var exception = await Should.ThrowAsync<InvalidOperationException>(() => repository.SyncUserAccessProfilesAsync(
            new User { Id = ResumeTestData.UserId },
            [ResumeTestData.AccessProfileId],
            TestContext.Current.CancellationToken));

        exception.Message.ShouldBe("The administrator access-profile source is not configured.");
    }

    [Fact]
    public async Task Repository_sync_filters_invalid_and_unknown_access_profiles()
    {
        using var scope = new SqliteAppDbContextScope();
        var repository = new ProfilingRepository(scope.UnitOfWork, Substitute.For<ICacheProvider>());

        await repository.SyncUserAccessProfilesAsync(
            new User { Id = ResumeTestData.UserId },
            [0, ResumeTestData.AccessProfileId, 999],
            TestContext.Current.CancellationToken);
        await scope.UnitOfWork.SaveAsync(TestContext.Current.CancellationToken);

        var assignments = await scope.DbContext.Set<UserAccessProfile>()
            .Where(item => item.UserId == ResumeTestData.UserId && item.Enabled)
            .Select(item => item.AccessProfileId)
            .ToListAsync(TestContext.Current.CancellationToken);
        assignments.ShouldBe([ResumeTestData.AccessProfileId]);
    }

    [Fact]
    public async Task Repository_sync_filters_invalid_and_unknown_roles()
    {
        using var scope = new SqliteAppDbContextScope();
        var repository = new ProfilingRepository(scope.UnitOfWork, Substitute.For<ICacheProvider>());
        var accessProfile = await repository.GetAccessProfileAsync(ResumeTestData.AccessProfileId, true, TestContext.Current.CancellationToken);

        await repository.SyncAccessProfileRolesAsync(
            accessProfile!,
            [0, ResumeTestData.RoleId, 999],
            TestContext.Current.CancellationToken);
        await scope.UnitOfWork.SaveAsync(TestContext.Current.CancellationToken);

        var roleIds = await scope.DbContext.Set<AccessProfileRole>()
            .Where(item => item.AccessProfileId == ResumeTestData.AccessProfileId)
            .Select(item => item.RoleId)
            .ToListAsync(TestContext.Current.CancellationToken);
        roleIds.ShouldBe([ResumeTestData.RoleId]);
    }

    [Fact]
    public async Task Repository_revision_and_baseline_paths_reject_missing_configuration_or_invalid_inputs()
    {
        using (var scope = new SqliteAppDbContextScope())
        {
            var repository = new ProfilingRepository(scope.UnitOfWork, Substitute.For<ICacheProvider>());
            await repository.ReviseAccessProfilesAsync([], TestContext.Current.CancellationToken);

            var planSource = await scope.DbContext.Set<AccessProfileSource>().SingleAsync(source => source.Code == "plan");
            planSource.ObsoleteFlag = true;
            await scope.DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

            var exception = await Should.ThrowAsync<InvalidOperationException>(() => repository.ReviseAccessProfilesAsync(
                [new AccessProfileRevisionInput(ResumeTestData.UserId, 10, "FREE", ResumeTestData.AccessProfileId)],
                TestContext.Current.CancellationToken));
            exception.Message.ShouldBe("The plan access-profile source is not configured.");
        }

        using (var scope = new SqliteAppDbContextScope())
        {
            var repository = new ProfilingRepository(scope.UnitOfWork, Substitute.For<ICacheProvider>());
            var exception = await Should.ThrowAsync<InvalidOperationException>(() => repository.ReviseAccessProfilesAsync(
                [new AccessProfileRevisionInput(ResumeTestData.UserId, 10, "FREE", 999)],
                TestContext.Current.CancellationToken));
            exception.Message.ShouldBe("Access profile '999' is not available.");

            await Should.ThrowAsync<InvalidOperationException>(() => repository.AddRegistrationBaselineAsync(
                new RegistrationBaselineInput(ResumeTestData.UserId, 10, ResumeTestData.AccessProfileId, " "),
                TestContext.Current.CancellationToken));
        }

        using (var scope = new SqliteAppDbContextScope())
        {
            var repository = new ProfilingRepository(scope.UnitOfWork, Substitute.For<ICacheProvider>());
            var planSource = await scope.DbContext.Set<AccessProfileSource>().SingleAsync(source => source.Code == "plan");
            planSource.ObsoleteFlag = true;
            await scope.DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

            var exception = await Should.ThrowAsync<InvalidOperationException>(() => repository.AddRegistrationBaselineAsync(
                new RegistrationBaselineInput(ResumeTestData.UserId, 10, ResumeTestData.AccessProfileId, "FREE"),
                TestContext.Current.CancellationToken));
            exception.Message.ShouldBe("The plan access-profile source is not configured.");
        }
    }
}
