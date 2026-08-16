using Microsoft.EntityFrameworkCore;
using ResumeEnhancer.Infrastructure.Caching;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.ResumeModule.DM.Entities;
using ResumeEnhancer.ResumeModule.SL.Abstractions.Persistence;

namespace ResumeEnhancer.ResumeModule.PL.Repositories;

public sealed class ResumeSetupDataRepository(
    IUnitOfWork<AppDbContext> unitOfWork,
    ICacheProvider cacheProvider) : IResumeSetupDataRepository
{
    internal const string ResumeSectionsCacheKey = "resume:setup:sections";

    private static readonly CacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12)
    };

    public Task<IReadOnlyList<ResumeSectionSetup>> ListResumeSectionsAsync(CancellationToken cancellationToken = default) =>
        cacheProvider.GetOrSetAsync<IReadOnlyList<ResumeSectionSetup>>(
            ResumeSectionsCacheKey,
            async token => (IReadOnlyList<ResumeSectionSetup>)await unitOfWork.GetRepo<ResumeSectionSetup>()
                .Query()
                .AsNoTracking()
                .OrderBy(section => section.Order)
                .ThenBy(section => section.Id)
                .ToListAsync(token),
            CacheOptions,
            cancellationToken);
}
