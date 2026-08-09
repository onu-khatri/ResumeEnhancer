using Mediator;
using ResumeModuleSL.Abstractions.Persistence;
using ResumeModuleSL.Contracts;

namespace ResumeModuleSL.Handlers;

public sealed class ResumeExistsQueryHandler
    : IQueryHandler<ResumeExistsQuery, bool>
{
    private readonly IResumeRepository _resumeRepository;

    public ResumeExistsQueryHandler(IResumeRepository resumeRepository)
    {
        _resumeRepository = resumeRepository;
    }

    public async ValueTask<bool> Handle(
        ResumeExistsQuery request,
        CancellationToken cancellationToken = default) =>
        await _resumeRepository.ExistsAsync(
            request.ResumeId,
            request.UserId,
            cancellationToken);
}
