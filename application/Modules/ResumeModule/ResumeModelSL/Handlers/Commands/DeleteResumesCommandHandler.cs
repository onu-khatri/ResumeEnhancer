using Mediator;
using ResumeEnhancer.ResumeModule.AM.Responses;
using ResumeEnhancer.ResumeModule.SL.Abstractions.Persistence;
using ResumeEnhancer.ResumeModule.SL.Contracts;

namespace ResumeEnhancer.ResumeModule.SL.Handlers;

public sealed class DeleteResumesCommandHandler
    : ICommandHandler<DeleteResumesCommand, ResumeDeleteResponse>
{
    private readonly IResumeRepository _resumeRepository;

    public DeleteResumesCommandHandler(IResumeRepository resumeRepository)
    {
        _resumeRepository = resumeRepository;
    }

    public async ValueTask<ResumeDeleteResponse> Handle(
        DeleteResumesCommand request,
        CancellationToken cancellationToken = default)
    {
        var result = await _resumeRepository.DeleteAsync(
            request.ResumeIds,
            request.AuditUserId,
            request.UserId,
            cancellationToken);

        return ResumeModelMapper.MapDelete(result);
    }
}

