using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResumeModuleDM.Entities;

public class WorkExperience
{
    [Key]
    public int Id { get; set; }

    public int ResumeId { get; set; }

    [ForeignKey(nameof(ResumeId))]
    public Resume Resume { get; set; } = null!;

    [MaxLength(150)]
    public string? JobTitle { get; set; }

    [MaxLength(200)]
    public string? CompanyName { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [MaxLength(200)]
    public string? Location { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    public bool IsCurrent { get; set; }
}
