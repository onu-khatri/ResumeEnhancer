using ResumeEnhancer.TemplateModule.DM.Entities;

namespace ResumeEnhancer.TemplateModule.SL.Abstractions.Persistence;

public interface ITemplateSetupDataRepository
{
    Task<IReadOnlyList<TemplateCategory>> ListTemplateCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TemplateRenderTypeSetup>> ListTemplateRenderTypesAsync(CancellationToken cancellationToken = default);
}
