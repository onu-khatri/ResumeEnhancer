using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ResumeEnhancer.Core.DomainLibrary.DomainModel;

namespace ResumeEnhancer.ResumeModule.DM.Entities;

public class SocialMediaLink : BusinessRelation
{
    public int PersonalInformationId { get; set; }

    [ForeignKey(nameof(PersonalInformationId))]
    public PersonalInformation PersonalInformation { get; set; } = null!;

    [MaxLength(100)]
    public string Platform { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Url { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? DisplayName { get; set; }
}

