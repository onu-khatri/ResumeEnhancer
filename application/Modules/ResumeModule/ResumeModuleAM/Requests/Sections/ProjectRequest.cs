using System.ComponentModel.DataAnnotations;

namespace ResumeEnhancer.ResumeModule.AM.Requests;

public sealed class ProjectRequest
{
    [Range(0, int.MaxValue)]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string ProjectName { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? Role { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(500)]
    public string? TechnologiesUsed { get; set; }

    public bool IsCurrent { get; set; }
}

