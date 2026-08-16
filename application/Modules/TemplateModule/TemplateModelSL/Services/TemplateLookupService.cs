using ResumeEnhancer.TemplateModule.SL.Abstractions.Persistence;
using ResumeEnhancer.TemplateModule.SL.Integrations;

namespace ResumeEnhancer.TemplateModule.SL.Services;

internal sealed class TemplateLookupService : ITemplateLookupService
{
    private readonly ITemplateRepository _repository;

    public TemplateLookupService(ITemplateRepository repository) => _repository = repository;

    public Task<bool> TemplateExistsAsync(int templateId, CancellationToken cancellationToken = default) =>
        _repository.TemplateExistsAsync(templateId, cancellationToken);
}
