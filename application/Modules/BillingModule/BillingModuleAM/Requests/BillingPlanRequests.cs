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

    [Range(1, int.MaxValue)]
    public int CurrencyId { get; set; }

    [Range(1, int.MaxValue)]
    public int BillingIntervalId { get; set; }

    [Range(1, int.MaxValue)]
    public int AccessProfileId { get; set; }

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

    [Range(1, int.MaxValue)]
    public int CurrencyId { get; set; }

    [Range(1, int.MaxValue)]
    public int BillingIntervalId { get; set; }

    [Range(1, int.MaxValue)]
    public int AccessProfileId { get; set; }

    public bool IsDeactivated { get; set; }
    public bool ObsoleteFlag { get; set; }

    public bool CascadeExistingSubscriptions { get; set; }
}
