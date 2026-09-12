using System.ComponentModel.DataAnnotations;

namespace ResumeEnhancer.BillingModule.AM.Requests;

public sealed class CreateBillingSubscriptionRequest
{
    [Required]
    public int BillingAccountId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int BillingPlanId { get; set; }

    [Range(1, int.MaxValue)]
    public int StatusId { get; set; }

    public DateTime StartDateUtc { get; set; } = DateTime.UtcNow;

    public DateTime? EndDateUtc { get; set; }
}

public sealed class UpdateBillingSubscriptionRequest
{
    [Required]
    public int BillingAccountId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int BillingPlanId { get; set; }

    [Range(1, int.MaxValue)]
    public int StatusId { get; set; }

    public DateTime StartDateUtc { get; set; } = DateTime.UtcNow;

    public DateTime? EndDateUtc { get; set; }
}
