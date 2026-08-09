using System.ComponentModel.DataAnnotations;

namespace ResumeModuleAM.Requests;

public sealed class SocialMediaLinkRequest
{
    [Range(0, int.MaxValue)]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Platform { get; set; } = string.Empty;

    [Required]
    [Url]
    [MaxLength(500)]
    public string Url { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? DisplayName { get; set; }
}
