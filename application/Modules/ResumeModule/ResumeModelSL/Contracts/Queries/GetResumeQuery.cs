using Mediator;
using ResumeEnhancer.ResumeModule.AM.Responses;

namespace ResumeEnhancer.ResumeModule.SL.Contracts;

public sealed record GetResumeQuery(
    int ResumeId,
    int? UserId = null) : IQuery<ResumeDetailResponse?>;

