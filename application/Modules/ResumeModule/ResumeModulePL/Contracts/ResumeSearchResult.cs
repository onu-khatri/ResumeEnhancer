using ResumeModuleDM.Entities;

namespace ResumeModulePL.Contracts;

public sealed class ResumeSearchResult
{
    public ResumeSearchResult(
        IReadOnlyList<Resume> items,
        int pageNumber,
        int pageSize,
        int totalCount)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public IReadOnlyList<Resume> Items { get; }

    public int PageNumber { get; }

    public int PageSize { get; }

    public int TotalCount { get; }
}
