using ResumeEnhancer.Core.DomainLibrary.DomainModel;
using ResumeEnhancer.ResumeModule.DM.Entities;

namespace ResumeEnhancer.ResumeModule.SL.Abstractions.Persistence;

public interface IResumeRepository
{
    Task<Resume> AddAsync(
        Resume resume,
        int? auditUserId,
        CancellationToken cancellationToken = default);

    Task<Resume?> GetAsync(
        int resumeId,
        string? userId = null,
        bool track = false,
        CancellationToken cancellationToken = default);

    Task<ResumeSearchResult> SearchAsync(
        ResumeSearchCriteria criteria,
        CancellationToken cancellationToken = default);

    void Remove(AuditEntity entity);

    Task<int> SaveAsync(
        int? auditUserId,
        CancellationToken cancellationToken = default);

    Task<ResumeDeleteResult> DeleteAsync(
        IReadOnlyList<int> resumeIds,
        int? auditUserId,
        string? userId = null,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        int resumeId,
        string? userId = null,
        CancellationToken cancellationToken = default);
}

