using NSubstitute;
using Shouldly;
using ResumeEnhancer.ProfilingModule.SL.Integrations;
using ResumeEnhancer.TemplateModule.SL.Integrations;
using ResumeEnhancer.Tests.Unit.TestInfrastructure;
using ResumeEnhancer.ResumeModule.AM.Requests;
using ResumeEnhancer.ResumeModule.DM.Entities;
using ResumeEnhancer.ResumeModule.SL.Abstractions.Persistence;
using ResumeEnhancer.ResumeModule.SL.Contracts;
using ResumeEnhancer.ResumeModule.SL.Handlers;

namespace ResumeEnhancer.Tests.Unit.Modules.ResumeModule.Application;

public sealed class ResumeHandlerTests
{
    [Fact]
    public async Task CreateResumeCommandHandler_ValidCommand_AddsResumeAndMapsResponse()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = Substitute.For<IResumeRepository>();
        var userLookupService = Substitute.For<IUserLookupService>();
        var templateLookupService = Substitute.For<ITemplateLookupService>();
        userLookupService.UserExistsAsync(ResumeTestData.UserId, cancellationToken).Returns(true);
        repository.AddAsync(Arg.Any<Resume>(), 42, cancellationToken)
            .Returns(call => Task.FromResult<Resume>(call.Arg<Resume>()!));
        var handler = new CreateResumeCommandHandler(repository, userLookupService, templateLookupService);

        var response = await handler.Handle(
            new CreateResumeCommand(ResumeTestData.CreateResumeRequest(), 42),
            cancellationToken);

        response.Title.ShouldBe("Senior Engineer");
        response.UserId.ShouldBe(ResumeTestData.UserId);
        await userLookupService.Received(1).UserExistsAsync(ResumeTestData.UserId, cancellationToken);
        await repository.Received(1).AddAsync(
            Arg.Is<Resume>(resume => resume != null && resume.Title == "Senior Engineer"),
            42,
            cancellationToken);
    }

    [Fact]
    public async Task UpdateResumeCommandHandler_ResumeMissing_ThrowsKeyNotFoundException()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = Substitute.For<IResumeRepository>();
        var userLookupService = Substitute.For<IUserLookupService>();
        var templateLookupService = Substitute.For<ITemplateLookupService>();
        repository.GetAsync(10, null, true, cancellationToken).Returns((Resume?)null);
        var handler = new UpdateResumeCommandHandler(repository, userLookupService, templateLookupService);

        await Should.ThrowAsync<KeyNotFoundException>(
            () => handler.Handle(
                new UpdateResumeCommand(10, new UpdateResumeRequest { Title = "Updated" }, 5),
                cancellationToken).AsTask());
    }

    [Fact]
    public async Task UpdateResumeCommandHandler_UserMismatch_ThrowsUnauthorizedAccessException()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = Substitute.For<IResumeRepository>();
        var userLookupService = Substitute.For<IUserLookupService>();
        var templateLookupService = Substitute.For<ITemplateLookupService>();
        repository.GetAsync(1, null, true, cancellationToken)
            .Returns(ResumeTestData.ResumeGraph(userId: ResumeTestData.UserId));
        var handler = new UpdateResumeCommandHandler(repository, userLookupService, templateLookupService);

        await Should.ThrowAsync<UnauthorizedAccessException>(
            () => handler.Handle(
                new UpdateResumeCommand(
                    1,
                    new UpdateResumeRequest { Title = "Updated" },
                    5,
                    ResumeTestData.OtherUserId),
                cancellationToken).AsTask());

        await repository.DidNotReceive().SaveAsync(Arg.Any<int?>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateResumeCommandHandler_ValidCommand_UpdatesAndSaves()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = Substitute.For<IResumeRepository>();
        var userLookupService = Substitute.For<IUserLookupService>();
        var templateLookupService = Substitute.For<ITemplateLookupService>();
        var resume = ResumeTestData.ResumeGraph();
        repository.GetAsync(1, null, true, cancellationToken).Returns(resume);
        userLookupService.UserExistsAsync(ResumeTestData.UserId, cancellationToken).Returns(true);
        var handler = new UpdateResumeCommandHandler(repository, userLookupService, templateLookupService);

        var response = await handler.Handle(
            new UpdateResumeCommand(
                1,
                new UpdateResumeRequest { Title = " Updated " },
                7,
                ResumeTestData.UserId),
            cancellationToken);

        response.Title.ShouldBe("Updated");
        await userLookupService.DidNotReceive().UserExistsAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
        await repository.Received(1).SaveAsync(7, cancellationToken);
    }

    [Fact]
    public async Task DeleteResumeCommandHandler_DelegatesSingleIdAndMapsResponse()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = Substitute.For<IResumeRepository>();
        repository.DeleteAsync(
                Arg.Any<IReadOnlyList<int>>(),
                9,
                ResumeTestData.UserId,
                cancellationToken)
            .Returns(new ResumeDeleteResult([1], [1], [], []));
        var handler = new DeleteResumeCommandHandler(repository);

        var response = await handler.Handle(
            new DeleteResumeCommand(1, 9, ResumeTestData.UserId),
            cancellationToken);

        response.DeletedCount.ShouldBe(1);
        await repository.Received(1).DeleteAsync(
            Arg.Is<IReadOnlyList<int>>(ids => ids != null && ids.Count == 1 && ids[0] == 1),
            9,
            ResumeTestData.UserId,
            cancellationToken);
    }

    [Fact]
    public async Task DeleteResumesCommandHandler_DelegatesIdsAndMapsResponse()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = Substitute.For<IResumeRepository>();
        repository.DeleteAsync(Arg.Any<IReadOnlyList<int>>(), 9, null, cancellationToken)
            .Returns(new ResumeDeleteResult([1, 2], [1], [2], []));
        var handler = new DeleteResumesCommandHandler(repository);

        var response = await handler.Handle(
            new DeleteResumesCommand([1, 2], 9),
            cancellationToken);

        response.DeletedCount.ShouldBe(1);
        response.HasFailures.ShouldBeTrue();
        await repository.Received(1).DeleteAsync(
            Arg.Is<IReadOnlyList<int>>(ids => ids != null && ids.Count == 2 && ids[0] == 1 && ids[1] == 2),
            9,
            null,
            cancellationToken);
    }

    [Fact]
    public async Task GetResumeQueryHandler_ResumeMissing_ReturnsNull()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = Substitute.For<IResumeRepository>();
        repository.GetAsync(1, ResumeTestData.UserId, false, cancellationToken)
            .Returns((Resume?)null);
        var handler = new GetResumeQueryHandler(repository);

        var response = await handler.Handle(
            new GetResumeQuery(1, ResumeTestData.UserId),
            cancellationToken);

        response.ShouldBeNull();
    }

    [Fact]
    public async Task GetResumeQueryHandler_ResumeExists_MapsDetail()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = Substitute.For<IResumeRepository>();
        repository.GetAsync(1, ResumeTestData.UserId, false, cancellationToken)
            .Returns(ResumeTestData.ResumeGraph());
        var handler = new GetResumeQueryHandler(repository);

        var response = await handler.Handle(
            new GetResumeQuery(1, ResumeTestData.UserId),
            cancellationToken);

        response!.Title.ShouldBe("Senior Engineer");
        response.PersonalInformation.ShouldNotBeNull();
    }

    [Fact]
    public async Task SearchResumesQueryHandler_DelegatesCriteriaAndMapsResponse()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = Substitute.For<IResumeRepository>();
        repository.SearchAsync(Arg.Any<ResumeSearchCriteria>(), cancellationToken)
            .Returns(new ResumeSearchResult([ResumeTestData.ResumeGraph()], 1, 25, 1));
        var handler = new SearchResumesQueryHandler(repository);

        var response = await handler.Handle(
            new SearchResumesQuery(new ResumeSearchRequest { UserId = ResumeTestData.UserId }),
            cancellationToken);

        response.Items.ShouldHaveSingleItem();
        await repository.Received(1).SearchAsync(
            Arg.Is<ResumeSearchCriteria>(criteria => criteria != null && criteria.UserId == ResumeTestData.UserId),
            cancellationToken);
    }

    [Fact]
    public async Task ResumeExistsQueryHandler_DelegatesToRepository()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = Substitute.For<IResumeRepository>();
        repository.ExistsAsync(3, ResumeTestData.UserId, cancellationToken).Returns(true);
        var handler = new ResumeExistsQueryHandler(repository);

        var exists = await handler.Handle(
            new ResumeExistsQuery(3, ResumeTestData.UserId),
            cancellationToken);

        exists.ShouldBeTrue();
    }
}


