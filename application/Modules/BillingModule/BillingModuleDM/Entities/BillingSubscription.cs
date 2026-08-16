using System.ComponentModel.DataAnnotations;
using ResumeEnhancer.Core.DomainLibrary.DomainModel;
using ResumeEnhancer.ResumeModule.DM.Entities;

namespace ResumeEnhancer.BillingModule.DM.Entities;

public sealed class BillingSubscription : BusinessRelation
{
    public int BillingAccountId { get; set; }

    public BillingAccount? BillingAccount { get; set; }

    public int BillingPlanId { get; set; }

    public BillingPlan? BillingPlan { get; set; }

    public int? ResumeId { get; set; }

    public Resume? Resume { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "Active";

    public DateTime StartDateUtc { get; set; } = DateTime.UtcNow;

    public DateTime? EndDateUtc { get; set; }
}
