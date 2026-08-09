using Mediator;
using ResumeModuleAM.Responses;
using ResumeModuleSL.Abstractions.Persistence;
using ResumeModuleSL.Contracts;

namespace ResumeModuleSL.Handlers;

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
