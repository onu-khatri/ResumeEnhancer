using System.ComponentModel.DataAnnotations;

namespace DomainLibrary.DomainModel;

public abstract class SetupData : AuditEntity, ISetupData
{
    [MaxLength(100)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    public System.Guid? Guid { get; set; }

    public bool ObsoleteFlag { get; set; }
}
