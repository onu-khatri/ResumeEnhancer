namespace ResumeEnhancer.AuthModule.AM.Requests;

public sealed class RegisterRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool TermsConsent { get; set; }
    public bool PrivacyConsent { get; set; }
    public bool MarketingConsent { get; set; }
    public string Source { get; set; } = "homepage";
    public string? SelectedTemplateId { get; set; }
    public string? IdempotencyKey { get; set; }
}

public sealed class RefreshRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}

public sealed class ResendVerificationRequest
{
    public string Email { get; set; } = string.Empty;
    public string? IdempotencyKey { get; set; }
}
