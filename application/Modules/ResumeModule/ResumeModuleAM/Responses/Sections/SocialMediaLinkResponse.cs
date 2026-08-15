namespace ResumeEnhancer.ResumeModule.AM.Responses;

public sealed class SocialMediaLinkResponse
{
    public int Id { get; set; }

    public string Platform { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public string? DisplayName { get; set; }
}

