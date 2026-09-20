using System.ComponentModel.DataAnnotations;
using ResumeEnhancer.Core.DomainLibrary.DomainModel;

namespace ResumeEnhancer.AuthModule.DM.Entities;

public sealed class AuthSigningKeyMetadata : BusinessEntity
{
    [MaxLength(128)]
    public string KeyIdentifier { get; set; } = string.Empty;

    public byte[] ProtectedMaterial { get; set; } = [];

    public bool IsActive { get; set; }

    public long LifecycleVersion { get; set; }

    public DateTime ActivatedAtUtc { get; set; }
    public DateTime? RetiredAtUtc { get; set; }
    public DateTime? InvalidatedAtUtc { get; set; }
}
