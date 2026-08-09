using System.ComponentModel.DataAnnotations;

namespace ResumeModuleAM.Requests;

public sealed class LanguageRequest
{
    [Range(0, int.MaxValue)]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string LanguageName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? ProficiencyLevel { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
}
