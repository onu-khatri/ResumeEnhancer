using System.ComponentModel.DataAnnotations;

namespace ResumeEnhancer.BillingModule.AM.Requests;

public sealed class CreateBillingPlanRequest
{
    [Required]
    [MaxLength(100)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string DisplayName { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    [MaxLength(10)]
    public string Currency { get; set; } = "USD";

    [Required]
    [MaxLength(50)]
    public string BillingInterval { get; set; } = "Monthly";

    public bool IsDeactivated { get; set; } = false;
}

public sealed class UpdateBillingPlanRequest
{
    [Required]
    [MaxLength(100)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string DisplayName { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    [MaxLength(10)]
    public string Currency { get; set; } = "USD";

    [Required]
    [MaxLength(50)]
    public string BillingInterval { get; set; } = "Monthly";

    public bool IsDeactivated { get; set; }
    public bool ObsoleteFlag { get; set; }
}
