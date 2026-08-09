using System.ComponentModel.DataAnnotations;

namespace ResumeModuleAM.Requests;

public sealed class SkillRequest
{
    [Range(0, int.MaxValue)]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string SkillName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? ProficiencyLevel { get; set; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal? YearsOfExperience { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
}
