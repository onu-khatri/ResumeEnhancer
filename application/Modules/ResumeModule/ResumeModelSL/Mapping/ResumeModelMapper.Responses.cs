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

    public static void EnsureUserAccess(Resume resume, string? userId)
    {
        var normalizedUserId = NormalizeOptional(userId);

        if (normalizedUserId is not null && resume.UserId != normalizedUserId)
        {
            throw new UnauthorizedAccessException(
                $"Resume '{resume.Id}' does not belong to user '{normalizedUserId}'.");
        }
    }

}

