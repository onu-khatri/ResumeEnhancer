using System.ComponentModel.DataAnnotations;

namespace ResumeEnhancer.BillingModule.AM.Requests;

public sealed class CreateBillingSubscriptionRequest
{
    [Required]
    public int BillingAccountId { get; set; }

    [Required]
    public int BillingPlanId { get; set; }

    public int? ResumeId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Active";

    public DateTime StartDateUtc { get; set; } = DateTime.UtcNow;

    public DateTime? EndDateUtc { get; set; }
}

public sealed class UpdateBillingSubscriptionRequest
{
    [Required]
    public int BillingAccountId { get; set; }

    [Required]
    public int BillingPlanId { get; set; }

    public int? ResumeId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Active";

    public DateTime StartDateUtc { get; set; } = DateTime.UtcNow;

    public DateTime? EndDateUtc { get; set; }
}
