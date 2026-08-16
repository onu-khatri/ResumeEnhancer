using Mapster;
using ResumeEnhancer.ResumeModule.AM.Responses;
using ResumeEnhancer.ResumeModule.DM.Entities;

namespace ResumeEnhancer.ResumeModule.SL.Handlers;

internal static partial class ResumeModelMapper
{
    public static ResumeDetailResponse MapDetail(Resume resume)
    {
        ArgumentNullException.ThrowIfNull(resume);

        return resume.Adapt<ResumeDetailResponse>(MapsterConfig);
    }

    public static void EnsureUserAccess(Resume resume, int? userId)
    {
        if (userId is not null && resume.UserId != userId.Value)
        {
            throw new UnauthorizedAccessException(
                $"Resume '{resume.Id}' does not belong to user '{userId.Value}'.");
        }
    }

}

