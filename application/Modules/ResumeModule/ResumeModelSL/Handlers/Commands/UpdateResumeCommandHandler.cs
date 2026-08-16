using Mediator;
using ResumeEnhancer.ProfilingModule.SL.Integrations;
using ResumeEnhancer.ResumeModule.AM.Responses;
using ResumeEnhancer.ResumeModule.SL.Abstractions.Persistence;
using ResumeEnhancer.ResumeModule.SL.Contracts;
using ResumeEnhancer.TemplateModule.SL.Integrations;

namespace ResumeEnhancer.ResumeModule.SL.Handlers;

public sealed class UpdateResumeCommandHandler
    : ICommandHandler<UpdateResumeCommand, ResumeDetailResponse>
{
    private readonly IResumeRepository _resumeRepository;
    private readonly IUserLookupService _userLookupService;
    private readonly ITemplateLookupService _templateLookupService;

    public UpdateResumeCommandHandler(
        IResumeRepository resumeRepository,
        IUserLookupService userLookupService,
        ITemplateLookupService templateLookupService)
    {
        _resumeRepository = resumeRepository;
        _userLookupService = userLookupService;
        _templateLookupService = templateLookupService;
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

        if (request.Request.UserId is int targetUserId
            && !await _userLookupService.UserExistsAsync(targetUserId, cancellationToken))
        {
            throw new InvalidOperationException($"User '{targetUserId}' was not found.");
        }

        if (request.Request.TemplateId is int templateId
            && !await _templateLookupService.TemplateExistsAsync(templateId, cancellationToken))
        {
            throw new InvalidOperationException($"Template '{templateId}' was not found.");
        }

        ResumeModelMapper.ApplyResumeUpdate(
            resume,
            request.Request,
            _resumeRepository.Remove);

        await _resumeRepository.SaveAsync(request.AuditUserId, cancellationToken);

        return ResumeModelMapper.MapDetail(resume);
    }
}

