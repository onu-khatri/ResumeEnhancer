namespace ResumeModuleAM.Responses;

public sealed class ResumeDeleteResponse
{
    public ResumeDeleteResponse(
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

    public int DeletedCount => DeletedIds.Count;

    public bool HasFailures => NotFoundIds.Count > 0 || ForbiddenIds.Count > 0;
}
