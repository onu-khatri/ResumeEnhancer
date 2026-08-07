using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResumeModuleDM.Entities;

public class Education
{
    [Key]
    public int Id { get; set; }

    public int ResumeId { get; set; }

    [ForeignKey(nameof(ResumeId))]
    public Resume Resume { get; set; } = null!;

    public int? PassingYear { get; set; }

    [MaxLength(200)]
    public string? Degree { get; set; }

    [MaxLength(200)]
    public string? Institution { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? State { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    public decimal? Percentage { get; set; }

    [MaxLength(50)]
    public string? Grade { get; set; }

    public bool IsCurrent { get; set; }
}
