using Mediator;
using ResumeEnhancer.ResumeModule.AM.Responses;
using ResumeEnhancer.ResumeModule.SL.Abstractions.Persistence;
using ResumeEnhancer.ResumeModule.SL.Contracts;

namespace ResumeEnhancer.ResumeModule.SL.Handlers;

public sealed class UpdateResumeCommandHandler
    : ICommandHandler<UpdateResumeCommand, ResumeDetailResponse>
{
    private readonly IResumeRepository _resumeRepository;

    public UpdateResumeCommandHandler(IResumeRepository resumeRepository)
    {
        _resumeRepository = resumeRepository;
    }

    public async ValueTask<ResumeDetailResponse> Handle(
        UpdateResumeCommand request,
        CancellationToken cancellationToken = default)
    {
        var resume = await _resumeRepository.GetAsync(
            request.ResumeId,
            userId: null,
            track: true,
            cancellationToken)
            ?? throw new KeyNotFoundException(
                $"Resume '{request.ResumeId}' was not found.");

        ResumeModelMapper.EnsureUserAccess(resume, request.UserId);
        ResumeModelMapper.ApplyResumeUpdate(
            resume,
            request.Request,
            _resumeRepository.Remove);

        await _resumeRepository.SaveAsync(request.AuditUserId, cancellationToken);

        return ResumeModelMapper.MapDetail(resume);
    }
}

