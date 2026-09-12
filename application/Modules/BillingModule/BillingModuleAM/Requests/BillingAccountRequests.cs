using System.ComponentModel.DataAnnotations;

namespace ResumeEnhancer.BillingModule.AM.Requests;

public sealed class CreateBillingAccountRequest
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(50)]
    public string AccountNumber { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int StatusId { get; set; }

    [MaxLength(100)]
    public string? ExternalReference { get; set; }
}

public sealed class UpdateBillingAccountRequest
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(50)]
    public string AccountNumber { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int StatusId { get; set; }

    [MaxLength(100)]
    public string? ExternalReference { get; set; }
}
