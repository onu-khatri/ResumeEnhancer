using Mediator;
using ResumeModuleAM.Responses;

namespace ResumeModuleSL.Contracts;

public sealed record GetResumeQuery(
    int ResumeId,
    string? UserId = null) : IQuery<ResumeDetailResponse?>;
