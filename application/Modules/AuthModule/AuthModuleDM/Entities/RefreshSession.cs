using System.ComponentModel.DataAnnotations;
using ResumeEnhancer.Core.DomainLibrary.DomainModel;

namespace ResumeEnhancer.AuthModule.DM.Entities;

public sealed class RefreshSession : BusinessEntity
{
    public Guid SessionKey { get; set; } = Guid.NewGuid();
    public int UserId { get; set; }
    public Guid FamilyId { get; set; }

    [MaxLength(128)]
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    // Nullable for legacy rows created before durable session timestamps existed.
    // All application-created and rotated sessions set this explicitly.
    public DateTime? CreatedAtUtc { get; set; }
    public DateTime? LastUsedAtUtc { get; set; }
    public DateTime? RotatedAtUtc { get; set; }
    public DateTime? RevokedAtUtc { get; set; }

    [MaxLength(80)]
    public string? RevocationReason { get; set; }

    [MaxLength(100)]
    public string? IpAddress { get; set; }

    [MaxLength(300)]
    public string? UserAgent { get; set; }
}
