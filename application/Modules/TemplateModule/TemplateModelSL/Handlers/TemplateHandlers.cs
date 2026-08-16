using Mediator;
using ResumeEnhancer.TemplateModule.AM.Responses;
using ResumeEnhancer.TemplateModule.SL.Abstractions.Persistence;
using ResumeEnhancer.TemplateModule.SL.Contracts;
using ResumeEnhancer.TemplateModule.SL.Mapping;

namespace ResumeEnhancer.TemplateModule.SL.Handlers;

public sealed class CreateTemplateCategoryCommandHandler : ICommandHandler<CreateTemplateCategoryCommand, TemplateCategoryDetailResponse>
{
    private readonly ITemplateRepository _repository;

    public CreateTemplateCategoryCommandHandler(ITemplateRepository repository) => _repository = repository;

    public async ValueTask<TemplateCategoryDetailResponse> Handle(CreateTemplateCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = TemplateModelMapper.CreateTemplateCategory(request.Request);
        await _repository.AddTemplateCategoryAsync(entity, request.AuditUserId, cancellationToken);
        return TemplateModelMapper.MapTemplateCategoryDetail(entity);
    }
}

public sealed class UpdateTemplateCategoryCommandHandler : ICommandHandler<UpdateTemplateCategoryCommand, TemplateCategoryDetailResponse?>
{
    private readonly ITemplateRepository _repository;

    public UpdateTemplateCategoryCommandHandler(ITemplateRepository repository) => _repository = repository;

    public async ValueTask<TemplateCategoryDetailResponse?> Handle(UpdateTemplateCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetTemplateCategoryAsync(request.TemplateCategoryId, track: true, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        TemplateModelMapper.Apply(request.Request, entity);
        await _repository.SaveAsync(request.AuditUserId, cancellationToken);
        return TemplateModelMapper.MapTemplateCategoryDetail(entity);
    }
}

public sealed class DeleteTemplateCategoryCommandHandler : ICommandHandler<DeleteTemplateCategoryCommand, bool>
{
    private readonly ITemplateRepository _repository;

    public DeleteTemplateCategoryCommandHandler(ITemplateRepository repository) => _repository = repository;

    public async ValueTask<bool> Handle(DeleteTemplateCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetTemplateCategoryAsync(request.TemplateCategoryId, track: true, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        await _repository.DeleteTemplateCategoryAsync(entity, request.AuditUserId, cancellationToken);
        return true;
    }
}

public sealed class GetTemplateCategoryQueryHandler : IQueryHandler<GetTemplateCategoryQuery, TemplateCategoryDetailResponse?>
{
    private readonly ITemplateRepository _repository;

    public GetTemplateCategoryQueryHandler(ITemplateRepository repository) => _repository = repository;

    public async ValueTask<TemplateCategoryDetailResponse?> Handle(GetTemplateCategoryQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetTemplateCategoryAsync(request.TemplateCategoryId, false, cancellationToken);
        return entity is null ? null : TemplateModelMapper.MapTemplateCategoryDetail(entity);
    }
}

public sealed class ListTemplateCategoriesQueryHandler : IQueryHandler<ListTemplateCategoriesQuery, IReadOnlyList<TemplateCategoryListItemResponse>>
{
    private readonly ITemplateRepository _repository;

    public ListTemplateCategoriesQueryHandler(ITemplateRepository repository) => _repository = repository;

    public async ValueTask<IReadOnlyList<TemplateCategoryListItemResponse>> Handle(ListTemplateCategoriesQuery request, CancellationToken cancellationToken)
    {
        var entities = await _repository.ListTemplateCategoriesAsync(cancellationToken);
        return entities.Select(TemplateModelMapper.MapTemplateCategoryListItem).ToArray();
    }
}

public sealed class CreateTemplateCommandHandler : ICommandHandler<CreateTemplateCommand, TemplateDetailResponse?>
{
    private readonly ITemplateRepository _repository;
    private readonly ITemplateSetupDataRepository _setupDataRepository;

    public CreateTemplateCommandHandler(ITemplateRepository repository, ITemplateSetupDataRepository setupDataRepository)
    {
        _repository = repository;
        _setupDataRepository = setupDataRepository;
    }

    public async ValueTask<TemplateDetailResponse?> Handle(CreateTemplateCommand request, CancellationToken cancellationToken)
    {
        if (!await _repository.TemplateCategoryExistsAsync(request.Request.TemplateCategoryId, cancellationToken))
        {
            return null;
        }

        var renderTypes = await _setupDataRepository.ListTemplateRenderTypesAsync(cancellationToken);
        var renderType = renderTypes.SingleOrDefault(item =>
            string.Equals(item.Code, request.Request.RenderTypeCode, StringComparison.OrdinalIgnoreCase));
        if (renderType is null)
        {
            return null;
        }

        var entity = TemplateModelMapper.CreateTemplate(request.Request, renderType);
        await _repository.AddTemplateAsync(entity, request.AuditUserId, cancellationToken);
        return TemplateModelMapper.MapTemplateDetail(entity);
    }
}

public sealed class UpdateTemplateCommandHandler : ICommandHandler<UpdateTemplateCommand, TemplateDetailResponse?>
{
    private readonly ITemplateRepository _repository;
    private readonly ITemplateSetupDataRepository _setupDataRepository;

    public UpdateTemplateCommandHandler(ITemplateRepository repository, ITemplateSetupDataRepository setupDataRepository)
    {
        _repository = repository;
        _setupDataRepository = setupDataRepository;
    }

    public async ValueTask<TemplateDetailResponse?> Handle(UpdateTemplateCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetTemplateAsync(request.TemplateId, track: true, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        if (!await _repository.TemplateCategoryExistsAsync(request.Request.TemplateCategoryId, cancellationToken))
        {
            return null;
        }

        var renderTypes = await _setupDataRepository.ListTemplateRenderTypesAsync(cancellationToken);
        var renderType = renderTypes.SingleOrDefault(item =>
            string.Equals(item.Code, request.Request.RenderTypeCode, StringComparison.OrdinalIgnoreCase));
        if (renderType is null)
        {
            return null;
        }

        TemplateModelMapper.Apply(request.Request, entity, renderType);
        await _repository.SaveAsync(request.AuditUserId, cancellationToken);
        return TemplateModelMapper.MapTemplateDetail(entity);
    }
}

public sealed class DeleteTemplateCommandHandler : ICommandHandler<DeleteTemplateCommand, bool>
{
    private readonly ITemplateRepository _repository;

    public DeleteTemplateCommandHandler(ITemplateRepository repository) => _repository = repository;

    public async ValueTask<bool> Handle(DeleteTemplateCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetTemplateAsync(request.TemplateId, track: true, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        await _repository.DeleteTemplateAsync(entity, request.AuditUserId, cancellationToken);
        return true;
    }
}

public sealed class GetTemplateQueryHandler : IQueryHandler<GetTemplateQuery, TemplateDetailResponse?>
{
    private readonly ITemplateRepository _repository;

    public GetTemplateQueryHandler(ITemplateRepository repository) => _repository = repository;

    public async ValueTask<TemplateDetailResponse?> Handle(GetTemplateQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetTemplateAsync(request.TemplateId, false, cancellationToken);
        return entity is null ? null : TemplateModelMapper.MapTemplateDetail(entity);
    }
}

public sealed class ListTemplatesQueryHandler : IQueryHandler<ListTemplatesQuery, IReadOnlyList<TemplateListItemResponse>>
{
    private readonly ITemplateRepository _repository;

    public ListTemplatesQueryHandler(ITemplateRepository repository) => _repository = repository;

    public async ValueTask<IReadOnlyList<TemplateListItemResponse>> Handle(ListTemplatesQuery request, CancellationToken cancellationToken)
    {
        var entities = await _repository.ListTemplatesAsync(cancellationToken);
        return entities.Select(TemplateModelMapper.MapTemplateListItem).ToArray();
    }
}
