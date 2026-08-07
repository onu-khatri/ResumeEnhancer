using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResumeModuleDM.Entities;

public class Award
{
    [Key]
    public int Id { get; set; }

    public int PersonalInformationId { get; set; }

    [ForeignKey(nameof(PersonalInformationId))]
    public PersonalInformation PersonalInformation { get; set; } = null!;

    [MaxLength(200)]
    public string AwardName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? IssuingOrganization { get; set; }

    public DateTime? AwardDate { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }
}
