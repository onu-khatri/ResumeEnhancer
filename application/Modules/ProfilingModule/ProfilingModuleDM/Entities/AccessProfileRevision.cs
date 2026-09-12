using ResumeEnhancer.Core.DomainLibrary.DomainModel;

namespace ResumeEnhancer.ProfilingModule.DM.Entities;

public sealed class AccessProfileRevision : BusinessEntity
{
    public string RevisionKey { get; set; } = string.Empty;

    public int UserId { get; set; }

    public int BillingSubscriptionId { get; set; }

    public string BillingPlanCode { get; set; } = string.Empty;

    public int AccessProfileId { get; set; }

    public DateTime RequestedOnUtc { get; set; } = DateTime.UtcNow;

    public DateTime? ProcessedOnUtc { get; set; }
}
