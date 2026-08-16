using System.ComponentModel.DataAnnotations;
using ResumeEnhancer.Core.DomainLibrary.DomainModel;

namespace ResumeEnhancer.TemplateModule.DM.Entities;

public sealed class TemplateRenderTypeSetup : SetupEntity, IHasOrderedValues
{
    [MaxLength(200)]
    public string DisplayName { get; set; } = string.Empty;

    public int Order { get; set; }

    public ICollection<Template> Templates { get; set; } = new List<Template>();
}
