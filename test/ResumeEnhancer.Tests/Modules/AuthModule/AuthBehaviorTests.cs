using FluentValidation.TestHelper;
using Microsoft.Extensions.Configuration;
using ResumeEnhancer.AuthModule.AM.Requests;
using ResumeEnhancer.AuthModule.PL;
using ResumeEnhancer.AuthModule.Web;

namespace ResumeEnhancer.Tests.Unit.Modules.AuthModule;

public sealed class AuthBehaviorTests
{
    [Fact]
    public void Validator_rejects_missing_legal_consent_and_invalid_source()
    {
        var result = new RegisterRequestValidator().TestValidate(
            new RegisterRequest
            {
                FirstName = "A",
                LastName = "B",
                Email = "person@example.com",
                Password = "weak",
                Source = "https://evil.example",
            }
        );

        result
            .ShouldHaveValidationErrorFor(x => x.TermsConsent)
            .WithErrorCode("AUTH_TERMS_REQUIRED");
        result
            .ShouldHaveValidationErrorFor(x => x.PrivacyConsent)
            .WithErrorCode("AUTH_PRIVACY_REQUIRED");
        result.ShouldHaveValidationErrorFor(x => x.Source).WithErrorCode("AUTH_INVALID_SOURCE");
    }

    [Fact]
    public void Token_service_requires_shared_signing_material()
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
        Assert.Throws<InvalidOperationException>(() => new AuthTokenService(configuration));
    }

    [Fact]
    public void Validator_rejects_an_idempotency_key_longer_than_one_hundred_characters()
    {
        var request = new RegisterRequest
        {
            FirstName = "Ada",
            LastName = "Lovelace",
            Email = "ada@example.com",
            Password = "Password!1234",
            TermsConsent = true,
            PrivacyConsent = true,
            IdempotencyKey = new string('k', 101),
        };

        new RegisterRequestValidator()
            .TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.IdempotencyKey);
    }
}
