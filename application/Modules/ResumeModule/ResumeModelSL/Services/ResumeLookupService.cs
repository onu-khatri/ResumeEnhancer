using ResumeEnhancer.ResumeModule.SL.Abstractions.Persistence;
using ResumeEnhancer.ResumeModule.SL.Integrations;

namespace ResumeEnhancer.ResumeModule.SL.Services;

internal sealed class ResumeLookupService : IResumeLookupService
{
    private readonly IResumeRepository _resumeRepository;

    public ResumeLookupService(IResumeRepository resumeRepository)
    {
        _resumeRepository = resumeRepository;
    }

    public Task<bool> ResumeExistsAsync(int resumeId, CancellationToken cancellationToken = default) =>
        _resumeRepository.ExistsAsync(resumeId, cancellationToken: cancellationToken);
}
