using System.ComponentModel.DataAnnotations;
using ResumeEnhancer.Core.DomainLibrary.DomainModel;

namespace ResumeEnhancer.AuthModule.DM.Entities;

public sealed class AuthConsentVersion : SetupEntity
{
    [MaxLength(30)]
    public string Type { get; set; } = string.Empty;

    [MaxLength(100)]
    public string VersionId { get; set; } = string.Empty;
    public bool Required { get; set; }
    public bool Active { get; set; } = true;
}
