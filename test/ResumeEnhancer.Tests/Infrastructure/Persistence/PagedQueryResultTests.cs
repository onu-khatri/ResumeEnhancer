using Shouldly;
using Persistence;

namespace ResumeEnhancer.Tests.Infrastructure.Persistence;

public sealed class PagedQueryResultTests
{
    [Fact]
    public void Constructor_ItemsNull_ThrowsArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() => new PagedQueryResult<int>(null!, 1, 10, 0));
    }

    [Fact]
    public void TotalPages_PageSizeIsZero_ReturnsZero()
    {
        var result = new PagedQueryResult<int>([], 1, 0, 10);

        result.TotalPages.ShouldBe(0);
        result.HasNextPage.ShouldBeFalse();
        result.HasPreviousPage.ShouldBeFalse();
    }

    [Fact]
    public void PagingFlags_MiddlePage_ReturnsPreviousAndNext()
    {
        var result = new PagedQueryResult<int>([1], 2, 10, 25);

        result.TotalPages.ShouldBe(3);
        result.HasPreviousPage.ShouldBeTrue();
        result.HasNextPage.ShouldBeTrue();
    }
}
