using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResumeModuleDM.Entities;

public class Hobby
{
    [Key]
    public int Id { get; set; }

    public int PersonalInformationId { get; set; }

    [ForeignKey(nameof(PersonalInformationId))]
    public PersonalInformation PersonalInformation { get; set; } = null!;

    [MaxLength(100)]
    public string HobbyName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
}
