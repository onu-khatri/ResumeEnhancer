using Mediator;
using ResumeEnhancer.ProfilingModule.AM.Responses;
using ResumeEnhancer.ProfilingModule.SL.Abstractions.Persistence;
using ResumeEnhancer.ProfilingModule.SL.Contracts;
using ResumeEnhancer.ProfilingModule.SL.Mapping;

namespace ResumeEnhancer.ProfilingModule.SL.Handlers;

public sealed class CreateRoleCommandHandler : ICommandHandler<CreateRoleCommand, RoleDetailResponse>
{
    private readonly IProfilingRepository _repository;

    public CreateRoleCommandHandler(IProfilingRepository repository) => _repository = repository;

    public async ValueTask<RoleDetailResponse> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var entity = ProfilingModelMapper.CreateRole(request.Request);
        await _repository.AddRoleAsync(entity, request.AuditUserId, cancellationToken);
        return ProfilingModelMapper.MapRoleDetail(entity);
    }
}

public sealed class UpdateRoleCommandHandler : ICommandHandler<UpdateRoleCommand, RoleDetailResponse?>
{
    private readonly IProfilingRepository _repository;

    public UpdateRoleCommandHandler(IProfilingRepository repository) => _repository = repository;

    public async ValueTask<RoleDetailResponse?> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetRoleAsync(request.RoleId, track: true, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        ProfilingModelMapper.Apply(request.Request, entity);
        await _repository.SaveAsync(request.AuditUserId, cancellationToken);
        return ProfilingModelMapper.MapRoleDetail(entity);
    }
}

public sealed class DeleteRoleCommandHandler : ICommandHandler<DeleteRoleCommand, bool>
{
    private readonly IProfilingRepository _repository;

    public DeleteRoleCommandHandler(IProfilingRepository repository) => _repository = repository;

    public async ValueTask<bool> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetRoleAsync(request.RoleId, track: true, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        await _repository.DeleteRoleAsync(entity, request.AuditUserId, cancellationToken);
        return true;
    }
}

public sealed class GetRoleQueryHandler : IQueryHandler<GetRoleQuery, RoleDetailResponse?>
{
    private readonly IProfilingRepository _repository;

    public GetRoleQueryHandler(IProfilingRepository repository) => _repository = repository;

    public async ValueTask<RoleDetailResponse?> Handle(GetRoleQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetRoleAsync(request.RoleId, false, cancellationToken);
        return entity is null ? null : ProfilingModelMapper.MapRoleDetail(entity);
    }
}

public sealed class ListRolesQueryHandler : IQueryHandler<ListRolesQuery, IReadOnlyList<RoleListItemResponse>>
{
    private readonly IProfilingRepository _repository;

    public ListRolesQueryHandler(IProfilingRepository repository) => _repository = repository;

    public async ValueTask<IReadOnlyList<RoleListItemResponse>> Handle(ListRolesQuery request, CancellationToken cancellationToken)
    {
        var entities = await _repository.ListRolesAsync(cancellationToken);
        return entities.Select(ProfilingModelMapper.MapRoleListItem).ToArray();
    }
}
