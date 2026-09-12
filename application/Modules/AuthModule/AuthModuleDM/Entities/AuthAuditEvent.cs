using System.ComponentModel.DataAnnotations;
using ResumeEnhancer.Core.DomainLibrary.DomainModel;

namespace ResumeEnhancer.AuthModule.DM.Entities;

public sealed class AuthAuditEvent : BusinessEntity
{
    public int? UserId { get; set; }

    [MaxLength(80)]
    public string EventType { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string MetadataJson { get; set; } = "{}";
    public string? IpAddress { get; set; }
}
