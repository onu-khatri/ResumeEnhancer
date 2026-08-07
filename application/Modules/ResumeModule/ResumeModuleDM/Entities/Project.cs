using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResumeModuleDM.Entities;

public class Project
{
    [Key]
    public int Id { get; set; }

    public int ResumeId { get; set; }

    [ForeignKey(nameof(ResumeId))]
    public Resume Resume { get; set; } = null!;

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
