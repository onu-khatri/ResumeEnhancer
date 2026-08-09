using System.ComponentModel.DataAnnotations;

namespace ResumeModuleAM.Requests;

public sealed class EducationRequest
{
    [Range(0, int.MaxValue)]
    public int Id { get; set; }

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

    [Range(typeof(decimal), "0", "100")]
    public decimal? Percentage { get; set; }

    [MaxLength(50)]
    public string? Grade { get; set; }

    public bool IsCurrent { get; set; }
}
