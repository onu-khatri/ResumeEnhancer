namespace ResumeModulePL.Contracts;

public sealed class ResumeDeleteResult
{
    public ResumeDeleteResult(
        IReadOnlyList<int> requestedIds,
        IReadOnlyList<int> deletedIds,
        IReadOnlyList<int> notFoundIds,
        IReadOnlyList<int> forbiddenIds)
    {
        RequestedIds = requestedIds;
        DeletedIds = deletedIds;
        NotFoundIds = notFoundIds;
        ForbiddenIds = forbiddenIds;
    }

    public IReadOnlyList<int> RequestedIds { get; }

    public IReadOnlyList<int> DeletedIds { get; }

    public IReadOnlyList<int> NotFoundIds { get; }

    public IReadOnlyList<int> ForbiddenIds { get; }
}
