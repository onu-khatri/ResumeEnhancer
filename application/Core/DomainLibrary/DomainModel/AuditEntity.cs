using System.ComponentModel.DataAnnotations;

namespace ResumeEnhancer.Core.DomainLibrary.DomainModel;

public abstract class AuditEntity : IAuditEntity
{
    [Key]
    public int Id { get; set; }

    public int? App_CreateUserId { get; set; }

    public int? App_UpdateUserId { get; set; }

    public DateTime App_CreateDate { get; set; } = DateTime.UtcNow;

    public DateTime? App_UpdateDate { get; set; }

    public byte[] App_Version { get; set; } = [];
}

