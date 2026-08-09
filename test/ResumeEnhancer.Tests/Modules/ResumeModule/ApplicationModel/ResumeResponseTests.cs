using Shouldly;
using ResumeModuleAM.Responses;

namespace ResumeEnhancer.Tests.Modules.ResumeModule.ApplicationModel;

public sealed class ResumeResponseTests
{
    [Fact]
    public void ResumeSearchResponse_PageSizeIsZero_ReturnsNoNextPage()
    {
        var response = new ResumeSearchResponse([], pageNumber: 1, pageSize: 0, totalCount: 5);

        response.TotalPages.ShouldBe(0);
        response.HasPreviousPage.ShouldBeFalse();
        response.HasNextPage.ShouldBeFalse();
    }

    [Fact]
    public void ResumeSearchResponse_MiddlePage_ReturnsPagingFlags()
    {
        var response = new ResumeSearchResponse([], pageNumber: 2, pageSize: 10, totalCount: 25);

        response.TotalPages.ShouldBe(3);
        response.HasPreviousPage.ShouldBeTrue();
        response.HasNextPage.ShouldBeTrue();
    }

    [Fact]
    public void ResumeDeleteResponse_NotFoundOrForbiddenIdsExist_ReportsFailures()
    {
        var response = new ResumeDeleteResponse(
            requestedIds: [1, 2, 3],
            deletedIds: [1],
            notFoundIds: [2],
            forbiddenIds: [3]);

        response.DeletedCount.ShouldBe(1);
        response.HasFailures.ShouldBeTrue();
    }

    [Fact]
    public void ResumeDeleteResponse_AllRequestedIdsDeleted_ReportsNoFailures()
    {
        var response = new ResumeDeleteResponse(
            requestedIds: [1],
            deletedIds: [1],
            notFoundIds: [],
            forbiddenIds: []);

        response.DeletedCount.ShouldBe(1);
        response.HasFailures.ShouldBeFalse();
    }
}
