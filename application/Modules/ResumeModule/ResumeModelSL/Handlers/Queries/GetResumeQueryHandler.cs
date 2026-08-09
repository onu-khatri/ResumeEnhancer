using Mediator;
using ResumeModuleAM.Responses;
using ResumeModulePL.Contracts;
using ResumeModuleSL.Contracts;

namespace ResumeModuleSL.Handlers;

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
