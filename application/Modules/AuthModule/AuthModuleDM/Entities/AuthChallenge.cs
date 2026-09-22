using System.ComponentModel.DataAnnotations;
using ResumeEnhancer.Core.DomainLibrary.DomainModel;

namespace ResumeEnhancer.AuthModule.DM.Entities;

public sealed class AuthChallenge : BusinessEntity
{
    public int AuthenticationIdentityId { get; set; }
    public AuthenticationIdentity? AuthenticationIdentity { get; set; }

    public int AuthChallengePurposeId { get; set; }
    public AuthChallengePurpose? AuthChallengePurpose { get; set; }

    [MaxLength(500)]
    public string TokenHash { get; set; } = string.Empty;

    public DateTime IssuedAtUtc { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? ConsumedAtUtc { get; set; }

    [MaxLength(100)]
    public string? IpAddress { get; set; }

    [MaxLength(300)]
    public string? UserAgent { get; set; }
}
