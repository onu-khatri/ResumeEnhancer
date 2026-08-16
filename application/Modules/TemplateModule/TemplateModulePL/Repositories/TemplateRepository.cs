using Microsoft.EntityFrameworkCore;
using ResumeEnhancer.Infrastructure.Caching;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.TemplateModule.DM.Entities;
using ResumeEnhancer.TemplateModule.SL.Abstractions.Persistence;

namespace ResumeEnhancer.TemplateModule.PL.Repositories;

public sealed class TemplateRepository : ITemplateRepository
{
    private static readonly string[] SetupCacheKeys =
    [
        TemplateSetupDataRepository.TemplateCategoriesCacheKey,
        TemplateSetupDataRepository.TemplateRenderTypesCacheKey
    ];

    private readonly IUnitOfWork<AppDbContext> _unitOfWork;
    private readonly ICacheProvider _cacheProvider;

    public TemplateRepository(IUnitOfWork<AppDbContext> unitOfWork, ICacheProvider cacheProvider)
    {
        _unitOfWork = unitOfWork;
        _cacheProvider = cacheProvider;
    }

    public async Task<TemplateCategory> AddTemplateCategoryAsync(TemplateCategory category, int? auditUserId, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.GetRepo<TemplateCategory>().AddAsync(category, cancellationToken);
        await SaveAsync(auditUserId, cancellationToken);
        return category;
    }

    public async Task<TemplateCategory?> GetTemplateCategoryAsync(int templateCategoryId, bool track = false, CancellationToken cancellationToken = default)
    {
        IQueryable<TemplateCategory> query = _unitOfWork.GetRepo<TemplateCategory>()
            .Query()
            .Include(category => category.Templates);

        if (!track)
        {
            query = query.AsNoTracking();
        }

        return await query.SingleOrDefaultAsync(category => category.Id == templateCategoryId, cancellationToken);
    }

    public async Task<IReadOnlyList<TemplateCategory>> ListTemplateCategoriesAsync(CancellationToken cancellationToken = default) =>
        await _unitOfWork.GetRepo<TemplateCategory>()
            .Query()
            .AsNoTracking()
            .OrderBy(category => category.DisplayName)
            .ToListAsync(cancellationToken);

    public async Task DeleteTemplateCategoryAsync(TemplateCategory category, int? auditUserId, CancellationToken cancellationToken = default)
    {
        _unitOfWork.DbContext.Remove(category);
        await SaveAsync(auditUserId, cancellationToken);
    }

    public async Task<Template> AddTemplateAsync(Template template, int? auditUserId, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.GetRepo<Template>().AddAsync(template, cancellationToken);
        await SaveAsync(auditUserId, cancellationToken);
        return template;
    }

    public async Task<Template?> GetTemplateAsync(int templateId, bool track = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Template> query = _unitOfWork.GetRepo<Template>()
            .Query()
            .Include(template => template.TemplateCategory)
            .Include(template => template.RenderType);

        if (!track)
        {
            query = query.AsNoTracking();
        }

        return await query.SingleOrDefaultAsync(template => template.Id == templateId, cancellationToken);
    }

    public async Task<IReadOnlyList<Template>> ListTemplatesAsync(CancellationToken cancellationToken = default) =>
        await _unitOfWork.GetRepo<Template>()
            .Query()
            .AsNoTracking()
            .OrderBy(template => template.DisplayName)
            .ToListAsync(cancellationToken);

    public async Task DeleteTemplateAsync(Template template, int? auditUserId, CancellationToken cancellationToken = default)
    {
        _unitOfWork.DbContext.Remove(template);
        await SaveAsync(auditUserId, cancellationToken);
    }

    public async Task<bool> TemplateExistsAsync(int templateId, CancellationToken cancellationToken = default) =>
        await _unitOfWork.GetRepo<Template>().ExistsAsync(templateId, cancellationToken);

    public async Task<bool> TemplateCategoryExistsAsync(int templateCategoryId, CancellationToken cancellationToken = default) =>
        await _unitOfWork.GetRepo<TemplateCategory>().ExistsAsync(templateCategoryId, cancellationToken);

    public async Task SaveAsync(int? auditUserId, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.SaveAsync(new RepositoryAudit(auditUserId), cancellationToken);
        await InvalidateSetupCacheAsync(cancellationToken);
    }

    private async Task InvalidateSetupCacheAsync(CancellationToken cancellationToken)
    {
        foreach (var cacheKey in SetupCacheKeys)
        {
            await _cacheProvider.RemoveAsync(cacheKey, cancellationToken);
        }
    }

    private sealed class RepositoryAudit : IAudit
    {
        public RepositoryAudit(int? userId) => UserId = userId;
        public int? UserId { get; }
    }
}
