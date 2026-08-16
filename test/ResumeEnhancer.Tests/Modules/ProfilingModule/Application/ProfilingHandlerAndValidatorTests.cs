using NSubstitute;
using Shouldly;
using ResumeEnhancer.ProfilingModule.AM.Requests;
using ResumeEnhancer.ProfilingModule.DM.Entities;
using ResumeEnhancer.ProfilingModule.DM.Enums;
using ResumeEnhancer.ProfilingModule.SL.Abstractions.Persistence;
using ResumeEnhancer.ProfilingModule.SL.Handlers;
using ResumeEnhancer.ProfilingModule.Web.Validation;

namespace ResumeEnhancer.Tests.Unit.Modules.ProfilingModule.Application;

public sealed class ProfilingHandlerAndValidatorTests
{
    [Fact]
    public async Task UserHandlers_CoverCrudFlowsAndSetupLookups()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = Substitute.For<IProfilingRepository>();
        var setupRepository = Substitute.For<IProfilingSetupDataRepository>();
        var billingType = new UserAddressTypeSetup { Id = 1, Code = nameof(UserAddressType.Billing), DisplayName = "Billing" };
        var communicationType = new UserAddressTypeSetup { Id = 2, Code = nameof(UserAddressType.Communication), DisplayName = "Communication" };
        setupRepository.ListUserAddressTypesAsync(cancellationToken).Returns(new[] { billingType, communicationType });
        repository
            .When(call => call.SyncUserAccessProfilesAsync(Arg.Any<User>(), Arg.Any<IReadOnlyCollection<int>>(), cancellationToken))
            .Do(call =>
            {
                var user = call.Arg<User>();
                var accessProfileIds = call.Arg<IReadOnlyCollection<int>>();
                user.UserAccessProfiles = accessProfileIds
                    .Select(id => new UserAccessProfile { UserId = user.Id, AccessProfileId = id })
                    .ToList();
            });
        repository.GetUserAsync(7, true, cancellationToken)
            .Returns(new User
            {
                Id = 7,
                FirstName = "Alex",
                LastName = "Taylor",
                Email = "alex@example.com",
                UserAddresses = [new UserAddress { AddressTypeId = 1, AddressType = billingType, AddressLine1 = "Old" }]
            });
        repository.GetUserAsync(9, true, cancellationToken).Returns((User?)null);
        repository.GetUserAsync(7, false, cancellationToken)
            .Returns(new User { Id = 7, FirstName = "Alex", LastName = "Taylor", Email = "alex@example.com" });
        repository.ListUsersAsync(cancellationToken).Returns(
            new[] { new User { Id = 7, FirstName = "Alex", LastName = "Taylor", Email = "alex@example.com" } });

        var created = await new CreateUserCommandHandler(repository, setupRepository).Handle(
            new(new CreateUserRequest
            {
                FirstName = " Alex ",
                LastName = " Taylor ",
                Email = " alex@example.com ",
                BillingAddressLine1 = " 1 Main ",
                BillingCity = " Pune ",
                BillingCountry = " India ",
                CommunicationAddressLine1 = " 2 Main ",
                CommunicationCity = " Mumbai ",
                CommunicationCountry = " India ",
                AccessProfileIds = [4, 5]
            }, 3),
            cancellationToken);
        var updated = await new UpdateUserCommandHandler(repository, setupRepository).Handle(
            new(7, new UpdateUserRequest
            {
                FirstName = " Sam ",
                LastName = " Reed ",
                Email = " sam@example.com ",
                CommunicationAddressLine1 = " 4 Pine ",
                CommunicationCity = " Delhi ",
                CommunicationCountry = " India ",
                AccessProfileIds = [6]
            }, 4),
            cancellationToken);
        var updateMissing = await new UpdateUserCommandHandler(repository, setupRepository).Handle(
            new(9, new UpdateUserRequest { FirstName = "A", LastName = "B", Email = "c@example.com" }, 4),
            cancellationToken);
        var deleted = await new DeleteUserCommandHandler(repository).Handle(new(7, 4), cancellationToken);
        var deletedMissing = await new DeleteUserCommandHandler(repository).Handle(new(9, 4), cancellationToken);
        var detail = await new GetUserQueryHandler(repository).Handle(new(7), cancellationToken);
        var items = await new ListUsersQueryHandler(repository).Handle(new(), cancellationToken);

        created.FirstName.ShouldBe("Alex");
        created.AccessProfileIds.ShouldBe([4, 5]);
        updated!.FirstName.ShouldBe("Sam");
        updateMissing.ShouldBeNull();
        deleted.ShouldBeTrue();
        deletedMissing.ShouldBeFalse();
        detail!.Id.ShouldBe(7);
        items.ShouldHaveSingleItem();
        await repository.Received(2).SyncUserAccessProfilesAsync(Arg.Any<User>(), Arg.Any<IReadOnlyCollection<int>>(), cancellationToken);
    }

    [Fact]
    public async Task RoleAndAccessProfileHandlers_CoverCrudFlows()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = Substitute.For<IProfilingRepository>();
        repository.GetRoleAsync(1, true, cancellationToken)
            .Returns(new Role { Id = 1, Code = "ADMIN", Description = "Desc", DisplayName = "Admin" });
        repository.GetRoleAsync(2, true, cancellationToken).Returns((Role?)null);
        repository.GetRoleAsync(1, false, cancellationToken)
            .Returns(new Role { Id = 1, Code = "ADMIN", Description = "Desc", DisplayName = "Admin" });
        repository.ListRolesAsync(cancellationToken).Returns(new[] { new Role { Id = 1, Code = "ADMIN", DisplayName = "Admin" } });
        repository.GetAccessProfileAsync(3, true, cancellationToken)
            .Returns(new AccessProfile { Id = 3, Code = "HR", Description = "Desc", DisplayName = "HR" });
        repository.GetAccessProfileAsync(4, true, cancellationToken).Returns((AccessProfile?)null);
        repository.GetAccessProfileAsync(3, false, cancellationToken)
            .Returns(new AccessProfile { Id = 3, Code = "HR", Description = "Desc", DisplayName = "HR" });
        repository.ListAccessProfilesAsync(cancellationToken).Returns(new[] { new AccessProfile { Id = 3, Code = "HR", DisplayName = "HR" } });

        var createdRole = await new CreateRoleCommandHandler(repository).Handle(
            new(new CreateRoleRequest { Code = " ADMIN ", Description = " Desc ", DisplayName = " Admin " }, 1),
            cancellationToken);
        var updatedRole = await new UpdateRoleCommandHandler(repository).Handle(
            new(1, new UpdateRoleRequest { Code = " USER ", Description = " Updated ", DisplayName = " User ", ObsoleteFlag = true }, 2),
            cancellationToken);
        var createdProfile = await new CreateAccessProfileCommandHandler(repository).Handle(
            new(new CreateAccessProfileRequest { Code = " HR ", Description = " Desc ", DisplayName = " HR ", RoleIds = [1, 2] }, 1),
            cancellationToken);
        var updatedProfile = await new UpdateAccessProfileCommandHandler(repository).Handle(
            new(3, new UpdateAccessProfileRequest { Code = " IT ", Description = " Updated ", DisplayName = " IT ", ObsoleteFlag = true, RoleIds = [2] }, 2),
            cancellationToken);

        createdRole.Code.ShouldBe("ADMIN");
        updatedRole!.Code.ShouldBe("USER");
        createdProfile.Code.ShouldBe("HR");
        updatedProfile!.Code.ShouldBe("IT");
        (await new DeleteRoleCommandHandler(repository).Handle(new(1, 2), cancellationToken)).ShouldBeTrue();
        (await new DeleteRoleCommandHandler(repository).Handle(new(2, 2), cancellationToken)).ShouldBeFalse();
        (await new DeleteAccessProfileCommandHandler(repository).Handle(new(3, 2), cancellationToken)).ShouldBeTrue();
        (await new DeleteAccessProfileCommandHandler(repository).Handle(new(4, 2), cancellationToken)).ShouldBeFalse();
        (await new GetRoleQueryHandler(repository).Handle(new(1), cancellationToken))!.Id.ShouldBe(1);
        (await new GetAccessProfileQueryHandler(repository).Handle(new(3), cancellationToken))!.Id.ShouldBe(3);
        (await new ListRolesQueryHandler(repository).Handle(new(), cancellationToken)).ShouldHaveSingleItem();
        (await new ListAccessProfilesQueryHandler(repository).Handle(new(), cancellationToken)).ShouldHaveSingleItem();
    }

    [Fact]
    public void ProfilingValidators_RejectInvalidRequests()
    {
        new CreateUserRequestValidator()
            .Validate(new CreateUserRequest { FirstName = "", LastName = "", Email = "bad", AccessProfileIds = [0] })
            .Errors.Count.ShouldBeGreaterThan(0);
        new UpdateUserRequestValidator()
            .Validate(new UpdateUserRequest { FirstName = "", LastName = "", Email = "bad", AccessProfileIds = [0] })
            .Errors.Count.ShouldBeGreaterThan(0);
        new CreateRoleRequestValidator()
            .Validate(new CreateRoleRequest { Code = "", Description = "", DisplayName = "" })
            .Errors.Count.ShouldBeGreaterThan(0);
        new UpdateRoleRequestValidator()
            .Validate(new UpdateRoleRequest { Code = "", Description = "", DisplayName = "" })
            .Errors.Count.ShouldBeGreaterThan(0);
        new CreateAccessProfileRequestValidator()
            .Validate(new CreateAccessProfileRequest { Code = "", Description = "", DisplayName = "", RoleIds = [0] })
            .Errors.Count.ShouldBeGreaterThan(0);
        new UpdateAccessProfileRequestValidator()
            .Validate(new UpdateAccessProfileRequest { Code = "", Description = "", DisplayName = "", RoleIds = [0] })
            .Errors.Count.ShouldBeGreaterThan(0);
    }
}
