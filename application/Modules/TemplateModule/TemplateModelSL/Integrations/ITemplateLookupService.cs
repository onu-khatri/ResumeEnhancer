namespace ResumeEnhancer.TemplateModule.SL.Integrations;

public interface ITemplateLookupService
{
    Task<bool> TemplateExistsAsync(int templateId, CancellationToken cancellationToken = default);
}
