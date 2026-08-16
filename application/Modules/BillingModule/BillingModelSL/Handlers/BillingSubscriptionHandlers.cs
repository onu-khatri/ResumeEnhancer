using Mediator;
using ResumeEnhancer.BillingModule.AM.Responses;
using ResumeEnhancer.BillingModule.SL.Abstractions.Persistence;
using ResumeEnhancer.BillingModule.SL.Contracts;
using ResumeEnhancer.BillingModule.SL.Mapping;
using ResumeEnhancer.ResumeModule.SL.Integrations;

namespace ResumeEnhancer.BillingModule.SL.Handlers;

public sealed class CreateBillingSubscriptionCommandHandler : ICommandHandler<CreateBillingSubscriptionCommand, BillingSubscriptionDetailResponse?>
{
    private readonly IBillingRepository _repository;
    private readonly IResumeLookupService _resumeLookupService;

    public CreateBillingSubscriptionCommandHandler(IBillingRepository repository, IResumeLookupService resumeLookupService)
    {
        _repository = repository;
        _resumeLookupService = resumeLookupService;
    }

    public async ValueTask<BillingSubscriptionDetailResponse?> Handle(CreateBillingSubscriptionCommand request, CancellationToken cancellationToken)
    {
        if (request.Request.ResumeId is int resumeId && !await _resumeLookupService.ResumeExistsAsync(resumeId, cancellationToken))
        {
            return null;
        }

        var entity = BillingModelMapper.CreateBillingSubscription(request.Request);
        await _repository.AddBillingSubscriptionAsync(entity, request.AuditUserId, cancellationToken);
        return BillingModelMapper.MapBillingSubscriptionDetail(entity);
    }
}

public sealed class UpdateBillingSubscriptionCommandHandler : ICommandHandler<UpdateBillingSubscriptionCommand, BillingSubscriptionDetailResponse?>
{
    private readonly IBillingRepository _repository;
    private readonly IResumeLookupService _resumeLookupService;

    public UpdateBillingSubscriptionCommandHandler(IBillingRepository repository, IResumeLookupService resumeLookupService)
    {
        _repository = repository;
        _resumeLookupService = resumeLookupService;
    }

    public async ValueTask<BillingSubscriptionDetailResponse?> Handle(UpdateBillingSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetBillingSubscriptionAsync(request.BillingSubscriptionId, track: true, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        if (request.Request.ResumeId is int resumeId && !await _resumeLookupService.ResumeExistsAsync(resumeId, cancellationToken))
        {
            return null;
        }

        BillingModelMapper.Apply(request.Request, entity);
        await _repository.SaveAsync(request.AuditUserId, cancellationToken);
        return BillingModelMapper.MapBillingSubscriptionDetail(entity);
    }
}

public sealed class DeleteBillingSubscriptionCommandHandler : ICommandHandler<DeleteBillingSubscriptionCommand, bool>
{
    private readonly IBillingRepository _repository;

    public DeleteBillingSubscriptionCommandHandler(IBillingRepository repository) => _repository = repository;

    public async ValueTask<bool> Handle(DeleteBillingSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetBillingSubscriptionAsync(request.BillingSubscriptionId, track: true, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        await _repository.DeleteBillingSubscriptionAsync(entity, request.AuditUserId, cancellationToken);
        return true;
    }
}

public sealed class GetBillingSubscriptionQueryHandler : IQueryHandler<GetBillingSubscriptionQuery, BillingSubscriptionDetailResponse?>
{
    private readonly IBillingRepository _repository;

    public GetBillingSubscriptionQueryHandler(IBillingRepository repository) => _repository = repository;

    public async ValueTask<BillingSubscriptionDetailResponse?> Handle(GetBillingSubscriptionQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetBillingSubscriptionAsync(request.BillingSubscriptionId, false, cancellationToken);
        return entity is null ? null : BillingModelMapper.MapBillingSubscriptionDetail(entity);
    }
}

public sealed class ListBillingSubscriptionsQueryHandler : IQueryHandler<ListBillingSubscriptionsQuery, IReadOnlyList<BillingSubscriptionListItemResponse>>
{
    private readonly IBillingRepository _repository;

    public ListBillingSubscriptionsQueryHandler(IBillingRepository repository) => _repository = repository;

    public async ValueTask<IReadOnlyList<BillingSubscriptionListItemResponse>> Handle(ListBillingSubscriptionsQuery request, CancellationToken cancellationToken)
    {
        var entities = await _repository.ListBillingSubscriptionsAsync(cancellationToken);
        return entities.Select(BillingModelMapper.MapBillingSubscriptionListItem).ToArray();
    }
}
