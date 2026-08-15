using Mapster;
using ResumeEnhancer.ResumeModule.AM.Requests;
using ResumeEnhancer.ResumeModule.AM.Responses;
using ResumeEnhancer.ResumeModule.SL.Abstractions.Persistence;

namespace ResumeEnhancer.ResumeModule.SL.Handlers;

internal static partial class ResumeModelMapper
{
    public static ResumeSearchCriteria ToCriteria(ResumeSearchRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return request.Adapt<ResumeSearchCriteria>(MapsterConfig);
    }

    public static ResumeSearchResponse MapSearch(ResumeSearchResult result) =>
        result.Adapt<ResumeSearchResponse>(MapsterConfig);

    public static ResumeDeleteResponse MapDelete(ResumeDeleteResult result) =>
        result.Adapt<ResumeDeleteResponse>(MapsterConfig);
}

