using Mediator;
using ResumeModuleAM.Responses;
using ResumeModulePL.Contracts;
using ResumeModuleSL.Contracts;

namespace ResumeModuleSL.Handlers;

public sealed class CreateResumeCommandHandler
    : ICommandHandler<CreateResumeCommand, ResumeDetailResponse>
{
    private readonly IResumeRepository _resumeRepository;

    public CreateResumeCommandHandler(IResumeRepository resumeRepository)
    {
        _resumeRepository = resumeRepository;
    }

    public async ValueTask<ResumeDetailResponse> Handle(
        CreateResumeCommand request,
        CancellationToken cancellationToken = default)
    {
        var resume = ResumeModelMapper.CreateResume(request.Request);
        var savedResume = await _resumeRepository.AddAsync(
            resume,
            request.AuditUserId,
            cancellationToken);

        return ResumeModelMapper.MapDetail(savedResume);
    }
}
