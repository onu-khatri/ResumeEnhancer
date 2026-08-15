using Mediator;
using ResumeEnhancer.ResumeModule.SL.Abstractions.Persistence;
using ResumeEnhancer.ResumeModule.SL.Contracts;

namespace ResumeEnhancer.ResumeModule.SL.Handlers;

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

