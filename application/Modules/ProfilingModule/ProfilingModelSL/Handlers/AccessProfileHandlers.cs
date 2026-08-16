using Mediator;
using ResumeEnhancer.ProfilingModule.AM.Responses;
using ResumeEnhancer.ProfilingModule.DM.Entities;
using ResumeEnhancer.ProfilingModule.SL.Abstractions.Persistence;
using ResumeEnhancer.ProfilingModule.SL.Contracts;
using ResumeEnhancer.ProfilingModule.SL.Mapping;

namespace ResumeEnhancer.ProfilingModule.SL.Handlers;

public sealed class CreateAccessProfileCommandHandler : ICommandHandler<CreateAccessProfileCommand, AccessProfileDetailResponse>
{
    private readonly IProfilingRepository _repository;

    public CreateAccessProfileCommandHandler(IProfilingRepository repository) => _repository = repository;

    public async ValueTask<AccessProfileDetailResponse> Handle(CreateAccessProfileCommand request, CancellationToken cancellationToken)
    {
        var entity = ProfilingModelMapper.CreateAccessProfile(request.Request);
        await _repository.AddAccessProfileAsync(entity, request.AuditUserId, cancellationToken);
        await _repository.SyncAccessProfileRolesAsync(entity, request.Request.RoleIds, cancellationToken);
        await _repository.SaveAsync(request.AuditUserId, cancellationToken);
        return ProfilingModelMapper.MapAccessProfileDetail(entity);
    }
}

public sealed class UpdateAccessProfileCommandHandler : ICommandHandler<UpdateAccessProfileCommand, AccessProfileDetailResponse?>
{
    private readonly IProfilingRepository _repository;

    public UpdateAccessProfileCommandHandler(IProfilingRepository repository) => _repository = repository;

    public async ValueTask<AccessProfileDetailResponse?> Handle(UpdateAccessProfileCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetAccessProfileAsync(request.AccessProfileId, track: true, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        ProfilingModelMapper.Apply(request.Request, entity);
        await _repository.SyncAccessProfileRolesAsync(entity, request.Request.RoleIds, cancellationToken);
        await _repository.SaveAsync(request.AuditUserId, cancellationToken);
        return ProfilingModelMapper.MapAccessProfileDetail(entity);
    }
}

public sealed class DeleteAccessProfileCommandHandler : ICommandHandler<DeleteAccessProfileCommand, bool>
{
    private readonly IProfilingRepository _repository;

    public DeleteAccessProfileCommandHandler(IProfilingRepository repository) => _repository = repository;

    public async ValueTask<bool> Handle(DeleteAccessProfileCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetAccessProfileAsync(request.AccessProfileId, track: true, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        await _repository.DeleteAccessProfileAsync(entity, request.AuditUserId, cancellationToken);
        return true;
    }
}

public sealed class GetAccessProfileQueryHandler : IQueryHandler<GetAccessProfileQuery, AccessProfileDetailResponse?>
{
    private readonly IProfilingRepository _repository;

    public GetAccessProfileQueryHandler(IProfilingRepository repository) => _repository = repository;

    public async ValueTask<AccessProfileDetailResponse?> Handle(GetAccessProfileQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetAccessProfileAsync(request.AccessProfileId, false, cancellationToken);
        return entity is null ? null : ProfilingModelMapper.MapAccessProfileDetail(entity);
    }
}

public sealed class ListAccessProfilesQueryHandler : IQueryHandler<ListAccessProfilesQuery, IReadOnlyList<AccessProfileListItemResponse>>
{
    private readonly IProfilingRepository _repository;

    public ListAccessProfilesQueryHandler(IProfilingRepository repository) => _repository = repository;

    public async ValueTask<IReadOnlyList<AccessProfileListItemResponse>> Handle(ListAccessProfilesQuery request, CancellationToken cancellationToken)
    {
        var entities = await _repository.ListAccessProfilesAsync(cancellationToken);
        return entities.Select(ProfilingModelMapper.MapAccessProfileListItem).ToArray();
    }
}
