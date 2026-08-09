using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Persistence;
using ResumeEnhancer.TestUtilities.IntegrationSupport;
using ResumeModuleAM.Requests;
using ResumeModuleDM.Entities;

namespace ResumeEnhancer.IntegrationTests.Modules.ResumeModule;

[Collection("Sequential_ResumeModul")]
public sealed partial class ResumeQueryIntegrationTests : IClassFixture<ResumeModuleIntegrationTestFixture>
{
    private readonly ResumeModuleIntegrationTestFixture _fixture;

    public ResumeQueryIntegrationTests(ResumeModuleIntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory]
    [MemberData(nameof(GetResumeSetups))]
    public async Task GetResumeAsync_SetupObject_ExercisesRealHttpBoundary(ResumeEndpointSetup setup)
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
    public async Task ResumeExistsAsync_SetupObject_ExercisesRealHttpBoundary(ResumeEndpointSetup setup)
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
        ResumeEndpointSetup<ResumeSearchRequest> setup)
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
        CancellationToken cancellationToken)
    {
        var dbContext = (AppDbContext)setupper.GetFreshDbContext();

        return await dbContext.Set<Resume>().CountAsync(cancellationToken);
    }
}
