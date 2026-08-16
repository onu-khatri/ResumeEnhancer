using ResumeEnhancer.ResumeModule.DM.Entities;

namespace ResumeEnhancer.ResumeModule.SL.Abstractions.Persistence;

public interface IResumeSetupDataRepository
{
    Task<IReadOnlyList<ResumeSectionSetup>> ListResumeSectionsAsync(CancellationToken cancellationToken = default);
}
