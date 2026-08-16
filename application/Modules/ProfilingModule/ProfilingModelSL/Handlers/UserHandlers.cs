using Mediator;
using ResumeEnhancer.ProfilingModule.AM.Responses;
using ResumeEnhancer.ProfilingModule.DM.Enums;
using ResumeEnhancer.ProfilingModule.SL.Abstractions.Persistence;
using ResumeEnhancer.ProfilingModule.SL.Contracts;
using ResumeEnhancer.ProfilingModule.SL.Mapping;

namespace ResumeEnhancer.ProfilingModule.SL.Handlers;

public sealed class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, UserDetailResponse>
{
    private readonly IProfilingRepository _repository;
    private readonly IProfilingSetupDataRepository _setupDataRepository;

    public CreateUserCommandHandler(IProfilingRepository repository, IProfilingSetupDataRepository setupDataRepository)
    {
        _repository = repository;
        _setupDataRepository = setupDataRepository;
    }

    public async ValueTask<UserDetailResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var addressTypes = await _setupDataRepository.ListUserAddressTypesAsync(cancellationToken);
        var billingAddressType = addressTypes.SingleOrDefault(item =>
                string.Equals(item.Code, nameof(UserAddressType.Billing), StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException("Billing address type setup was not found.");
        var communicationAddressType = addressTypes.SingleOrDefault(item =>
                string.Equals(item.Code, nameof(UserAddressType.Communication), StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException("Communication address type setup was not found.");

        var entity = ProfilingModelMapper.CreateUser(request.Request, billingAddressType, communicationAddressType);
        await _repository.AddUserAsync(entity, request.AuditUserId, cancellationToken);
        await _repository.SyncUserAccessProfilesAsync(entity, request.Request.AccessProfileIds, cancellationToken);
        await _repository.SaveAsync(request.AuditUserId, cancellationToken);
        return ProfilingModelMapper.MapUserDetail(entity);
    }
}

public sealed class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, UserDetailResponse?>
{
    private readonly IProfilingRepository _repository;
    private readonly IProfilingSetupDataRepository _setupDataRepository;

    public UpdateUserCommandHandler(IProfilingRepository repository, IProfilingSetupDataRepository setupDataRepository)
    {
        _repository = repository;
        _setupDataRepository = setupDataRepository;
    }

    public async ValueTask<UserDetailResponse?> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetUserAsync(request.UserId, track: true, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        var addressTypes = await _setupDataRepository.ListUserAddressTypesAsync(cancellationToken);
        var billingAddressType = addressTypes.SingleOrDefault(item =>
                string.Equals(item.Code, nameof(UserAddressType.Billing), StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException("Billing address type setup was not found.");
        var communicationAddressType = addressTypes.SingleOrDefault(item =>
                string.Equals(item.Code, nameof(UserAddressType.Communication), StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException("Communication address type setup was not found.");

        ProfilingModelMapper.Apply(request.Request, entity, billingAddressType, communicationAddressType);
        await _repository.SyncUserAccessProfilesAsync(entity, request.Request.AccessProfileIds, cancellationToken);
        await _repository.SaveAsync(request.AuditUserId, cancellationToken);
        return ProfilingModelMapper.MapUserDetail(entity);
    }
}

public sealed class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand, bool>
{
    private readonly IProfilingRepository _repository;

    public DeleteUserCommandHandler(IProfilingRepository repository) => _repository = repository;

    public async ValueTask<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetUserAsync(request.UserId, track: true, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        await _repository.DeleteUserAsync(entity, request.AuditUserId, cancellationToken);
        return true;
    }
}

public sealed class GetUserQueryHandler : IQueryHandler<GetUserQuery, UserDetailResponse?>
{
    private readonly IProfilingRepository _repository;

    public GetUserQueryHandler(IProfilingRepository repository) => _repository = repository;

    public async ValueTask<UserDetailResponse?> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetUserAsync(request.UserId, false, cancellationToken);
        return entity is null ? null : ProfilingModelMapper.MapUserDetail(entity);
    }
}

public sealed class ListUsersQueryHandler : IQueryHandler<ListUsersQuery, IReadOnlyList<UserListItemResponse>>
{
    private readonly IProfilingRepository _repository;

    public ListUsersQueryHandler(IProfilingRepository repository) => _repository = repository;

    public async ValueTask<IReadOnlyList<UserListItemResponse>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
    {
        var entities = await _repository.ListUsersAsync(cancellationToken);
        return entities.Select(ProfilingModelMapper.MapUserListItem).ToArray();
    }
}
