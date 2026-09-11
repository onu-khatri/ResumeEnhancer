using System.ComponentModel.DataAnnotations;
using ResumeEnhancer.Core.DomainLibrary.DomainModel;
using ResumeEnhancer.ProfilingModule.DM.Entities;

namespace ResumeEnhancer.AuthModule.DM.Entities;

public sealed class AuthenticationIdentity : BusinessEntity
{
    public int UserId { get; set; }
    public User? User { get; set; }

    [MaxLength(320)]
    public string NormalizedEmail { get; set; } = string.Empty;

    [MaxLength(500)]
    public string PasswordHash { get; set; } = string.Empty;
    public bool EmailVerified { get; set; }
    public DateTime? EmailVerifiedAtUtc { get; set; }
}
