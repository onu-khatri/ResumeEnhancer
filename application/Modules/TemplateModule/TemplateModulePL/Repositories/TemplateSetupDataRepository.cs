using Microsoft.EntityFrameworkCore;
using ResumeEnhancer.Infrastructure.Caching;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.TemplateModule.DM.Entities;
using ResumeEnhancer.TemplateModule.SL.Abstractions.Persistence;

namespace ResumeEnhancer.TemplateModule.PL.Repositories;

public sealed class TemplateSetupDataRepository(
    IUnitOfWork<AppDbContext> unitOfWork,
    ICacheProvider cacheProvider) : ITemplateSetupDataRepository
{
    internal const string TemplateCategoriesCacheKey = "template:setup:categories";
    internal const string TemplateRenderTypesCacheKey = "template:setup:render-types";

    private static readonly CacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12)
    };

    public Task<IReadOnlyList<TemplateCategory>> ListTemplateCategoriesAsync(CancellationToken cancellationToken = default) =>
        cacheProvider.GetOrSetAsync<IReadOnlyList<TemplateCategory>>(
            TemplateCategoriesCacheKey,
            async token => (IReadOnlyList<TemplateCategory>)await unitOfWork.GetRepo<TemplateCategory>()
                .Query()
                .AsNoTracking()
                .OrderBy(category => category.Order)
                .ThenBy(category => category.DisplayName)
                .ToListAsync(token),
            CacheOptions,
            cancellationToken);

    public Task<IReadOnlyList<TemplateRenderTypeSetup>> ListTemplateRenderTypesAsync(CancellationToken cancellationToken = default) =>
        cacheProvider.GetOrSetAsync<IReadOnlyList<TemplateRenderTypeSetup>>(
            TemplateRenderTypesCacheKey,
            async token => (IReadOnlyList<TemplateRenderTypeSetup>)await unitOfWork.GetRepo<TemplateRenderTypeSetup>()
                .Query()
                .AsNoTracking()
                .OrderBy(renderType => renderType.Order)
                .ThenBy(renderType => renderType.Id)
                .ToListAsync(token),
            CacheOptions,
            cancellationToken);

}
