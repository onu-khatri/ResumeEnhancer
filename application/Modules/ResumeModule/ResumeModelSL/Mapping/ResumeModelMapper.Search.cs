using Mapster;
using ResumeModuleAM.Requests;
using ResumeModuleAM.Responses;
using ResumeModulePL.Contracts;

namespace ResumeModuleSL.Handlers;

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
