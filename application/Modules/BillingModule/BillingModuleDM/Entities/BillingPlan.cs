using System.ComponentModel.DataAnnotations;
using ResumeEnhancer.Core.DomainLibrary.DomainModel;

namespace ResumeEnhancer.BillingModule.DM.Entities;

public sealed class BillingPlan : SetupEntity, IDeactivateable, IHasOrderedValues
{
    [MaxLength(200)]
    public string DisplayName { get; set; } = string.Empty;

    public int Order { get; set; }

    public decimal Price { get; set; }

    [MaxLength(10)]
    public string Currency { get; set; } = "USD";

    [MaxLength(50)]
    public string BillingInterval { get; set; } = "Monthly";

    public bool IsDeactivated { get; set; } = false;

    public ICollection<BillingSubscription> Subscriptions { get; set; } = new List<BillingSubscription>();
}
