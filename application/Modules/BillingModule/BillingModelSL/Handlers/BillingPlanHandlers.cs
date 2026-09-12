using Mediator;
using ResumeEnhancer.BillingModule.AM.Responses;
using ResumeEnhancer.BillingModule.SL.Abstractions.Persistence;
using ResumeEnhancer.BillingModule.SL.Contracts;
using ResumeEnhancer.BillingModule.SL.Mapping;
using ResumeEnhancer.ProfilingModule.SL.Integrations;

namespace ResumeEnhancer.BillingModule.SL.Handlers;

public sealed class CreateBillingPlanCommandHandler : ICommandHandler<CreateBillingPlanCommand, BillingPlanDetailResponse>
{
    private readonly IBillingRepository _repository;

    public CreateBillingPlanCommandHandler(IBillingRepository repository) => _repository = repository;

    public async ValueTask<BillingPlanDetailResponse> Handle(CreateBillingPlanCommand request, CancellationToken cancellationToken)
    {
        var entity = BillingModelMapper.CreateBillingPlan(request.Request);
        await _repository.AddBillingPlanAsync(entity, request.AuditUserId, cancellationToken);
        return BillingModelMapper.MapBillingPlanDetail(entity);
    }
}

public sealed class UpdateBillingPlanCommandHandler(
    IBillingRepository repository,
    IProfilingRegistrationService profiling
) : ICommandHandler<UpdateBillingPlanCommand, BillingPlanDetailResponse?>
{
    public async ValueTask<BillingPlanDetailResponse?> Handle(UpdateBillingPlanCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetBillingPlanAsync(request.BillingPlanId, track: true, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        var previousAccessProfileId = entity.AccessProfileId;
        BillingModelMapper.Apply(request.Request, entity);
        await repository.SaveAsync(request.AuditUserId, cancellationToken);

        if (request.Request.CascadeExistingSubscriptions && previousAccessProfileId != entity.AccessProfileId)
        {
            var subscriptions = await repository.ListActiveSubscriptionsForPlanAsync(entity.Id, cancellationToken);
            await profiling.ReviseAccessProfilesAsync(
                subscriptions.Select(subscription => new AccessProfileRevisionInput(
                    subscription.UserId,
                    subscription.Id,
                    entity.Code,
                    entity.AccessProfileId
                )).ToArray(),
                cancellationToken);
        }
        return BillingModelMapper.MapBillingPlanDetail(entity);
    }
}

public sealed class DeleteBillingPlanCommandHandler : ICommandHandler<DeleteBillingPlanCommand, bool>
{
    private readonly IBillingRepository _repository;

    public DeleteBillingPlanCommandHandler(IBillingRepository repository) => _repository = repository;

    public async ValueTask<bool> Handle(DeleteBillingPlanCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetBillingPlanAsync(request.BillingPlanId, track: true, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        await _repository.DeleteBillingPlanAsync(entity, request.AuditUserId, cancellationToken);
        return true;
    }
}

public sealed class GetBillingPlanQueryHandler : IQueryHandler<GetBillingPlanQuery, BillingPlanDetailResponse?>
{
    private readonly IBillingRepository _repository;

    public GetBillingPlanQueryHandler(IBillingRepository repository) => _repository = repository;

    public async ValueTask<BillingPlanDetailResponse?> Handle(GetBillingPlanQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetBillingPlanAsync(request.BillingPlanId, false, cancellationToken);
        return entity is null ? null : BillingModelMapper.MapBillingPlanDetail(entity);
    }
}

public sealed class ListBillingPlansQueryHandler : IQueryHandler<ListBillingPlansQuery, IReadOnlyList<BillingPlanListItemResponse>>
{
    private readonly IBillingRepository _repository;

    public ListBillingPlansQueryHandler(IBillingRepository repository) => _repository = repository;

    public async ValueTask<IReadOnlyList<BillingPlanListItemResponse>> Handle(ListBillingPlansQuery request, CancellationToken cancellationToken)
    {
        var entities = await _repository.ListBillingPlansAsync(cancellationToken);
        return entities.Select(BillingModelMapper.MapBillingPlanListItem).ToArray();
    }
}
