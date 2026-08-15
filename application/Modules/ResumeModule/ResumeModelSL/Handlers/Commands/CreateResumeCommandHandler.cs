using Mediator;
using ResumeEnhancer.ResumeModule.AM.Responses;
using ResumeEnhancer.ResumeModule.SL.Abstractions.Persistence;
using ResumeEnhancer.ResumeModule.SL.Contracts;

namespace ResumeEnhancer.ResumeModule.SL.Handlers;

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

