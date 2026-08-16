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
        int? userId = null,
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
        int? userId = null,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        int resumeId,
        int? userId = null,
        CancellationToken cancellationToken = default);
}

