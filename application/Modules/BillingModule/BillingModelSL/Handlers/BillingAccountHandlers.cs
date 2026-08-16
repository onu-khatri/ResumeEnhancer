using Mediator;
using ResumeEnhancer.BillingModule.AM.Responses;
using ResumeEnhancer.BillingModule.SL.Abstractions.Persistence;
using ResumeEnhancer.BillingModule.SL.Contracts;
using ResumeEnhancer.BillingModule.SL.Mapping;
using ResumeEnhancer.ProfilingModule.SL.Integrations;

namespace ResumeEnhancer.BillingModule.SL.Handlers;

public sealed class CreateBillingAccountCommandHandler : ICommandHandler<CreateBillingAccountCommand, BillingAccountDetailResponse>
{
    private readonly IBillingRepository _repository;
    private readonly IUserLookupService _userLookupService;

    public CreateBillingAccountCommandHandler(IBillingRepository repository, IUserLookupService userLookupService)
    {
        _repository = repository;
        _userLookupService = userLookupService;
    }

    public async ValueTask<BillingAccountDetailResponse> Handle(CreateBillingAccountCommand request, CancellationToken cancellationToken)
    {
        if (!await _userLookupService.UserExistsAsync(request.Request.UserId, cancellationToken))
        {
            return new BillingAccountDetailResponse();
        }

        var entity = BillingModelMapper.CreateBillingAccount(request.Request);
        await _repository.AddBillingAccountAsync(entity, request.AuditUserId, cancellationToken);
        return BillingModelMapper.MapBillingAccountDetail(entity);
    }
}

public sealed class UpdateBillingAccountCommandHandler : ICommandHandler<UpdateBillingAccountCommand, BillingAccountDetailResponse?>
{
    private readonly IBillingRepository _repository;
    private readonly IUserLookupService _userLookupService;

    public UpdateBillingAccountCommandHandler(IBillingRepository repository, IUserLookupService userLookupService)
    {
        _repository = repository;
        _userLookupService = userLookupService;
    }

    public async ValueTask<BillingAccountDetailResponse?> Handle(UpdateBillingAccountCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetBillingAccountAsync(request.BillingAccountId, track: true, cancellationToken);
        if (entity is null || !await _userLookupService.UserExistsAsync(request.Request.UserId, cancellationToken))
        {
            return null;
        }

        BillingModelMapper.Apply(request.Request, entity);
        await _repository.SaveAsync(request.AuditUserId, cancellationToken);
        return BillingModelMapper.MapBillingAccountDetail(entity);
    }
}

public sealed class DeleteBillingAccountCommandHandler : ICommandHandler<DeleteBillingAccountCommand, bool>
{
    private readonly IBillingRepository _repository;

    public DeleteBillingAccountCommandHandler(IBillingRepository repository) => _repository = repository;

    public async ValueTask<bool> Handle(DeleteBillingAccountCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetBillingAccountAsync(request.BillingAccountId, track: true, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        await _repository.DeleteBillingAccountAsync(entity, request.AuditUserId, cancellationToken);
        return true;
    }
}

public sealed class GetBillingAccountQueryHandler : IQueryHandler<GetBillingAccountQuery, BillingAccountDetailResponse?>
{
    private readonly IBillingRepository _repository;

    public GetBillingAccountQueryHandler(IBillingRepository repository) => _repository = repository;

    public async ValueTask<BillingAccountDetailResponse?> Handle(GetBillingAccountQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetBillingAccountAsync(request.BillingAccountId, false, cancellationToken);
        return entity is null ? null : BillingModelMapper.MapBillingAccountDetail(entity);
    }
}

public sealed class ListBillingAccountsQueryHandler : IQueryHandler<ListBillingAccountsQuery, IReadOnlyList<BillingAccountListItemResponse>>
{
    private readonly IBillingRepository _repository;

    public ListBillingAccountsQueryHandler(IBillingRepository repository) => _repository = repository;

    public async ValueTask<IReadOnlyList<BillingAccountListItemResponse>> Handle(ListBillingAccountsQuery request, CancellationToken cancellationToken)
    {
        var entities = await _repository.ListBillingAccountsAsync(cancellationToken);
        return entities.Select(BillingModelMapper.MapBillingAccountListItem).ToArray();
    }
}
