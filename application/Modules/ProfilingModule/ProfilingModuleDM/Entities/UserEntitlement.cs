using System.ComponentModel.DataAnnotations;
using ResumeEnhancer.Core.DomainLibrary.DomainModel;

namespace ResumeEnhancer.ProfilingModule.DM.Entities;

public sealed class UserEntitlement : BusinessEntity
{
    public int UserId { get; set; }
    public User? User { get; set; }
    public int AccessProfileId { get; set; }
    public AccessProfile? AccessProfile { get; set; }
    public int BillingSubscriptionId { get; set; }
    public bool Enabled { get; set; }

    [MaxLength(30)]
    public string Source { get; set; } = "plan";
    public DateTime? ExpiresAtUtc { get; set; }
}
