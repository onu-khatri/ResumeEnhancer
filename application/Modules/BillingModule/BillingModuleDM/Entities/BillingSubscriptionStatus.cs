using System.ComponentModel.DataAnnotations;
using ResumeEnhancer.Core.DomainLibrary.DomainModel;

namespace ResumeEnhancer.BillingModule.DM.Entities;

public sealed class BillingSubscriptionStatus : SetupEntity, IHasOrderedValues
{
    [MaxLength(200)]
    public string DisplayName { get; set; } = string.Empty;

    public int Order { get; set; }

    public ICollection<BillingSubscription> BillingSubscriptions { get; set; } = new List<BillingSubscription>();
}
