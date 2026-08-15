using Mediator;
using ResumeEnhancer.ResumeModule.AM.Responses;
using ResumeEnhancer.ResumeModule.SL.Abstractions.Persistence;
using ResumeEnhancer.ResumeModule.SL.Contracts;

namespace ResumeEnhancer.ResumeModule.SL.Handlers;

public sealed class SearchResumesQueryHandler
    : IQueryHandler<SearchResumesQuery, ResumeSearchResponse>
{
    private readonly IResumeRepository _resumeRepository;

    public SearchResumesQueryHandler(IResumeRepository resumeRepository)
    {
        _resumeRepository = resumeRepository;
    }

    public async ValueTask<ResumeSearchResponse> Handle(
        SearchResumesQuery request,
        CancellationToken cancellationToken = default)
    {
        var searchResult = await _resumeRepository.SearchAsync(
            ResumeModelMapper.ToCriteria(request.Request),
            cancellationToken);

        return ResumeModelMapper.MapSearch(searchResult);
    }
}

