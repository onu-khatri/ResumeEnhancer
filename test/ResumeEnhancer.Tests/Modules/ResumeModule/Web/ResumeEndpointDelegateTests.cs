using System.Reflection;
using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Shouldly;
using ResumeEnhancer.Tests.TestInfrastructure;
using ResumeModuleAM.Requests;
using ResumeModuleAM.Responses;
using ResumeModuleSL.Contracts;
using ResumeModuleWeb.MiniApis.Commands;
using ResumeModuleWeb.MiniApis.Queries;

namespace ResumeEnhancer.Tests.Modules.ResumeModule.Web;

public sealed class ResumeEndpointDelegateTests
{
    [Fact]
    public async Task CreateResumeAsync_NullRequest_ReturnsValidationProblem()
    {
        var result = await InvokeCommandAsync(
            "CreateResumeAsync",
            null,
            new InlineValidator<CreateResumeRequest>(),
            Substitute.For<IMediator>(),
            new DefaultHttpContext(),
            TestContext.Current.CancellationToken);

        var snapshot = await result.ExecuteAsync();

        snapshot.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        snapshot.ReadJson().GetProperty("errors").GetProperty("request")[0].GetString()
            .ShouldBe("Request body is required.");
    }

    [Fact]
    public async Task CreateResumeAsync_ValidRequest_SendsAuditUserAndReturnsCreated()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<ICommand<ResumeDetailResponse>>(), cancellationToken)
            .Returns(new ValueTask<ResumeDetailResponse>(new ResumeDetailResponse
            {
                Id = 12,
                Title = "Created"
            }));
        var httpContext = HttpContextWithHeaders(auditUserId: "42");

        var result = await InvokeCommandAsync(
            "CreateResumeAsync",
            ResumeTestData.CreateResumeRequest(),
            new InlineValidator<CreateResumeRequest>(),
            mediator,
            httpContext,
            cancellationToken);
        var snapshot = await result.ExecuteAsync();

        snapshot.StatusCode.ShouldBe(StatusCodes.Status201Created);
        snapshot.ReadJson().GetProperty("id").GetInt32().ShouldBe(12);
        await mediator.Received(1).Send(
            Arg.Is<ICommand<ResumeDetailResponse>>(command => IsCreateCommand(command, 42)),
            cancellationToken);
    }

    [Fact]
    public async Task UpdateResumeAsync_NullRequestAndInvalidId_ReturnsMergedValidationProblem()
    {
        var result = await InvokeCommandAsync(
            "UpdateResumeAsync",
            0,
            null,
            new InlineValidator<UpdateResumeRequest>(),
            Substitute.For<IMediator>(),
            new DefaultHttpContext(),
            TestContext.Current.CancellationToken);

        var snapshot = await result.ExecuteAsync();
        var errors = snapshot.ReadJson().GetProperty("errors");

        snapshot.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        errors.GetProperty("resumeId")[0].GetString().ShouldBe("Resume id must be greater than 0.");
        errors.GetProperty("request")[0].GetString().ShouldBe("Request body is required.");
    }

    [Fact]
    public async Task UpdateResumeAsync_ValidRequest_SendsAuditAndUserHeadersAndReturnsOk()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<ICommand<ResumeDetailResponse>>(), cancellationToken)
            .Returns(new ValueTask<ResumeDetailResponse>(new ResumeDetailResponse
            {
                Id = 7,
                Title = "Updated"
            }));
        var httpContext = HttpContextWithHeaders(auditUserId: "9", userId: " owner ");

        var result = await InvokeCommandAsync(
            "UpdateResumeAsync",
            7,
            new UpdateResumeRequest { Title = "Updated" },
            new InlineValidator<UpdateResumeRequest>(),
            mediator,
            httpContext,
            cancellationToken);
        var snapshot = await result.ExecuteAsync();

        snapshot.StatusCode.ShouldBe(StatusCodes.Status200OK);
        snapshot.ReadJson().GetProperty("title").GetString().ShouldBe("Updated");
        await mediator.Received(1).Send(
            Arg.Is<ICommand<ResumeDetailResponse>>(command => IsUpdateCommand(command, 7, 9, "owner")),
            cancellationToken);
    }

    [Fact]
    public async Task DeleteResumeAsync_InvalidId_ReturnsValidationProblem()
    {
        var result = await InvokeCommandAsync(
            "DeleteResumeAsync",
            -1,
            Substitute.For<IMediator>(),
            new DefaultHttpContext(),
            TestContext.Current.CancellationToken);

        var snapshot = await result.ExecuteAsync();

        snapshot.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        snapshot.ReadJson().GetProperty("errors").GetProperty("resumeId")[0].GetString()
            .ShouldBe("Resume id must be greater than 0.");
    }

    [Fact]
    public async Task DeleteResumeAsync_ValidId_SendsHeadersAndReturnsOk()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<ICommand<ResumeDeleteResponse>>(), cancellationToken)
            .Returns(new ValueTask<ResumeDeleteResponse>(
                new ResumeDeleteResponse([5], [5], [], [])));
        var httpContext = HttpContextWithHeaders(auditUserId: "11", userId: "owner");

        var result = await InvokeCommandAsync(
            "DeleteResumeAsync",
            5,
            mediator,
            httpContext,
            cancellationToken);
        var snapshot = await result.ExecuteAsync();

        snapshot.StatusCode.ShouldBe(StatusCodes.Status200OK);
        snapshot.ReadJson().GetProperty("deletedCount").GetInt32().ShouldBe(1);
        await mediator.Received(1).Send(
            Arg.Is<ICommand<ResumeDeleteResponse>>(command => IsDeleteCommand(command, 5, 11, "owner")),
            cancellationToken);
    }

    [Fact]
    public async Task DeleteResumesAsync_NullRequest_ReturnsValidationProblem()
    {
        var result = await InvokeCommandAsync(
            "DeleteResumesAsync",
            null,
            new InlineValidator<DeleteResumesRequest>(),
            Substitute.For<IMediator>(),
            new DefaultHttpContext(),
            TestContext.Current.CancellationToken);

        var snapshot = await result.ExecuteAsync();

        snapshot.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        snapshot.ReadJson().GetProperty("errors").GetProperty("request")[0].GetString()
            .ShouldBe("Request body is required.");
    }

    [Fact]
    public async Task DeleteResumesAsync_ValidRequest_SendsHeadersAndReturnsOk()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<ICommand<ResumeDeleteResponse>>(), cancellationToken)
            .Returns(new ValueTask<ResumeDeleteResponse>(
                new ResumeDeleteResponse([1, 2], [1], [2], [])));
        var httpContext = HttpContextWithHeaders(auditUserId: "13", userId: "owner");

        var result = await InvokeCommandAsync(
            "DeleteResumesAsync",
            new DeleteResumesRequest { ResumeIds = [1, 2] },
            new InlineValidator<DeleteResumesRequest>(),
            mediator,
            httpContext,
            cancellationToken);
        var snapshot = await result.ExecuteAsync();

        snapshot.StatusCode.ShouldBe(StatusCodes.Status200OK);
        snapshot.ReadJson().GetProperty("hasFailures").GetBoolean().ShouldBeTrue();
        await mediator.Received(1).Send(
            Arg.Is<ICommand<ResumeDeleteResponse>>(command =>
                IsDeleteResumesCommand(command, new[] { 1, 2 }, 13, "owner")),
            cancellationToken);
    }

    [Fact]
    public async Task GetResumeAsync_MissingResume_ReturnsNotFound()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<IQuery<ResumeDetailResponse?>>(), cancellationToken)
            .Returns(new ValueTask<ResumeDetailResponse?>((ResumeDetailResponse?)null));

        var result = await InvokeQueryAsync(
            "GetResumeAsync",
            1,
            mediator,
            new DefaultHttpContext(),
            cancellationToken);
        var snapshot = await result.ExecuteAsync();

        snapshot.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task GetResumeAsync_FoundResume_SendsUserHeaderAndReturnsOk()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<IQuery<ResumeDetailResponse?>>(), cancellationToken)
            .Returns(new ValueTask<ResumeDetailResponse?>(new ResumeDetailResponse
            {
                Id = 3,
                Title = "Found"
            }));
        var httpContext = HttpContextWithHeaders(userId: " owner ");

        var result = await InvokeQueryAsync(
            "GetResumeAsync",
            3,
            mediator,
            httpContext,
            cancellationToken);
        var snapshot = await result.ExecuteAsync();

        snapshot.StatusCode.ShouldBe(StatusCodes.Status200OK);
        snapshot.ReadJson().GetProperty("title").GetString().ShouldBe("Found");
        await mediator.Received(1).Send(
            Arg.Is<IQuery<ResumeDetailResponse?>>(query => IsGetResumeQuery(query, 3, "owner")),
            cancellationToken);
    }

    [Fact]
    public async Task SearchResumesAsync_InvalidRequest_ReturnsValidationProblem()
    {
        var validator = new InlineValidator<ResumeSearchRequest>();
        validator.RuleFor(request => request.PageNumber).GreaterThan(0);

        var result = await InvokeQueryAsync(
            "SearchResumesAsync",
            new ResumeSearchRequest { PageNumber = 0 },
            validator,
            Substitute.For<IMediator>(),
            TestContext.Current.CancellationToken);
        var snapshot = await result.ExecuteAsync();

        snapshot.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        snapshot.ReadJson().GetProperty("errors").TryGetProperty("PageNumber", out _).ShouldBeTrue();
    }

    [Fact]
    public async Task SearchResumesAsync_NullRequest_ReturnsValidationProblem()
    {
        var result = await InvokeQueryAsync(
            "SearchResumesAsync",
            null,
            new InlineValidator<ResumeSearchRequest>(),
            Substitute.For<IMediator>(),
            TestContext.Current.CancellationToken);

        var snapshot = await result.ExecuteAsync();

        snapshot.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        snapshot.ReadJson().GetProperty("errors").GetProperty("request")[0].GetString()
            .ShouldBe("Request body is required.");
    }

    [Fact]
    public async Task SearchResumesAsync_ValidRequest_SendsQueryAndReturnsOk()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<IQuery<ResumeSearchResponse>>(), cancellationToken)
            .Returns(new ValueTask<ResumeSearchResponse>(
                new ResumeSearchResponse([new ResumeListItemResponse { Id = 1, Title = "One" }], 1, 10, 1)));

        var result = await InvokeQueryAsync(
            "SearchResumesAsync",
            new ResumeSearchRequest { UserId = " user " },
            new InlineValidator<ResumeSearchRequest>(),
            mediator,
            cancellationToken);
        var snapshot = await result.ExecuteAsync();

        snapshot.StatusCode.ShouldBe(StatusCodes.Status200OK);
        snapshot.ReadJson().GetProperty("items")[0].GetProperty("id").GetInt32().ShouldBe(1);
        await mediator.Received(1).Send(
            Arg.Is<IQuery<ResumeSearchResponse>>(query => IsSearchResumesQuery(query, " user ")),
            cancellationToken);
    }

    [Fact]
    public async Task ResumeExistsAsync_ValidId_SendsUserHeaderAndReturnsOk()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<IQuery<bool>>(), cancellationToken)
            .Returns(new ValueTask<bool>(true));
        var httpContext = HttpContextWithHeaders(userId: " owner ");

        var result = await InvokeQueryAsync(
            "ResumeExistsAsync",
            4,
            mediator,
            httpContext,
            cancellationToken);
        var snapshot = await result.ExecuteAsync();

        snapshot.StatusCode.ShouldBe(StatusCodes.Status200OK);
        snapshot.ReadJson().GetBoolean().ShouldBeTrue();
        await mediator.Received(1).Send(
            Arg.Is<IQuery<bool>>(query => IsResumeExistsQuery(query, 4, "owner")),
            cancellationToken);
    }

    private static async Task<IResult> InvokeCommandAsync(string methodName, params object?[] arguments) =>
        await InvokeEndpointAsync(typeof(ResumeCommandEndpoints), methodName, arguments);

    private static async Task<IResult> InvokeQueryAsync(string methodName, params object?[] arguments) =>
        await InvokeEndpointAsync(typeof(ResumeQueryEndpoints), methodName, arguments);

    private static async Task<IResult> InvokeEndpointAsync(
        Type endpointType,
        string methodName,
        object?[] arguments)
    {
        var method = endpointType.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
        method.ShouldNotBeNull();

        return await (Task<IResult>)method!.Invoke(null, arguments)!;
    }

    private static DefaultHttpContext HttpContextWithHeaders(
        string? auditUserId = null,
        string? userId = null)
    {
        var httpContext = new DefaultHttpContext();

        if (auditUserId is not null)
        {
            httpContext.Request.Headers["X-Audit-UserId"] = auditUserId;
        }

        if (userId is not null)
        {
            httpContext.Request.Headers["X-User-Id"] = userId;
        }

        return httpContext;
    }

    private static bool IsCreateCommand(ICommand<ResumeDetailResponse>? command, int auditUserId) =>
        command is CreateResumeCommand createCommand
        && createCommand.AuditUserId == auditUserId;

    private static bool IsUpdateCommand(
        ICommand<ResumeDetailResponse>? command,
        int resumeId,
        int auditUserId,
        string userId) =>
        command is UpdateResumeCommand updateCommand
        && updateCommand.ResumeId == resumeId
        && updateCommand.AuditUserId == auditUserId
        && updateCommand.UserId == userId;

    private static bool IsDeleteCommand(
        ICommand<ResumeDeleteResponse>? command,
        int resumeId,
        int auditUserId,
        string userId) =>
        command is DeleteResumeCommand deleteCommand
        && deleteCommand.ResumeId == resumeId
        && deleteCommand.AuditUserId == auditUserId
        && deleteCommand.UserId == userId;

    private static bool IsDeleteResumesCommand(
        ICommand<ResumeDeleteResponse>? command,
        int[] resumeIds,
        int auditUserId,
        string userId) =>
        command is DeleteResumesCommand deleteCommand
        && deleteCommand.ResumeIds.SequenceEqual(resumeIds)
        && deleteCommand.AuditUserId == auditUserId
        && deleteCommand.UserId == userId;

    private static bool IsGetResumeQuery(
        IQuery<ResumeDetailResponse?>? query,
        int resumeId,
        string userId) =>
        query is GetResumeQuery getQuery
        && getQuery.ResumeId == resumeId
        && getQuery.UserId == userId;

    private static bool IsSearchResumesQuery(
        IQuery<ResumeSearchResponse>? query,
        string userId) =>
        query is SearchResumesQuery searchQuery
        && searchQuery.Request.UserId == userId;

    private static bool IsResumeExistsQuery(
        IQuery<bool>? query,
        int resumeId,
        string userId) =>
        query is ResumeExistsQuery existsQuery
        && existsQuery.ResumeId == resumeId
        && existsQuery.UserId == userId;
}
