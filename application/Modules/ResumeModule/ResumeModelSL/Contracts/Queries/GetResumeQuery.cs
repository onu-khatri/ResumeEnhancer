using Mediator;
using ResumeEnhancer.ResumeModule.AM.Responses;

namespace ResumeEnhancer.ResumeModule.SL.Contracts;

public sealed record GetResumeQuery(
    int ResumeId,
    string? UserId = null) : IQuery<ResumeDetailResponse?>;

