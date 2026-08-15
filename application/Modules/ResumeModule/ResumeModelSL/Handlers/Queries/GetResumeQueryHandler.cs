using Mediator;
using ResumeEnhancer.ResumeModule.AM.Responses;
using ResumeEnhancer.ResumeModule.SL.Abstractions.Persistence;
using ResumeEnhancer.ResumeModule.SL.Contracts;

namespace ResumeEnhancer.ResumeModule.SL.Handlers;

public sealed class GetResumeQueryHandler
    : IQueryHandler<GetResumeQuery, ResumeDetailResponse?>
{
    private readonly IResumeRepository _resumeRepository;

    public GetResumeQueryHandler(IResumeRepository resumeRepository)
    {
        _resumeRepository = resumeRepository;
    }

    public async ValueTask<ResumeDetailResponse?> Handle(
        GetResumeQuery request,
        CancellationToken cancellationToken = default)
    {
        var resume = await _resumeRepository.GetAsync(
            request.ResumeId,
            request.UserId,
            track: false,
            cancellationToken);

        return resume is null
            ? null
            : ResumeModelMapper.MapDetail(resume);
    }
}

