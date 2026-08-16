using System.ComponentModel.DataAnnotations;
using ResumeEnhancer.Core.DomainLibrary.DomainModel;
namespace ResumeEnhancer.TemplateModule.DM.Entities;

public sealed class Template : SetupEntity, IDeactivateable
{
    [MaxLength(200)]
    public string DisplayName { get; set; } = string.Empty;

    public int TemplateCategoryId { get; set; }

    public TemplateCategory? TemplateCategory { get; set; }

    public int RenderTypeId { get; set; }

    public TemplateRenderTypeSetup? RenderType { get; set; }

    [MaxLength(20000)]
    public string Body { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? PreviewImageUrl { get; set; }

    public bool IsDeactivated { get; set; } = false;
}
