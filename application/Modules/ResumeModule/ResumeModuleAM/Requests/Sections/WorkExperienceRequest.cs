using System.ComponentModel.DataAnnotations;

namespace ResumeModuleAM.Requests;

public sealed class WorkExperienceRequest
{
    [Range(0, int.MaxValue)]
    public int Id { get; set; }

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
