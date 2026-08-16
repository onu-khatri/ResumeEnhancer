using System.ComponentModel.DataAnnotations;
using ResumeEnhancer.Core.DomainLibrary.DomainModel;

namespace ResumeEnhancer.TemplateModule.DM.Entities;

public sealed class TemplateCategory : SetupEntity, IDeactivateable, IHasOrderedValues
{
    [MaxLength(200)]
    public string DisplayName { get; set; } = string.Empty;

    public int Order { get; set; }

    public bool IsDeactivated { get; set; } = false;

    public ICollection<Template> Templates { get; set; } = new List<Template>();
}
