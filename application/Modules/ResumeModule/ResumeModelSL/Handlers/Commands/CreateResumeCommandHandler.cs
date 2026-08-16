using Mediator;
using ResumeEnhancer.ProfilingModule.SL.Integrations;
using ResumeEnhancer.ResumeModule.AM.Responses;
using ResumeEnhancer.ResumeModule.SL.Abstractions.Persistence;
using ResumeEnhancer.ResumeModule.SL.Contracts;
using ResumeEnhancer.TemplateModule.SL.Integrations;

namespace ResumeEnhancer.ResumeModule.SL.Handlers;

public sealed class CreateResumeCommandHandler
    : ICommandHandler<CreateResumeCommand, ResumeDetailResponse>
{
    private readonly IResumeRepository _resumeRepository;
    private readonly IUserLookupService _userLookupService;
    private readonly ITemplateLookupService _templateLookupService;

    public CreateResumeCommandHandler(
        IResumeRepository resumeRepository,
        IUserLookupService userLookupService,
        ITemplateLookupService templateLookupService)
    {
        _resumeRepository = resumeRepository;
        _userLookupService = userLookupService;
        _templateLookupService = templateLookupService;
    }

    public async ValueTask<ResumeDetailResponse> Handle(
        CreateResumeCommand request,
        CancellationToken cancellationToken = default)
    {
        if (!await _userLookupService.UserExistsAsync(request.Request.UserId, cancellationToken))
        {
            throw new InvalidOperationException($"User '{request.Request.UserId}' was not found.");
        }

        if (request.Request.TemplateId is int templateId
            && !await _templateLookupService.TemplateExistsAsync(templateId, cancellationToken))
        {
            throw new InvalidOperationException($"Template '{templateId}' was not found.");
        }

        var resume = ResumeModelMapper.CreateResume(request.Request);
        var savedResume = await _resumeRepository.AddAsync(
            resume,
            request.AuditUserId,
            cancellationToken);

        return ResumeModelMapper.MapDetail(savedResume);
    }
}

