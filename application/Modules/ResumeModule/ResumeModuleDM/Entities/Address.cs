using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResumeModuleDM.Entities;

public class Address
{
    [Key]
    public int Id { get; set; }

    public int PersonalInformationId { get; set; }

    [ForeignKey(nameof(PersonalInformationId))]
    public PersonalInformation PersonalInformation { get; set; } = null!;

    [MaxLength(200)]
    public string? StreetAddress { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? State { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; }

    [MaxLength(20)]
    public string? ZipCode { get; set; }
}
