namespace ResumeEnhancer.ResumeModule.SL.Integrations;

public interface IResumeLookupService
{
    Task<bool> ResumeExistsAsync(int resumeId, CancellationToken cancellationToken = default);
}
