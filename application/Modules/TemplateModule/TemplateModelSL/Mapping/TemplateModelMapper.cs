using ResumeEnhancer.TemplateModule.AM.Requests;
using ResumeEnhancer.TemplateModule.AM.Responses;
using ResumeEnhancer.TemplateModule.DM.Entities;

namespace ResumeEnhancer.TemplateModule.SL.Mapping;

internal static class TemplateModelMapper
{
    public static Template CreateTemplate(CreateTemplateRequest request, TemplateRenderTypeSetup renderType) => new()
    {
        Code = request.Code.Trim(),
        Description = request.Description.Trim(),
        DisplayName = request.DisplayName.Trim(),
        TemplateCategoryId = request.TemplateCategoryId,
        RenderTypeId = renderType.Id,
        RenderType = renderType,
        Body = request.Body.Trim(),
        PreviewImageUrl = TrimOrNull(request.PreviewImageUrl),
        IsDeactivated = request.IsDeactivated
    };

    public static TemplateCategory CreateTemplateCategory(CreateTemplateCategoryRequest request) => new()
    {
        Code = request.Code.Trim(),
        Description = request.Description.Trim(),
        DisplayName = request.DisplayName.Trim(),
        IsDeactivated = request.IsDeactivated
    };

    public static void Apply(UpdateTemplateCategoryRequest request, TemplateCategory entity)
    {
        entity.Code = request.Code.Trim();
        entity.Description = request.Description.Trim();
        entity.DisplayName = request.DisplayName.Trim();
        entity.IsDeactivated = request.IsDeactivated;
        entity.ObsoleteFlag = request.ObsoleteFlag;
    }

    public static void Apply(UpdateTemplateRequest request, Template entity, TemplateRenderTypeSetup renderType)
    {
        entity.Code = request.Code.Trim();
        entity.Description = request.Description.Trim();
        entity.DisplayName = request.DisplayName.Trim();
        entity.TemplateCategoryId = request.TemplateCategoryId;
        entity.RenderTypeId = renderType.Id;
        entity.RenderType = renderType;
        entity.Body = request.Body.Trim();
        entity.PreviewImageUrl = TrimOrNull(request.PreviewImageUrl);
        entity.IsDeactivated = request.IsDeactivated;
        entity.ObsoleteFlag = request.ObsoleteFlag;
    }

    public static TemplateCategoryDetailResponse MapTemplateCategoryDetail(TemplateCategory entity) => new()
    {
        Id = entity.Id,
        Code = entity.Code,
        Description = entity.Description,
        DisplayName = entity.DisplayName,
        IsDeactivated = entity.IsDeactivated,
        ObsoleteFlag = entity.ObsoleteFlag,
        App_CreateDate = entity.App_CreateDate,
        App_UpdateDate = entity.App_UpdateDate,
        App_Version = entity.App_Version
    };

    public static TemplateCategoryListItemResponse MapTemplateCategoryListItem(TemplateCategory entity) => new()
    {
        Id = entity.Id,
        Code = entity.Code,
        DisplayName = entity.DisplayName,
        IsDeactivated = entity.IsDeactivated
    };

    public static TemplateDetailResponse MapTemplateDetail(Template entity) => new()
    {
        Id = entity.Id,
        Code = entity.Code,
        Description = entity.Description,
        DisplayName = entity.DisplayName,
        TemplateCategoryId = entity.TemplateCategoryId,
        RenderTypeCode = entity.RenderType?.Code ?? string.Empty,
        Body = entity.Body,
        PreviewImageUrl = entity.PreviewImageUrl,
        IsDeactivated = entity.IsDeactivated,
        ObsoleteFlag = entity.ObsoleteFlag,
        App_CreateDate = entity.App_CreateDate,
        App_UpdateDate = entity.App_UpdateDate,
        App_Version = entity.App_Version
    };

    public static TemplateListItemResponse MapTemplateListItem(Template entity) => new()
    {
        Id = entity.Id,
        Code = entity.Code,
        DisplayName = entity.DisplayName,
        TemplateCategoryId = entity.TemplateCategoryId,
        RenderTypeCode = entity.RenderType?.Code ?? string.Empty,
        IsDeactivated = entity.IsDeactivated
    };

    private static string? TrimOrNull(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
