using Mediator;
using ResumeEnhancer.BillingModule.AM.Responses;
using ResumeEnhancer.BillingModule.SL.Abstractions.Persistence;
using ResumeEnhancer.BillingModule.SL.Contracts;
using ResumeEnhancer.BillingModule.SL.Mapping;

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

public sealed class UpdateBillingPlanCommandHandler : ICommandHandler<UpdateBillingPlanCommand, BillingPlanDetailResponse?>
{
    private readonly IBillingRepository _repository;

    public UpdateBillingPlanCommandHandler(IBillingRepository repository) => _repository = repository;

    public async ValueTask<BillingPlanDetailResponse?> Handle(UpdateBillingPlanCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetBillingPlanAsync(request.BillingPlanId, track: true, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        BillingModelMapper.Apply(request.Request, entity);
        await _repository.SaveAsync(request.AuditUserId, cancellationToken);
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
