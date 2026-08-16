using ResumeEnhancer.TemplateModule.DM.Entities;

namespace ResumeEnhancer.TemplateModule.SL.Abstractions.Persistence;

public interface ITemplateRepository
{
    Task<TemplateCategory> AddTemplateCategoryAsync(TemplateCategory category, int? auditUserId, CancellationToken cancellationToken = default);
    Task<TemplateCategory?> GetTemplateCategoryAsync(int templateCategoryId, bool track = false, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TemplateCategory>> ListTemplateCategoriesAsync(CancellationToken cancellationToken = default);
    Task DeleteTemplateCategoryAsync(TemplateCategory category, int? auditUserId, CancellationToken cancellationToken = default);

    Task<Template> AddTemplateAsync(Template template, int? auditUserId, CancellationToken cancellationToken = default);
    Task<Template?> GetTemplateAsync(int templateId, bool track = false, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Template>> ListTemplatesAsync(CancellationToken cancellationToken = default);
    Task DeleteTemplateAsync(Template template, int? auditUserId, CancellationToken cancellationToken = default);
    Task<bool> TemplateExistsAsync(int templateId, CancellationToken cancellationToken = default);
    Task<bool> TemplateCategoryExistsAsync(int templateCategoryId, CancellationToken cancellationToken = default);

    Task SaveAsync(int? auditUserId, CancellationToken cancellationToken = default);
}
