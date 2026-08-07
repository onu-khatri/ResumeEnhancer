using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResumeModuleDM.Entities;

public class PersonalInformation
{
    [Key]
    public int Id { get; set; }

    public int ResumeId { get; set; }

    [ForeignKey(nameof(ResumeId))]
    public Resume Resume { get; set; } = null!;

    public Address? Address { get; set; }

    [MaxLength(256)]
    public string? Email { get; set; }

    public bool UseSameEmailAsProfile { get; set; }

    [MaxLength(30)]
    public string? PhoneNumber { get; set; }

    public bool UseSamePhoneNumberAsProfile { get; set; }

    public bool UseSameAwardsAsProfile { get; set; }

    public bool UseSameLanguagesAsProfile { get; set; }

    public bool UseSameHobbiesAsProfile { get; set; }

    public bool UseSameSocialMediaLinksAsProfile { get; set; }

    public ICollection<Award> Awards { get; set; } = new List<Award>();

    public ICollection<Language> Languages { get; set; } = new List<Language>();

    public ICollection<Hobby> Hobbies { get; set; } = new List<Hobby>();

    public ICollection<SocialMediaLink> SocialMediaLinks { get; set; } = new List<SocialMediaLink>();
}
