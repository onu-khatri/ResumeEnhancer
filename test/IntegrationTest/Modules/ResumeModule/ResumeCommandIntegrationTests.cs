using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Persistence;
using ResumeEnhancer.TestUtilities.IntegrationSupport;
using ResumeModuleAM.Requests;
using ResumeModuleDM.Entities;

namespace ResumeEnhancer.IntegrationTests.Modules.ResumeModule;

[Collection("Sequential_ResumeModul")]
public sealed partial class ResumeCommandIntegrationTests : IClassFixture<ResumeModuleIntegrationTestFixture>
{
    private readonly ResumeModuleIntegrationTestFixture _fixture;

    public ResumeCommandIntegrationTests(ResumeModuleIntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory]
    [MemberData(nameof(CreateResumeSetups))]
    public async Task CreateResumeAsync_SetupObject_ExercisesRealHttpBoundary(
        ResumeEndpointSetup<CreateResumeRequest> setup)
    {
        using var setupper = _fixture.CreateSetupper();
        var cancellationToken = TestContext.Current.CancellationToken;

        setupper.ClearDbContext();
        await setup.ArrangeAsync(setupper, setup, cancellationToken);

        using var client = _fixture.Utilities.CreateClient();
        var response = await SendAsync(client, setup, cancellationToken);

        await setup.AssertAsync(setupper, response, cancellationToken);
    }

    [Theory]
    [MemberData(nameof(UpdateResumeSetups))]
    public async Task UpdateResumeAsync_SetupObject_ExercisesRealHttpBoundary(
        ResumeEndpointSetup<UpdateResumeRequest> setup)
    {
        using var setupper = _fixture.CreateSetupper();
        var cancellationToken = TestContext.Current.CancellationToken;

        setupper.ClearDbContext();
        await setup.ArrangeAsync(setupper, setup, cancellationToken);

        using var client = _fixture.Utilities.CreateClient();
        var response = await SendAsync(client, setup, cancellationToken);

        await setup.AssertAsync(setupper, response, cancellationToken);
    }

    [Theory]
    [MemberData(nameof(DeleteResumeSetups))]
    public async Task DeleteResumeAsync_SetupObject_ExercisesRealHttpBoundary(ResumeEndpointSetup setup)
    {
        using var setupper = _fixture.CreateSetupper();
        var cancellationToken = TestContext.Current.CancellationToken;

        setupper.ClearDbContext();
        await setup.ArrangeAsync(setupper, setup, cancellationToken);

        using var client = _fixture.Utilities.CreateClient();
        var response = await client.DeleteAsync(setup.Route, cancellationToken);

        await setup.AssertAsync(setupper, response, cancellationToken);
    }

    [Theory]
    [MemberData(nameof(BulkDeleteResumeSetups))]
    public async Task DeleteResumesAsync_SetupObject_ExercisesRealHttpBoundary(
        ResumeEndpointSetup<DeleteResumesRequest> setup)
    {
        using var setupper = _fixture.CreateSetupper();
        var cancellationToken = TestContext.Current.CancellationToken;

        setupper.ClearDbContext();
        await setup.ArrangeAsync(setupper, setup, cancellationToken);

        using var client = _fixture.Utilities.CreateClient();
        var response = await SendAsync(client, setup, cancellationToken);

        await setup.AssertAsync(setupper, response, cancellationToken);
    }

    private static async Task<HttpResponseMessage> SendAsync<TRequest>(
        HttpClient client,
        ResumeEndpointSetup<TRequest> setup,
        CancellationToken cancellationToken)
    {
        if (setup.Method == HttpMethod.Post)
        {
            return await client.PostAsJsonAsync(setup.Route, setup.Input, cancellationToken);
        }

        if (setup.Method == HttpMethod.Put)
        {
            return await client.PutAsJsonAsync(setup.Route, setup.Input, cancellationToken);
        }

        throw new NotSupportedException($"HTTP method '{setup.Method}' is not supported by this test helper.");
    }

    private static async Task<int> CountResumesAsync(
        ISetupper setupper,
        CancellationToken cancellationToken)
    {
        var dbContext = (AppDbContext)setupper.GetFreshDbContext();

        return await dbContext.Set<Resume>().CountAsync(cancellationToken);
    }

    private static async Task<List<int>> ResumeIdsAsync(
        ISetupper setupper,
        CancellationToken cancellationToken)
    {
        var dbContext = (AppDbContext)setupper.GetFreshDbContext();

        return await dbContext.Set<Resume>()
            .Select(resume => resume.Id)
            .ToListAsync(cancellationToken);
    }

    private static async Task<Resume> SingleResumeAsync(
        ISetupper setupper,
        CancellationToken cancellationToken)
    {
        var dbContext = (AppDbContext)setupper.GetFreshDbContext();

        return await dbContext.Set<Resume>().SingleAsync(cancellationToken);
    }
}
