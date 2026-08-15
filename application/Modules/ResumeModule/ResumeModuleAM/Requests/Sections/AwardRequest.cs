using System.ComponentModel.DataAnnotations;

namespace ResumeEnhancer.ResumeModule.AM.Requests;

public sealed class AwardRequest
{
    [Range(0, int.MaxValue)]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string AwardName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? IssuingOrganization { get; set; }

    public DateTime? AwardDate { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }
}

