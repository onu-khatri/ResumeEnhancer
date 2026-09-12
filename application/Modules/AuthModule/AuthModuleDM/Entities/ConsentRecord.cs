using System.ComponentModel.DataAnnotations;
using ResumeEnhancer.Core.DomainLibrary.DomainModel;

namespace ResumeEnhancer.AuthModule.DM.Entities;

public sealed class ConsentRecord : BusinessEntity
{
    public int UserId { get; set; }

    [MaxLength(30)]
    public string Type { get; set; } = string.Empty;

    [MaxLength(100)]
    public string VersionId { get; set; } = string.Empty;
    public bool Accepted { get; set; }
    public DateTime AcceptedAtUtc { get; set; }
    public string? IpAddress { get; set; }
    public Guid? AuditEventId { get; set; }
}
