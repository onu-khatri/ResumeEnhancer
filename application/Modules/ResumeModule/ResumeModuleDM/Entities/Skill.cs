using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DomainLibrary.DomainModel;

namespace ResumeModuleDM.Entities;

public class Skill : BusinessRelation
{
    public int ResumeId { get; set; }

    [ForeignKey(nameof(ResumeId))]
    public Resume Resume { get; set; } = null!;

    [MaxLength(100)]
    public string SkillName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? ProficiencyLevel { get; set; }

    public decimal? YearsOfExperience { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
}
