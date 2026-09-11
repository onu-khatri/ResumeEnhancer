using System.ComponentModel.DataAnnotations;
using ResumeEnhancer.Core.DomainLibrary.DomainModel;

namespace ResumeEnhancer.ProfilingModule.DM.Entities;

public sealed class Role : SetupEntity, IHasOrderedValues
{
    [MaxLength(200)]
    public string DisplayName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Capability { get; set; } = string.Empty;

    public int Order { get; set; }

    public ICollection<AccessProfileRole> AccessProfileRoles { get; set; } = [];
}
