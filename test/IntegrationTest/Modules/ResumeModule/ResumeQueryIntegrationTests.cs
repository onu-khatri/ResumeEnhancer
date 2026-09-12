using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.ResumeModule.AM.Requests;
using ResumeEnhancer.ResumeModule.DM.Entities;
using ResumeEnhancer.TestUtilities.IntegrationSupport;
using ResumeEnhancer.Tests.Integration.TestSupport;

namespace ResumeEnhancer.Tests.Integration.Modules.ResumeModule;

[Collection("Sequential_Integration")]
public sealed partial class ResumeQueryIntegrationTests
{
    private readonly IntegrationTestAssemblyFixture _fixture;

    public ResumeQueryIntegrationTests(IntegrationTestAssemblyFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory]
    [MemberData(nameof(GetResumeSetups))]
    public async Task GetResumeAsync_SetupObject_ExercisesRealHttpBoundary(EndpointSetup setup)
    {
        using var setupper = _fixture.CreateSetupper();
        var cancellationToken = TestContext.Current.CancellationToken;

        setupper.ClearDbContext();
        await setup.ArrangeAsync(setupper, setup, cancellationToken);

        using var client = _fixture.Utilities.CreateClient();
        var response = await client.GetAsync(setup.Route, cancellationToken);

        await setup.AssertAsync(setupper, response, cancellationToken);
    }

    [Theory]
    [MemberData(nameof(ResumeExistsSetups))]
    public async Task ResumeExistsAsync_SetupObject_ExercisesRealHttpBoundary(EndpointSetup setup)
    {
        using var setupper = _fixture.CreateSetupper();
        var cancellationToken = TestContext.Current.CancellationToken;

        setupper.ClearDbContext();
        await setup.ArrangeAsync(setupper, setup, cancellationToken);

        using var client = _fixture.Utilities.CreateClient();
        var response = await client.GetAsync(setup.Route, cancellationToken);

        await setup.AssertAsync(setupper, response, cancellationToken);
    }

    [Theory]
    [MemberData(nameof(SearchResumeSetups))]
    public async Task SearchResumesAsync_SetupObject_ExercisesRealHttpBoundary(
        EndpointSetup<ResumeSearchRequest> setup
    )
    {
        using var setupper = _fixture.CreateSetupper();
        var cancellationToken = TestContext.Current.CancellationToken;

        setupper.ClearDbContext();
        await setup.ArrangeAsync(setupper, setup, cancellationToken);

        using var client = _fixture.Utilities.CreateClient();
        var response = await client.PostAsJsonAsync(setup.Route, setup.Input, cancellationToken);

        await setup.AssertAsync(setupper, response, cancellationToken);
    }

    private static async Task<int> CountResumesAsync(
        ISetupper setupper,
        CancellationToken cancellationToken
    )
    {
        var dbContext = (AppDbContext)setupper.GetFreshDbContext();

        return await dbContext.Set<Resume>().CountAsync(cancellationToken);
    }
}
