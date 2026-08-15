using ResumeEnhancer.Core.DomainLibrary.DomainModel;
using Microsoft.EntityFrameworkCore;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.ResumeModule.DM.Entities;
using ResumeEnhancer.ResumeModule.SL.Abstractions.Persistence;

namespace ResumeEnhancer.ResumeModule.PL.Repositories;

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
        await SaveAsync(auditUserId, cancellationToken);

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
        ValidateDateRange(criteria.CreatedFromUtc, criteria.CreatedToUtc, "created");
        ValidateDateRange(criteria.UpdatedFromUtc, criteria.UpdatedToUtc, "updated");

        var query = _unitOfWork.GetRepo<Resume>().Query().AsNoTracking();
        var ids = NormalizeIdsForSearch(criteria.Ids);

        if (ids is { Length: 0 })
        {
            return new ResumeSearchResult(
                [],
                criteria.PageNumber,
                criteria.PageSize,
                totalCount: 0);
        }

        if (ids is not null)
        {
            query = query.Where(resume => ids.Contains(resume.Id));
        }

        if (!string.IsNullOrWhiteSpace(criteria.UserId))
        {
            var userId = criteria.UserId.Trim();
            query = query.Where(resume => resume.UserId == userId);
        }

        if (!string.IsNullOrWhiteSpace(criteria.ResumeTemplate))
        {
            var template = criteria.ResumeTemplate.Trim();
            query = query.Where(resume => resume.ResumeTemplate == template);
        }

        if (criteria.HasPhoto is true)
        {
            query = query.Where(resume => resume.Photo != null && resume.Photo != string.Empty);
        }
        else if (criteria.HasPhoto is false)
        {
            query = query.Where(resume => resume.Photo == null || resume.Photo == string.Empty);
        }

        if (criteria.CreatedFromUtc is not null)
        {
            query = query.Where(resume => resume.App_CreateDate >= criteria.CreatedFromUtc);
        }

        if (criteria.CreatedToUtc is not null)
        {
            query = query.Where(resume => resume.App_CreateDate <= criteria.CreatedToUtc);
        }

        if (criteria.UpdatedFromUtc is not null)
        {
            query = query.Where(resume => resume.App_UpdateDate >= criteria.UpdatedFromUtc);
        }

        if (criteria.UpdatedToUtc is not null)
        {
            query = query.Where(resume => resume.App_UpdateDate <= criteria.UpdatedToUtc);
        }

        if (!string.IsNullOrWhiteSpace(criteria.SearchText))
        {
            query = ApplySearchText(query, criteria.SearchText.Trim());
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await ApplySort(query, criteria.SortBy, criteria.SortDirection)
            .Skip((criteria.PageNumber - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .Include(resume => resume.Education)
            .Include(resume => resume.Certifications)
            .Include(resume => resume.Skills)
            .Include(resume => resume.WorkExperiences)
            .Include(resume => resume.Projects)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        return new ResumeSearchResult(
            items,
            criteria.PageNumber,
            criteria.PageSize,
            totalCount);
    }

    public void Remove(AuditEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _unitOfWork.DbContext.Remove(entity);
    }

    public async Task<int> SaveAsync(
        int? auditUserId,
        CancellationToken cancellationToken = default) =>
        await _unitOfWork.SaveAsync(new RepositoryAudit(auditUserId), cancellationToken);

    public async Task<ResumeDeleteResult> DeleteAsync(
        IReadOnlyList<int> resumeIds,
        int? auditUserId,
        string? userId = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(resumeIds);

        var requestedIds = NormalizeIdsForRequiredOperation(resumeIds, nameof(resumeIds));

        if (requestedIds.Length == 0)
        {
            return new ResumeDeleteResult([], [], [], []);
        }

        var resumes = await _unitOfWork.GetRepo<Resume>()
            .GetQuery(requestedIds.ToList())
            .ToListAsync(cancellationToken);

        var loadedIds = resumes.Select(resume => resume.Id).ToHashSet();
        var notFoundIds = requestedIds.Where(id => !loadedIds.Contains(id)).ToArray();
        var normalizedUserId = string.IsNullOrWhiteSpace(userId) ? null : userId.Trim();
        var allowedResumes = normalizedUserId is null
            ? resumes
            : resumes.Where(resume => resume.UserId == normalizedUserId).ToList();
        var allowedIds = allowedResumes.Select(resume => resume.Id).ToHashSet();
        var forbiddenIds = normalizedUserId is null
            ? []
            : resumes
                .Where(resume => !allowedIds.Contains(resume.Id))
                .Select(resume => resume.Id)
                .ToArray();

        if (allowedResumes.Count > 0)
        {
            _unitOfWork.GetRepo<Resume>().Delete(allowedResumes);
            await SaveAsync(auditUserId, cancellationToken);
        }

        return new ResumeDeleteResult(
            requestedIds,
            allowedIds.ToArray(),
            notFoundIds,
            forbiddenIds);
    }

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

    private static IQueryable<Resume> ApplyResumeGraphIncludes(IQueryable<Resume> query) =>
        query
            .Include(resume => resume.PersonalInformation)
                .ThenInclude(personalInformation => personalInformation!.Address)
            .Include(resume => resume.PersonalInformation)
                .ThenInclude(personalInformation => personalInformation!.Awards)
            .Include(resume => resume.PersonalInformation)
                .ThenInclude(personalInformation => personalInformation!.Languages)
            .Include(resume => resume.PersonalInformation)
                .ThenInclude(personalInformation => personalInformation!.Hobbies)
            .Include(resume => resume.PersonalInformation)
                .ThenInclude(personalInformation => personalInformation!.SocialMediaLinks)
            .Include(resume => resume.Education)
            .Include(resume => resume.Certifications)
            .Include(resume => resume.Skills)
            .Include(resume => resume.WorkExperiences)
            .Include(resume => resume.Projects)
            .AsSplitQuery();

    private static IQueryable<Resume> ApplySearchText(
        IQueryable<Resume> query,
        string searchText) =>
        query.Where(resume =>
            resume.Title.Contains(searchText)
            || (resume.Summary != null && resume.Summary.Contains(searchText))
            || (resume.ResumeTemplate != null && resume.ResumeTemplate.Contains(searchText))
            || (resume.PersonalInformation != null
                && ((resume.PersonalInformation.Email != null
                        && resume.PersonalInformation.Email.Contains(searchText))
                    || (resume.PersonalInformation.PhoneNumber != null
                        && resume.PersonalInformation.PhoneNumber.Contains(searchText))))
            || resume.Skills.Any(skill => skill.SkillName.Contains(searchText))
            || resume.WorkExperiences.Any(workExperience =>
                (workExperience.JobTitle != null && workExperience.JobTitle.Contains(searchText))
                || (workExperience.CompanyName != null && workExperience.CompanyName.Contains(searchText)))
            || resume.Projects.Any(project =>
                project.ProjectName.Contains(searchText)
                || (project.TechnologiesUsed != null
                    && project.TechnologiesUsed.Contains(searchText))));

    private static IQueryable<Resume> ApplySort(
        IQueryable<Resume> query,
        ResumeSortBy sortBy,
        ResumeSortDirection direction) =>
        (sortBy, direction) switch
        {
            (ResumeSortBy.Title, ResumeSortDirection.Ascending) =>
                query.OrderBy(resume => resume.Title).ThenBy(resume => resume.Id),
            (ResumeSortBy.Title, ResumeSortDirection.Descending) =>
                query.OrderByDescending(resume => resume.Title).ThenByDescending(resume => resume.Id),
            (ResumeSortBy.CreatedDate, ResumeSortDirection.Ascending) =>
                query.OrderBy(resume => resume.App_CreateDate).ThenBy(resume => resume.Id),
            (ResumeSortBy.CreatedDate, ResumeSortDirection.Descending) =>
                query.OrderByDescending(resume => resume.App_CreateDate).ThenByDescending(resume => resume.Id),
            (ResumeSortBy.ResumeTemplate, ResumeSortDirection.Ascending) =>
                query.OrderBy(resume => resume.ResumeTemplate).ThenBy(resume => resume.Id),
            (ResumeSortBy.ResumeTemplate, ResumeSortDirection.Descending) =>
                query.OrderByDescending(resume => resume.ResumeTemplate).ThenByDescending(resume => resume.Id),
            (ResumeSortBy.Id, ResumeSortDirection.Ascending) =>
                query.OrderBy(resume => resume.Id),
            (ResumeSortBy.Id, ResumeSortDirection.Descending) =>
                query.OrderByDescending(resume => resume.Id),
            (_, ResumeSortDirection.Ascending) =>
                query.OrderBy(resume => resume.App_UpdateDate ?? resume.App_CreateDate)
                    .ThenBy(resume => resume.Id),
            _ =>
                query.OrderByDescending(resume => resume.App_UpdateDate ?? resume.App_CreateDate)
                    .ThenByDescending(resume => resume.Id)
        };

    private static void ValidatePaging(int pageNumber, int pageSize)
    {
        if (pageNumber < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pageNumber),
                pageNumber,
                "Page number must be greater than or equal to 1.");
        }

        if (pageSize < 1 || pageSize > MaxPageSize)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pageSize),
                pageSize,
                $"Page size must be between 1 and {MaxPageSize}.");
        }
    }

    private static void ValidateDateRange(DateTime? from, DateTime? to, string label)
    {
        if (from is not null && to is not null && from > to)
        {
            throw new ArgumentException(
                $"{label} start date cannot be later than end date.");
        }
    }

    private static void EnsurePositiveId(int id, string fieldName)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(
                fieldName,
                id,
                $"{fieldName} must be greater than 0.");
        }
    }

    private static int[]? NormalizeIdsForSearch(IEnumerable<int>? ids) =>
        ids is null
            ? null
            : NormalizeIdsForRequiredOperation(ids, nameof(ids));

    private static int[] NormalizeIdsForRequiredOperation(
        IEnumerable<int> ids,
        string fieldName)
    {
        var normalizedIds = ids.Distinct().ToArray();
        var invalidId = normalizedIds.FirstOrDefault(id => id <= 0);

        if (invalidId <= 0 && normalizedIds.Contains(invalidId))
        {
            throw new ArgumentOutOfRangeException(
                fieldName,
                invalidId,
                $"{fieldName} cannot contain zero or negative ids.");
        }

        return normalizedIds;
    }

    private sealed class RepositoryAudit : IAudit
    {
        public RepositoryAudit(int? userId)
        {
            UserId = userId;
        }

        public int? UserId { get; }
    }
}

