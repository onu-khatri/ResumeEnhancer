using System.ComponentModel.DataAnnotations;
using ResumeEnhancer.Core.DomainLibrary.DomainModel;
using ResumeEnhancer.ProfilingModule.DM.Entities;

namespace ResumeEnhancer.BillingModule.DM.Entities;

public sealed class BillingPlan : SetupEntity, IDeactivateable, IHasOrderedValues
{
    [MaxLength(200)]
    public string DisplayName { get; set; } = string.Empty;

    public int Order { get; set; }

    public decimal Price { get; set; }

    public int CurrencyId { get; set; }

    public Currency? Currency { get; set; }

    public int BillingIntervalId { get; set; }

    public BillingInterval? BillingInterval { get; set; }

    public bool IsDeactivated { get; set; } = false;

    public int AccessProfileId { get; set; }

    public AccessProfile? AccessProfile { get; set; }

    public ICollection<BillingSubscription> Subscriptions { get; set; } = new List<BillingSubscription>();
}
