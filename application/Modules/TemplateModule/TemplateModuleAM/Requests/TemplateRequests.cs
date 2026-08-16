namespace ResumeEnhancer.TemplateModule.AM.Requests;

public sealed class CreateTemplateCategoryRequest
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsDeactivated { get; set; } = false;
}

public sealed class UpdateTemplateCategoryRequest
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsDeactivated { get; set; } = false;
    public bool ObsoleteFlag { get; set; }
}

public sealed class CreateTemplateRequest
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int TemplateCategoryId { get; set; }
    public string RenderTypeCode { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? PreviewImageUrl { get; set; }
    public bool IsDeactivated { get; set; } = false;
}

public sealed class UpdateTemplateRequest
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int TemplateCategoryId { get; set; }
    public string RenderTypeCode { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? PreviewImageUrl { get; set; }
    public bool IsDeactivated { get; set; } = false;
    public bool ObsoleteFlag { get; set; }
}
