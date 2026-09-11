using FluentValidation;
using ResumeEnhancer.AuthModule.AM.Requests;

namespace ResumeEnhancer.AuthModule.Web;

public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    private static readonly string[] Sources =
    [
        "homepage",
        "pricing",
        "template-selection",
        "premium-lock",
    ];

    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName).Must(x => !string.IsNullOrWhiteSpace(x)).MaximumLength(100);
        RuleFor(x => x.LastName).Must(x => !string.IsNullOrWhiteSpace(x)).MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(320);
        RuleFor(x => x.Password)
            .MinimumLength(12)
            .Matches("[A-Z]")
            .Matches("[a-z]")
            .Matches("[0-9]")
            .Matches("[^a-zA-Z0-9]");
        RuleFor(x => x.TermsConsent).Equal(true).WithErrorCode("AUTH_TERMS_REQUIRED");
        RuleFor(x => x.PrivacyConsent).Equal(true).WithErrorCode("AUTH_PRIVACY_REQUIRED");
        RuleFor(x => x.Source)
            .Must(x => Sources.Contains(x.Trim().ToLowerInvariant()))
            .WithErrorCode("AUTH_INVALID_SOURCE");
        RuleFor(x => x.IdempotencyKey).MaximumLength(100);
    }
}
