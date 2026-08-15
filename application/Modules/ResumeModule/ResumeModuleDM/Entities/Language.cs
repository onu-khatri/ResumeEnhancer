using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ResumeEnhancer.Core.DomainLibrary.DomainModel;

namespace ResumeEnhancer.ResumeModule.DM.Entities;

public class Language : BusinessRelation
{
    public int PersonalInformationId { get; set; }

    [ForeignKey(nameof(PersonalInformationId))]
    public PersonalInformation PersonalInformation { get; set; } = null!;

    [MaxLength(100)]
    public string LanguageName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? ProficiencyLevel { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
}

