using System.ComponentModel.DataAnnotations;
using ResumeEnhancer.Core.DomainLibrary.DomainModel;

namespace ResumeEnhancer.AuthModule.DM.Entities;

public sealed class AuthRegistrationIdempotency : BusinessEntity
{
    [MaxLength(320)]
    public string NormalizedEmail { get; set; } = string.Empty;

    [MaxLength(100)]
    public string IdempotencyKey { get; set; } = string.Empty;

    [MaxLength(64)]
    public string RequestHash { get; set; } = string.Empty;

    [MaxLength(12000)]
    public string ResponseJson { get; set; } = string.Empty;
}
