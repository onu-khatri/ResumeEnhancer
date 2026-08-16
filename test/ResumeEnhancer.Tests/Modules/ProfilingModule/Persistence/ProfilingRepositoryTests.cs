using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shouldly;
using ResumeEnhancer.Infrastructure.Caching;
using ResumeEnhancer.ProfilingModule.DM.Entities;
using ResumeEnhancer.ProfilingModule.PL.Repositories;
using ResumeEnhancer.Tests.Unit.TestInfrastructure;

namespace ResumeEnhancer.Tests.Unit.Modules.ProfilingModule.Persistence;

public sealed class ProfilingRepositoryTests
{
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
