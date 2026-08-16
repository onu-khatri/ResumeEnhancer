using System.ComponentModel.DataAnnotations;
using ResumeEnhancer.BillingModule.DM.Entities;
using ResumeEnhancer.Core.DomainLibrary.DomainModel;
using ResumeEnhancer.ProfilingModule.DM.Entities;

namespace ResumeEnhancer.BillingModule.DM.Entities;

public sealed class BillingAccount : BusinessEntity
{
    public int UserId { get; set; }

    public User? User { get; set; }

    [MaxLength(50)]
    public string AccountNumber { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Status { get; set; } = "Active";

    [MaxLength(100)]
    public string? ExternalReference { get; set; }

    public ICollection<BillingSubscription> Subscriptions { get; set; } = new List<BillingSubscription>();
}
