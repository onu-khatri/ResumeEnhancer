using Mediator;
using ResumeEnhancer.ResumeModule.AM.Responses;
using ResumeEnhancer.ResumeModule.SL.Abstractions.Persistence;
using ResumeEnhancer.ResumeModule.SL.Contracts;

namespace ResumeEnhancer.ResumeModule.SL.Handlers;

public sealed class DeleteResumeCommandHandler
    : ICommandHandler<DeleteResumeCommand, ResumeDeleteResponse>
{
    private readonly IResumeRepository _resumeRepository;

    public DeleteResumeCommandHandler(IResumeRepository resumeRepository)
    {
        _resumeRepository = resumeRepository;
    }

    public async ValueTask<ResumeDeleteResponse> Handle(
        DeleteResumeCommand request,
        CancellationToken cancellationToken = default)
    {
        var result = await _resumeRepository.DeleteAsync(
            [request.ResumeId],
            request.AuditUserId,
            request.UserId,
            cancellationToken);

        return ResumeModelMapper.MapDelete(result);
    }
}

