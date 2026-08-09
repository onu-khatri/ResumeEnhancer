namespace ResumeModuleAM.Responses;

public sealed class PersonalInformationResponse
{
    public int Id { get; set; }

    public AddressResponse? Address { get; set; }

    public string? Email { get; set; }

    public bool UseSameEmailAsProfile { get; set; }

    public string? PhoneNumber { get; set; }

    public bool UseSamePhoneNumberAsProfile { get; set; }

    public bool UseSameAwardsAsProfile { get; set; }

    public bool UseSameLanguagesAsProfile { get; set; }

    public bool UseSameHobbiesAsProfile { get; set; }

    public bool UseSameSocialMediaLinksAsProfile { get; set; }

    public IReadOnlyList<AwardResponse> Awards { get; set; } = [];

    public IReadOnlyList<LanguageResponse> Languages { get; set; } = [];

    public IReadOnlyList<HobbyResponse> Hobbies { get; set; } = [];

    public IReadOnlyList<SocialMediaLinkResponse> SocialMediaLinks { get; set; } = [];
}
