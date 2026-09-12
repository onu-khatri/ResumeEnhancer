using System.ComponentModel.DataAnnotations;
using ResumeEnhancer.Core.DomainLibrary.DomainModel;

namespace ResumeEnhancer.BillingModule.DM.Entities;

public sealed class BillingInterval : SetupEntity, IHasOrderedValues
{
    [MaxLength(200)]
    public string DisplayName { get; set; } = string.Empty;

    public int Order { get; set; }

    public ICollection<BillingPlan> BillingPlans { get; set; } = new List<BillingPlan>();
}
