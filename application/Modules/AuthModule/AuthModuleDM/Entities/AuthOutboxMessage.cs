using System.ComponentModel.DataAnnotations;
using ResumeEnhancer.Core.DomainLibrary.DomainModel;

namespace ResumeEnhancer.AuthModule.DM.Entities;

public sealed class AuthOutboxMessage : BusinessEntity
{
    [MaxLength(80)]
    public string Type { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string PayloadJson { get; set; } = "{}";
    public DateTime AvailableAtUtc { get; set; } = DateTime.UnixEpoch;
    public DateTime? ProcessedAtUtc { get; set; }
    public Guid? LeaseId { get; set; }
    public DateTime? LeaseExpiresAtUtc { get; set; }
    public int Attempts { get; set; }
    public string? LastError { get; set; }
}
