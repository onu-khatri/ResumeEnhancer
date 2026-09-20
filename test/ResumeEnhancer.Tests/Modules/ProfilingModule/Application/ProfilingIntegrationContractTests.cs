using NSubstitute;
using Microsoft.Extensions.DependencyInjection;
using ResumeEnhancer.ProfilingModule.DM.Entities;
using ResumeEnhancer.ProfilingModule.SL;
using ResumeEnhancer.ProfilingModule.SL.Abstractions.Persistence;
using ResumeEnhancer.ProfilingModule.SL.Integrations;
using Shouldly;

namespace ResumeEnhancer.Tests.Unit.Modules.ProfilingModule.Application;

public sealed class ProfilingIntegrationContractTests
{
    [Fact]
    public async Task User_lookup_maps_only_profiling_account_state()
    {
        var repository = Substitute.For<IProfilingRepository>();
        repository.GetUserStateAsync(42, Arg.Any<CancellationToken>())
            .Returns(new User { Id = 42, IsDeactivated = true });

        var services = new ServiceCollection()
            .AddSingleton(repository)
            .AddProfilingModuleApplication()
            .BuildServiceProvider();

        var result = await services.GetRequiredService<IUserLookupService>().GetUserStateAsync(42);

        result.ShouldNotBeNull();
        result!.UserId.ShouldBe(42);
        result.IsDeactivated.ShouldBeTrue();
        result.IsDeleted.ShouldBeFalse();
        typeof(ProfilingUserStateSnapshot).GetProperties()
            .Select(property => property.Name)
            .ShouldBe(["UserId", "IsDeactivated", "IsDeleted"]);
    }

    [Fact]
    public async Task Authorization_service_preserves_empty_and_missing_snapshots()
    {
        var repository = Substitute.For<IProfilingRepository>();
        repository.GetUserAuthorizationAsync(7, Arg.Any<CancellationToken>())
            .Returns(ProfilingAuthorizationSnapshot.Empty(7));
        repository.GetUserAuthorizationAsync(8, Arg.Any<CancellationToken>())
            .Returns((ProfilingAuthorizationSnapshot?)null);

        var services = new ServiceCollection()
            .AddSingleton(repository)
            .AddProfilingModuleApplication()
            .BuildServiceProvider();
        var service = services.GetRequiredService<IProfilingAuthorizationService>();

        var empty = await service.GetUserAuthorizationAsync(7);
        var missing = await service.GetUserAuthorizationAsync(8);

        empty.ShouldNotBeNull();
        empty!.AccessProfileCodes.ShouldBeEmpty();
        empty.RoleCodes.ShouldBeEmpty();
        empty.Capabilities.ShouldBeEmpty();
        missing.ShouldBeNull();
    }

    [Fact]
    public async Task Authorization_service_preserves_missing_guest_as_absent()
    {
        var repository = Substitute.For<IProfilingRepository>();
        repository.GetGuestAuthorizationAsync(Arg.Any<CancellationToken>())
            .Returns((ProfilingAuthorizationSnapshot?)null);

        var services = new ServiceCollection()
            .AddSingleton(repository)
            .AddProfilingModuleApplication()
            .BuildServiceProvider();

        var result = await services.GetRequiredService<IProfilingAuthorizationService>()
            .GetGuestAuthorizationAsync();

        result.ShouldBeNull();
    }

    [Fact]
    public async Task Authorization_service_preserves_configured_guest_snapshot()
    {
        var repository = Substitute.For<IProfilingRepository>();
        var expected = new ProfilingAuthorizationSnapshot(
            0,
            new HashSet<string>(["Guest"]),
            new HashSet<string>(["PUBLIC"]),
            new HashSet<string>(["resume.read"]));
        repository.GetGuestAuthorizationAsync(Arg.Any<CancellationToken>()).Returns(expected);

        var services = new ServiceCollection()
            .AddSingleton(repository)
            .AddProfilingModuleApplication()
            .BuildServiceProvider();

        var result = await services.GetRequiredService<IProfilingAuthorizationService>()
            .GetGuestAuthorizationAsync();

        result.ShouldBeSameAs(expected);
    }

    [Fact]
    public async Task Authorization_service_does_not_swallow_repository_failures()
    {
        var repository = Substitute.For<IProfilingRepository>();
        var expected = new InvalidOperationException("profiling unavailable");
        repository.GetUserAuthorizationAsync(42, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<ProfilingAuthorizationSnapshot?>(expected));

        var services = new ServiceCollection()
            .AddSingleton(repository)
            .AddProfilingModuleApplication()
            .BuildServiceProvider();

        var actual = await Should.ThrowAsync<InvalidOperationException>(
            () => services.GetRequiredService<IProfilingAuthorizationService>().GetUserAuthorizationAsync(42));

        actual.ShouldBeSameAs(expected);
    }

    [Fact]
    public void Authorization_service_is_registered_at_the_profiling_application_boundary()
    {
        var descriptor = new Microsoft.Extensions.DependencyInjection.ServiceCollection()
            .AddProfilingModuleApplication()
            .Single(service => service.ServiceType == typeof(IProfilingAuthorizationService));

        descriptor.Lifetime.ShouldBe(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped);
        descriptor.ImplementationType!.Name.ShouldBe("ProfilingAuthorizationService");
    }
}
