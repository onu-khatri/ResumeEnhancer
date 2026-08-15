using System.ComponentModel.DataAnnotations;

namespace ResumeEnhancer.ResumeModule.AM.Requests;

public sealed class PersonalInformationRequest
{
    [Range(0, int.MaxValue)]
    public int Id { get; set; }

    public AddressRequest? Address { get; set; }

    public bool RemoveAddress { get; set; }

    [EmailAddress]
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

    public List<AwardRequest>? Awards { get; set; }

    public List<LanguageRequest>? Languages { get; set; }

    public List<HobbyRequest>? Hobbies { get; set; }

    public List<SocialMediaLinkRequest>? SocialMediaLinks { get; set; }
}

