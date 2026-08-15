namespace ResumeEnhancer.Infrastructure.Persistence;

public sealed class PagedQueryResult<TElement>
{
    public PagedQueryResult(
        IReadOnlyList<TElement> items,
        int pageNumber,
        int pageSize,
        int totalCount)
    {
        ArgumentNullException.ThrowIfNull(items);

        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public IReadOnlyList<TElement> Items { get; }

    public int PageNumber { get; }

    public int PageSize { get; }

    public int TotalCount { get; }

    public int TotalPages => PageSize == 0
        ? 0
        : (int)Math.Ceiling(TotalCount / (double)PageSize);

    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;
}

