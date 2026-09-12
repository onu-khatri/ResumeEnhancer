using System.ComponentModel.DataAnnotations;
using ResumeEnhancer.Core.DomainLibrary.DomainModel;
using ResumeEnhancer.ProfilingModule.DM.Entities;

namespace ResumeEnhancer.BillingModule.DM.Entities;

public sealed class BillingSubscription : BusinessRelation
{
    public int BillingAccountId { get; set; }

    public BillingAccount? BillingAccount { get; set; }

    public int UserId { get; set; }

    public User? User { get; set; }

    public int BillingPlanId { get; set; }

    public BillingPlan? BillingPlan { get; set; }

    public int StatusId { get; set; }

    public BillingSubscriptionStatus? Status { get; set; }

    public DateTime StartDateUtc { get; set; } = DateTime.UtcNow;

    public DateTime? EndDateUtc { get; set; }
}
