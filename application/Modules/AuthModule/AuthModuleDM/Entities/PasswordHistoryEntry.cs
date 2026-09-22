using System.ComponentModel.DataAnnotations;
using ResumeEnhancer.Core.DomainLibrary.DomainModel;

namespace ResumeEnhancer.AuthModule.DM.Entities;

public sealed class PasswordHistoryEntry : BusinessEntity
{
    public int AuthenticationIdentityId { get; set; }
    public AuthenticationIdentity? AuthenticationIdentity { get; set; }

    [MaxLength(500)]
    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }
}
