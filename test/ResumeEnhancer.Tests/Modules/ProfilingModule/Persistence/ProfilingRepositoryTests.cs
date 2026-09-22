using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shouldly;
using ResumeEnhancer.Infrastructure.Caching;
using ResumeEnhancer.ProfilingModule.DM.Entities;
using ResumeEnhancer.ProfilingModule.PL.Repositories;
using ResumeEnhancer.ProfilingModule.SL.Integrations;
using ResumeEnhancer.Tests.Unit.TestInfrastructure;

namespace ResumeEnhancer.Tests.Unit.Modules.ProfilingModule.Persistence;

public sealed class ProfilingRepositoryTests
{
    [Fact]
    public async Task GetUserAuthorizationAsync_ProjectsOnlyEligibleAssignmentsAndActiveRoles()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        scope.DbContext.AddRange(
            new AccessProfile
            {
                Id = 10,
                Code = "ACTIVE",
                Description = "Active",
                DisplayName = "Active",
                Order = 10,
                Guid = Guid.NewGuid()
            },
            new AccessProfile
            {
                Id = 11,
                Code = "DISABLED",
                Description = "Disabled",
                DisplayName = "Disabled",
                Order = 11,
                Guid = Guid.NewGuid()
            },
            new AccessProfile
            {
                Id = 12,
                Code = "EXPIRED",
                Description = "Expired",
                DisplayName = "Expired",
                Order = 12,
                Guid = Guid.NewGuid()
            },
            new AccessProfile
            {
                Id = 13,
                Code = "OBSOLETE",
                Description = "Obsolete",
                DisplayName = "Obsolete",
                Order = 13,
                Guid = Guid.NewGuid(),
                ObsoleteFlag = true
            },
            new Role
            {
                Id = 10,
                Code = "MEMBER",
                Capability = "resume.read",
                Description = "Member",
                DisplayName = "Member",
                Order = 10,
                Guid = Guid.NewGuid()
            },
            new Role
            {
                Id = 11,
                Code = "OBSOLETE_ROLE",
                Capability = "resume.delete",
                Description = "Obsolete role",
                DisplayName = "Obsolete role",
                Order = 11,
                Guid = Guid.NewGuid(),
                ObsoleteFlag = true
            },
            new AccessProfileRole
            {
                Id = 10,
                AccessProfileId = 10,
                RoleId = 10,
                Code = "ACTIVE_MEMBER",
                Description = "Active member",
                Guid = Guid.NewGuid()
            },
            new AccessProfileRole
            {
                Id = 11,
                AccessProfileId = 10,
                RoleId = 11,
                Code = "OBSOLETE_LINK",
                Description = "Obsolete link",
                Guid = Guid.NewGuid(),
                ObsoleteFlag = true
            },
            new UserAccessProfile
            {
                Id = 10,
                UserId = ResumeTestData.UserId,
                AccessProfileId = 10,
                AccessProfileSourceId = 1,
                AssignedOnUtc = DateTime.UtcNow.AddMinutes(-5),
                Enabled = true
            },
            new UserAccessProfile
            {
                Id = 11,
                UserId = ResumeTestData.UserId,
                AccessProfileId = 11,
                AccessProfileSourceId = 1,
                AssignedOnUtc = DateTime.UtcNow.AddMinutes(-5),
                Enabled = false
            },
            new UserAccessProfile
            {
                Id = 12,
                UserId = ResumeTestData.UserId,
                AccessProfileId = 12,
                AccessProfileSourceId = 1,
                AssignedOnUtc = DateTime.UtcNow.AddMinutes(-5),
                ValidTillUtc = DateTime.UtcNow.AddMinutes(-1),
                Enabled = true
            },
            new UserAccessProfile
            {
                Id = 13,
                UserId = ResumeTestData.UserId,
                AccessProfileId = 13,
                AccessProfileSourceId = 1,
                AssignedOnUtc = DateTime.UtcNow.AddMinutes(-5),
                Enabled = true
            });
        await scope.DbContext.SaveChangesAsync(cancellationToken);

        var repository = new ProfilingRepository(scope.UnitOfWork, CreateCacheProvider());
        var result = await repository.GetUserAuthorizationAsync(ResumeTestData.UserId, cancellationToken);

        result.ShouldNotBeNull();
        result!.UserId.ShouldBe(ResumeTestData.UserId);
        result.AccessProfileCodes.ShouldBe(["ACTIVE"]);
        result.RoleCodes.ShouldBe(["MEMBER"]);
        result.Capabilities.ShouldBe(["resume.read"]);
    }

    [Fact]
    public async Task GetGuestAuthorizationAsync_ProjectsConfiguredGuestAndFiltersObsoleteRows()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        scope.DbContext.AddRange(
            new AccessProfile
            {
                Id = 20,
                Code = "Guest",
                Description = "Guest",
                DisplayName = "Guest",
                Order = 20,
                Guid = Guid.NewGuid()
            },
            new Role
            {
                Id = 20,
                Code = "PUBLIC",
                Capability = "resume.read",
                Description = "Public",
                DisplayName = "Public",
                Order = 20,
                Guid = Guid.NewGuid()
            },
            new Role
            {
                Id = 21,
                Code = "OBSOLETE_PUBLIC",
                Capability = "resume.write",
                Description = "Obsolete public",
                DisplayName = "Obsolete public",
                Order = 21,
                Guid = Guid.NewGuid(),
                ObsoleteFlag = true
            },
            new AccessProfileRole
            {
                Id = 20,
                AccessProfileId = 20,
                RoleId = 20,
                Code = "GUEST_PUBLIC",
                Description = "Guest public",
                Guid = Guid.NewGuid()
            },
            new AccessProfileRole
            {
                Id = 21,
                AccessProfileId = 20,
                RoleId = 21,
                Code = "GUEST_OBSOLETE",
                Description = "Guest obsolete",
                Guid = Guid.NewGuid()
            });
        await scope.DbContext.SaveChangesAsync(cancellationToken);

        var repository = new ProfilingRepository(scope.UnitOfWork, CreateCacheProvider());
        var result = await repository.GetGuestAuthorizationAsync(cancellationToken);

        result.ShouldNotBeNull();
        result!.UserId.ShouldBe(0);
        result.AccessProfileCodes.ShouldBe(["Guest"]);
        result.RoleCodes.ShouldBe(["PUBLIC"]);
        result.Capabilities.ShouldBe(["resume.read"]);
    }

    [Fact]
    public async Task GetGuestAuthorizationAsync_ReturnsAbsentForMissingOrObsoleteGuest()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new ProfilingRepository(scope.UnitOfWork, CreateCacheProvider());

        (await repository.GetGuestAuthorizationAsync(cancellationToken)).ShouldBeNull();

        scope.DbContext.Add(new AccessProfile
        {
            Id = 30,
            Code = "Guest",
            Description = "Obsolete Guest",
            DisplayName = "Obsolete Guest",
            Order = 30,
            Guid = Guid.NewGuid(),
            ObsoleteFlag = true
        });
        await scope.DbContext.SaveChangesAsync(cancellationToken);

        (await repository.GetGuestAuthorizationAsync(cancellationToken)).ShouldBeNull();
    }

    [Fact]
    public async Task Authorization_projections_preserve_missing_user_empty_existing_user_and_cancellation()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new ProfilingRepository(scope.UnitOfWork, CreateCacheProvider());

        (await repository.GetUserAuthorizationAsync(999, cancellationToken)).ShouldBeNull();

        var empty = await repository.GetUserAuthorizationAsync(ResumeTestData.UserId, cancellationToken);
        empty.ShouldNotBeNull();
        empty!.UserId.ShouldBe(ResumeTestData.UserId);
        empty.AccessProfileCodes.ShouldBeEmpty();
        empty.RoleCodes.ShouldBeEmpty();
        empty.Capabilities.ShouldBeEmpty();

        var canceled = new CancellationToken(canceled: true);
        await Should.ThrowAsync<OperationCanceledException>(
            () => repository.GetUserAuthorizationAsync(ResumeTestData.UserId, canceled));
    }

    public static TheoryData<int, int, string, int> InvalidRevisionInputs => new()
    {
        { 0, 10, "FREE", 2 },
        { 1, 0, "FREE", 2 },
        { 1, 10, "", 2 },
        { 1, 10, "FREE", 0 },
    };

    [Theory]
    [MemberData(nameof(InvalidRevisionInputs))]
    public async Task ReviseAccessProfilesAsync_InvalidInput_Throws(
        int userId,
        int subscriptionId,
        string planCode,
        int accessProfileId)
    {
        using var scope = new SqliteAppDbContextScope();
        var repository = new ProfilingRepository(scope.UnitOfWork, CreateCacheProvider());

        await Should.ThrowAsync<InvalidOperationException>(() => repository.ReviseAccessProfilesAsync(
            [new AccessProfileRevisionInput(userId, subscriptionId, planCode, accessProfileId)],
            TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ReviseAccessProfilesAsync_Replay_DoesNotDuplicateActiveAssignment()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        scope.DbContext.Add(
            new AccessProfile { Id = 2, Code = "PRO", Description = "Pro", DisplayName = "Pro", Order = 2, Guid = Guid.NewGuid() });
        await scope.DbContext.SaveChangesAsync(cancellationToken);
        var repository = new ProfilingRepository(scope.UnitOfWork, CreateCacheProvider());
        var input = new AccessProfileRevisionInput(ResumeTestData.UserId, 501, "PRO", 2);

        await repository.ReviseAccessProfilesAsync([input], cancellationToken);
        await repository.ReviseAccessProfilesAsync([input], cancellationToken);

        var assignments = await scope.DbContext.Set<UserAccessProfile>()
            .Where(item => item.UserId == ResumeTestData.UserId && item.BillingSubscriptionId == 501 && item.Enabled)
            .ToListAsync(cancellationToken);
        assignments.ShouldHaveSingleItem().AccessProfileId.ShouldBe(2);
        (await scope.DbContext.Set<AccessProfileRevision>().CountAsync(cancellationToken)).ShouldBe(1);
    }

    [Fact]
    public async Task ReviseAccessProfilesAsync_DisablesPreviousAssignment_AndPreservesExpiry()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        scope.DbContext.Add(new AccessProfile
        {
            Id = 2,
            Code = "PRO",
            Description = "Pro",
            DisplayName = "Pro",
            Order = 2,
            Guid = Guid.NewGuid()
        });
        var expiredOn = DateTime.UtcNow.AddMinutes(-1);
        scope.DbContext.Add(new UserAccessProfile
        {
            UserId = ResumeTestData.UserId,
            AccessProfileId = ResumeTestData.AccessProfileId,
            AccessProfileSourceId = 1,
            BillingSubscriptionId = 501,
            AssignedOnUtc = expiredOn.AddDays(-1),
            ValidTillUtc = expiredOn,
            Enabled = true
        });
        await scope.DbContext.SaveChangesAsync(cancellationToken);

        var repository = new ProfilingRepository(scope.UnitOfWork, CreateCacheProvider());
        await repository.ReviseAccessProfilesAsync(
            [new AccessProfileRevisionInput(ResumeTestData.UserId, 501, "FREE", 2)],
            cancellationToken);

        var assignments = await scope.DbContext.Set<UserAccessProfile>()
            .Where(item => item.UserId == ResumeTestData.UserId && item.BillingSubscriptionId == 501)
            .OrderBy(item => item.AccessProfileId)
            .ToListAsync(cancellationToken);
        assignments.Count.ShouldBe(2);
        assignments[0].Enabled.ShouldBeFalse();
        assignments[0].ValidTillUtc.ShouldBe(expiredOn);
        assignments[1].Enabled.ShouldBeTrue();
    }

    [Fact]
    public async Task ProfilingRepository_CoversCrudAndSyncFlows()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        var cacheProvider = CreateCacheProvider();
        var repository = new ProfilingRepository(scope.UnitOfWork, cacheProvider);
        var accessProfile2 = ResumeTestData.AccessProfile(id: 2, code: "HR", order: 2);
        var role2 = ResumeTestData.Role(id: 2, code: "USER", order: 2);
        scope.DbContext.AddRange(accessProfile2, role2);
        await scope.DbContext.SaveChangesAsync(new TestAudit(1), cancellationToken);

        var user = await repository.AddUserAsync(
            new User
            {
                FirstName = "Alex",
                LastName = "Taylor",
                Email = "alex@example.com",
                UserAddresses =
                [
                    new UserAddress
                    {
                        AddressTypeId = ResumeTestData.BillingAddressTypeId,
                        AddressLine1 = "1 Main",
                        City = "Pune",
                        Country = "India"
                    }
                ]
            },
            7,
            cancellationToken);
        await repository.SyncUserAccessProfilesAsync(user, [ResumeTestData.AccessProfileId, accessProfile2.Id], cancellationToken);
        await repository.SaveAsync(7, cancellationToken);

        var role = await repository.AddRoleAsync(new Role { Code = "DEV", Description = "Developer", DisplayName = "Developer", Guid = Guid.NewGuid(), Order = 3 }, 7, cancellationToken);
        var accessProfile = await repository.AddAccessProfileAsync(new AccessProfile { Code = "OPS", Description = "Ops", DisplayName = "Ops", Guid = Guid.NewGuid(), Order = 3 }, 7, cancellationToken);
        scope.DbContext.AddRange(
            new AccessProfileRole { AccessProfileId = accessProfile.Id, RoleId = ResumeTestData.RoleId, Code = "APR-1", Description = "Access profile role 1", Guid = Guid.NewGuid() },
            new AccessProfileRole { AccessProfileId = accessProfile.Id, RoleId = role2.Id, Code = "APR-2", Description = "Access profile role 2", Guid = Guid.NewGuid() },
            new AccessProfileRole { AccessProfileId = accessProfile.Id, RoleId = role.Id, Code = "APR-3", Description = "Access profile role 3", Guid = Guid.NewGuid() });
        await scope.DbContext.SaveChangesAsync(new TestAudit(1), cancellationToken);
        scope.DbContext.ChangeTracker.Clear();
        var trackedAccessProfileForSync = await repository.GetAccessProfileAsync(accessProfile.Id, true, cancellationToken);
        await repository.SyncAccessProfileRolesAsync(trackedAccessProfileForSync!, [role2.Id], cancellationToken);
        await repository.SaveAsync(7, cancellationToken);

        scope.DbContext.ChangeTracker.Clear();

        (await repository.GetUserAsync(user.Id, false, cancellationToken))!.UserAccessProfiles.Count.ShouldBe(2);
        (await repository.GetRoleAsync(role.Id, false, cancellationToken))!.Code.ShouldBe("DEV");
        (await repository.GetAccessProfileAsync(accessProfile.Id, false, cancellationToken))!.AccessProfileRoles.Count.ShouldBe(1);
        (await repository.ListUsersAsync(cancellationToken)).ShouldContain(item => item.Id == user.Id);
        (await repository.ListRolesAsync(cancellationToken)).ShouldContain(item => item.Id == role.Id);
        (await repository.ListAccessProfilesAsync(cancellationToken)).ShouldContain(item => item.Id == accessProfile.Id);
        (await repository.UserExistsAsync(user.Id, cancellationToken)).ShouldBeTrue();

        var trackedUser = await repository.GetUserAsync(user.Id, true, cancellationToken);
        await repository.SyncUserAccessProfilesAsync(trackedUser!, [accessProfile2.Id], cancellationToken);
        await repository.SaveAsync(8, cancellationToken);
        (await repository.GetUserAsync(user.Id, false, cancellationToken))!.UserAccessProfiles.Select(item => item.AccessProfileId).ShouldBe([accessProfile2.Id]);

        (await repository.GetAccessProfileAsync(accessProfile.Id, false, cancellationToken))!.AccessProfileRoles.Select(item => item.RoleId).ShouldBe([role2.Id]);

        var trackedUserForDelete = await repository.GetUserAsync(user.Id, true, cancellationToken);
        await repository.SyncUserAccessProfilesAsync(trackedUserForDelete!, [], cancellationToken);
        await repository.SaveAsync(9, cancellationToken);
        await repository.DeleteUserAsync(trackedUserForDelete, 9, cancellationToken);
        var trackedAccessProfileForDelete = await repository.GetAccessProfileAsync(accessProfile.Id, true, cancellationToken);
        await repository.SyncAccessProfileRolesAsync(trackedAccessProfileForDelete!, [], cancellationToken);
        await repository.SaveAsync(9, cancellationToken);
        await repository.DeleteAccessProfileAsync(trackedAccessProfileForDelete, 9, cancellationToken);
        await repository.DeleteRoleAsync((await repository.GetRoleAsync(role.Id, true, cancellationToken))!, 9, cancellationToken);

        (await scope.DbContext.Set<User>().AnyAsync(item => item.Id == user.Id, cancellationToken)).ShouldBeFalse();
        (await scope.DbContext.Set<Role>().AnyAsync(item => item.Id == role.Id, cancellationToken)).ShouldBeFalse();
        (await scope.DbContext.Set<AccessProfile>().AnyAsync(item => item.Id == accessProfile.Id, cancellationToken)).ShouldBeFalse();
        await cacheProvider.Received().RemoveAsync("profiling:setup:roles", cancellationToken);
        await cacheProvider.Received().RemoveAsync("profiling:setup:access-profiles", cancellationToken);
        await cacheProvider.Received().RemoveAsync("profiling:setup:user-address-types", cancellationToken);
    }

    [Fact]
    public async Task ProfilingSetupDataRepository_ListsSetupRowsThroughCacheFactory()
    {
        using var scope = new SqliteAppDbContextScope();
        var cancellationToken = TestContext.Current.CancellationToken;
        var cacheProvider = Substitute.For<ICacheProvider>();
        cacheProvider
            .GetOrSetAsync<IReadOnlyList<Role>>(
                "profiling:setup:roles",
                Arg.Any<Func<CancellationToken, Task<IReadOnlyList<Role>>>>(),
                Arg.Any<CacheEntryOptions?>(),
                cancellationToken)
            .Returns(call => call.Arg<Func<CancellationToken, Task<IReadOnlyList<Role>>>>()(cancellationToken));
        cacheProvider
            .GetOrSetAsync<IReadOnlyList<AccessProfile>>(
                "profiling:setup:access-profiles",
                Arg.Any<Func<CancellationToken, Task<IReadOnlyList<AccessProfile>>>>(),
                Arg.Any<CacheEntryOptions?>(),
                cancellationToken)
            .Returns(call => call.Arg<Func<CancellationToken, Task<IReadOnlyList<AccessProfile>>>>()(cancellationToken));
        cacheProvider
            .GetOrSetAsync<IReadOnlyList<UserAddressTypeSetup>>(
                "profiling:setup:user-address-types",
                Arg.Any<Func<CancellationToken, Task<IReadOnlyList<UserAddressTypeSetup>>>>(),
                Arg.Any<CacheEntryOptions?>(),
                cancellationToken)
            .Returns(call => call.Arg<Func<CancellationToken, Task<IReadOnlyList<UserAddressTypeSetup>>>>()(cancellationToken));

        var repository = new ProfilingSetupDataRepository(scope.UnitOfWork, cacheProvider);

        (await repository.ListRolesAsync(cancellationToken)).ShouldNotBeEmpty();
        (await repository.ListAccessProfilesAsync(cancellationToken)).ShouldNotBeEmpty();
        (await repository.ListUserAddressTypesAsync(cancellationToken)).ShouldNotBeEmpty();
    }

    private static ICacheProvider CreateCacheProvider()
    {
        var cacheProvider = Substitute.For<ICacheProvider>();
        cacheProvider.RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        return cacheProvider;
    }
}
