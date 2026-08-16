namespace ResumeEnhancer.TemplateModule.AM.Responses;

public sealed class TemplateCategoryDetailResponse
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsDeactivated { get; set; }
    public bool ObsoleteFlag { get; set; }
    public DateTime App_CreateDate { get; set; }
    public DateTime? App_UpdateDate { get; set; }
    public byte[] App_Version { get; set; } = [];
}

public sealed class TemplateCategoryListItemResponse
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsDeactivated { get; set; }
}

public sealed class TemplateDetailResponse
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int TemplateCategoryId { get; set; }
    public string RenderTypeCode { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? PreviewImageUrl { get; set; }
    public bool IsDeactivated { get; set; }
    public bool ObsoleteFlag { get; set; }
    public DateTime App_CreateDate { get; set; }
    public DateTime? App_UpdateDate { get; set; }
    public byte[] App_Version { get; set; } = [];
}

public sealed class TemplateListItemResponse
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int TemplateCategoryId { get; set; }
    public string RenderTypeCode { get; set; } = string.Empty;
    public bool IsDeactivated { get; set; }
}
