// Repository Implementation Template (ResumeEnhancer)
//
// Mirrors <ModuleName>ModulePL.Repositories.ResumeRepository. Use this shape when adding
// a new repository to a module:
//   - The abstraction lives in <ModuleName>ModuleSL/Abstractions/Persistence.
//   - The EF implementation lives in <ModuleName>ModulePL/Repositories and wraps
//     IUnitOfWork<AppDbContext> (never inject a DbSet directly).
//   - Persistence flows through SaveAsync(IAudit auditUser, ct) so the audit
//     pipeline records App_Create/App_Update user and date.

using DomainLibrary.DomainModel;
using Microsoft.EntityFrameworkCore;
using Persistence;
using <ModuleName>ModuleDM.Entities;
using <ModuleName>ModuleSL.Abstractions.Persistence;

namespace <ModuleName>ModulePL.Repositories;

public sealed class ResumeRepository : IResumeRepository
{
    private const int MaxPageSize = 100;

    private readonly IUnitOfWork<AppDbContext> _unitOfWork;

    public ResumeRepository(IUnitOfWork<AppDbContext> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Resume> AddAsync(
        Resume resume,
        int? auditUserId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(resume);

        await _unitOfWork.GetRepo<Resume>().AddAsync(resume, cancellationToken);
        await _unitOfWork.SaveAsync(new RepositoryAudit(auditUserId), cancellationToken);

        return resume;
    }

    public async Task<Resume?> GetAsync(
        int resumeId,
        string? userId = null,
        bool track = false,
        CancellationToken cancellationToken = default)
    {
        EnsurePositiveId(resumeId, nameof(resumeId));

        var query = ApplyResumeGraphIncludes(_unitOfWork.GetRepo<Resume>().Query());

        if (!track)
        {
            query = query.AsNoTracking();
        }

        query = query.Where(resume => resume.Id == resumeId);

        if (!string.IsNullOrWhiteSpace(userId))
        {
            query = query.Where(resume => resume.UserId == userId.Trim());
        }

        return await query.SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<ResumeSearchResult> SearchAsync(
        ResumeSearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(criteria);
        ValidatePaging(criteria.PageNumber, criteria.PageSize);

        var query = _unitOfWork.GetRepo<Resume>().Query().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(criteria.UserId))
        {
            var userId = criteria.UserId.Trim();
            query = query.Where(resume => resume.UserId == userId);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await ApplySort(query, criteria.SortBy, criteria.SortDirection)
            .Skip((criteria.PageNumber - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .Include(resume => resume.Education)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        return new ResumeSearchResult(items, criteria.PageNumber, criteria.PageSize, totalCount);
    }

    public void Remove(AuditEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _unitOfWork.DbContext.Remove(entity);
    }

    public async Task<int> SaveAsync(int? auditUserId, CancellationToken cancellationToken = default) =>
        await _unitOfWork.SaveAsync(new RepositoryAudit(auditUserId), cancellationToken);

    public async Task<bool> ExistsAsync(
        int resumeId,
        string? userId = null,
        CancellationToken cancellationToken = default)
    {
        EnsurePositiveId(resumeId, nameof(resumeId));
        var normalizedUserId = string.IsNullOrWhiteSpace(userId) ? null : userId.Trim();

        return normalizedUserId is null
            ? await _unitOfWork.GetRepo<Resume>().ExistsAsync(resumeId, cancellationToken)
            : await _unitOfWork.GetRepo<Resume>().ExistsAsync(
                resume => resume.Id == resumeId && resume.UserId == normalizedUserId,
                cancellationToken);
    }

    // Wide graphs are loaded with explicit Include/ThenInclude + AsSplitQuery.
    private static IQueryable<Resume> ApplyResumeGraphIncludes(IQueryable<Resume> query) =>
        query
            .Include(resume => resume.PersonalInformation)!
                .ThenInclude(p => p!.Address)
            .Include(resume => resume.Education)
            .AsSplitQuery();

    private static IQueryable<Resume> ApplySort(
        IQueryable<Resume> query,
        ResumeSortBy sortBy,
        ResumeSortDirection direction) =>
        (sortBy, direction) switch
        {
            (ResumeSortBy.Title, ResumeSortDirection.Ascending) =>
                query.OrderBy(resume => resume.Title).ThenBy(resume => resume.Id),
            (ResumeSortBy.CreatedDate, ResumeSortDirection.Descending) =>
                query.OrderByDescending(resume => resume.App_CreateDate).ThenByDescending(resume => resume.Id),
            (ResumeSortBy.Id, ResumeSortDirection.Ascending) =>
                query.OrderBy(resume => resume.Id),
            (ResumeSortBy.Id, ResumeSortDirection.Descending) =>
                query.OrderByDescending(resume => resume.Id),
            (_, ResumeSortDirection.Ascending) =>
                query.OrderBy(resume => resume.App_UpdateDate ?? resume.App_CreateDate).ThenBy(resume => resume.Id),
            _ =>
                query.OrderByDescending(resume => resume.App_UpdateDate ?? resume.App_CreateDate)
                    .ThenByDescending(resume => resume.Id)
        };

    private static void ValidatePaging(int pageNumber, int pageSize)
    {
        if (pageNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(pageNumber), pageNumber, "Page number must be >= 1.");

        if (pageSize < 1 || pageSize > MaxPageSize)
            throw new ArgumentOutOfRangeException(nameof(pageSize), pageSize, $"Page size must be 1..{MaxPageSize}.");
    }

    private static void EnsurePositiveId(int id, string fieldName)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(fieldName, id, $"{fieldName} must be greater than 0.");
    }

    private sealed class RepositoryAudit : IAudit
    {
        public RepositoryAudit(int? userId) => UserId = userId;
        public int? UserId { get; }
    }
}
