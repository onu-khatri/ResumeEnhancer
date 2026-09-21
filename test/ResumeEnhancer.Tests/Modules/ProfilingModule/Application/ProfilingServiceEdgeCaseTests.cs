using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ResumeEnhancer.ProfilingModule.DM.Entities;
using ResumeEnhancer.ProfilingModule.SL;
using ResumeEnhancer.ProfilingModule.SL.Abstractions.Persistence;
using ResumeEnhancer.ProfilingModule.SL.Contracts;
using ResumeEnhancer.ProfilingModule.SL.Handlers;
using ResumeEnhancer.ProfilingModule.SL.Integrations;
using Shouldly;

namespace ResumeEnhancer.Tests.Unit.Modules.ProfilingModule.Application;

public sealed class ProfilingServiceEdgeCaseTests
{
    [Fact]
    public async Task CreateUserHandler_rejects_missing_address_type_setup_before_persisting()
    {
        var repository = Substitute.For<IProfilingRepository>();
        var setupRepository = Substitute.For<IProfilingSetupDataRepository>();
        setupRepository.ListUserAddressTypesAsync(Arg.Any<CancellationToken>())
            .Returns([new UserAddressTypeSetup { Id = 1, Code = "Billing" }]);

        var request = new CreateUserCommand(
            new ResumeEnhancer.ProfilingModule.AM.Requests.CreateUserRequest
            {
                FirstName = "Alex",
                LastName = "Taylor",
                Email = "alex@example.com",
            },
            7);

        var exception = await Should.ThrowAsync<InvalidOperationException>(
            () => new CreateUserCommandHandler(repository, setupRepository)
                .Handle(request, TestContext.Current.CancellationToken).AsTask());

        exception.Message.ShouldBe("Communication address type setup was not found.");
        await repository.DidNotReceiveWithAnyArgs().AddUserAsync(default!, default, default);
    }

    [Fact]
    public async Task ProfilingRegistrationService_forwards_registration_and_access_workflows()
    {
        var repository = Substitute.For<IProfilingRepository>();
        var user = new User { Id = 42 };
        var accessShape = new AccessShapeSnapshot(new HashSet<string>(["resume.read"]));
        var starterProfile = new StarterAccessProfileSnapshot(3);
        repository.AddUserForRegistrationAsync(Arg.Any<ProfileRegistrationInput>(), Arg.Any<CancellationToken>())
            .Returns(user);
        repository.GetAccessShapeAsync(3, Arg.Any<CancellationToken>()).Returns(accessShape);
        repository.GetStarterAccessShapeAsync(Arg.Any<CancellationToken>()).Returns(accessShape);
        repository.GetStarterAccessProfileAsync(Arg.Any<CancellationToken>()).Returns(starterProfile);

        using var provider = new ServiceCollection()
            .AddSingleton(repository)
            .AddProfilingModuleApplication()
            .BuildServiceProvider();
        var service = provider.GetRequiredService<IProfilingRegistrationService>();
        var cancellationToken = TestContext.Current.CancellationToken;
        var registrationInput = new ProfileRegistrationInput("Alex", "Taylor", "alex@example.com");
        var baselineInput = new RegistrationBaselineInput(42, 100, 3, "PRO");
        var revisionInput = new AccessProfileRevisionInput(42, 100, "PRO", 3);

        (await service.CreateRegistrationUserAsync(registrationInput, cancellationToken)).UserId.ShouldBe(42);
        await service.AddRegistrationBaselineAsync(baselineInput, cancellationToken);
        (await service.GetAccessShapeAsync(3, cancellationToken)).ShouldBeSameAs(accessShape);
        (await service.GetStarterAccessShapeAsync(cancellationToken)).ShouldBeSameAs(accessShape);
        (await service.GetStarterAccessProfileAsync(cancellationToken)).ShouldBeSameAs(starterProfile);
        await service.ReviseAccessProfilesAsync([revisionInput], cancellationToken);
        await service.ProcessPendingAccessProfileRevisionsAsync(cancellationToken);

        await repository.Received(1).AddUserForRegistrationAsync(registrationInput, cancellationToken);
        await repository.Received(1).AddRegistrationBaselineAsync(baselineInput, cancellationToken);
        await repository.Received(1).ReviseAccessProfilesAsync(
            Arg.Is<IReadOnlyCollection<AccessProfileRevisionInput>>(
                items => items != null && items.SequenceEqual(new[] { revisionInput })),
            cancellationToken);
        await repository.Received(1).ProcessPendingAccessProfileRevisionsAsync(cancellationToken);
    }

    [Fact]
    public async Task UserLookupService_preserves_missing_state_and_cancellation()
    {
        var repository = Substitute.For<IProfilingRepository>();
        repository.UserExistsAsync(404, Arg.Any<CancellationToken>()).Returns(false);
        repository.GetUserStateAsync(404, Arg.Any<CancellationToken>()).Returns((User?)null);

        using var provider = new ServiceCollection()
            .AddSingleton(repository)
            .AddProfilingModuleApplication()
            .BuildServiceProvider();
        var service = provider.GetRequiredService<IUserLookupService>();

        (await service.UserExistsAsync(404)).ShouldBeFalse();
        (await service.GetUserStateAsync(404)).ShouldBeNull();
        var canceled = new CancellationToken(canceled: true);
        (await service.GetUserStateAsync(405, canceled)).ShouldBeNull();
        await repository.Received(1).GetUserStateAsync(405, canceled);
    }
}
