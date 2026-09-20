using System.Text.Json;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ResumeEnhancer.AuthModule.AM.Requests;
using ResumeEnhancer.AuthModule.AM.Responses;
using ResumeEnhancer.AuthModule.Web;
using Shouldly;

namespace ResumeEnhancer.Tests.Unit.Modules.AuthModule;

public sealed class AuthContractTests
{
    private static readonly DateTime TestNowUtc = new(2030, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Lifecycle_validators_are_resolved_through_auth_web_registration()
    {
        using var provider = new ServiceCollection()
            .AddAuthModuleWeb()
            .BuildServiceProvider();

        provider.GetRequiredService<IValidator<LoginRequest>>().ShouldBeOfType<LoginRequestValidator>();
        provider.GetRequiredService<IValidator<ChangePasswordRequest>>().ShouldBeOfType<ChangePasswordRequestValidator>();
        provider.GetRequiredService<IValidator<ForgotPasswordRequest>>().ShouldBeOfType<ForgotPasswordRequestValidator>();
        provider.GetRequiredService<IValidator<ResetPasswordRequest>>().ShouldBeOfType<ResetPasswordRequestValidator>();
        provider.GetRequiredService<IValidator<VerifyEmailRequest>>().ShouldBeOfType<VerifyEmailRequestValidator>();

    }

    [Fact]
    public async Task Login_validator_covers_normalization_required_invalid_and_length_codes()
    {
        using var provider = CreateProvider();
        var validator = provider.GetRequiredService<IValidator<LoginRequest>>();

        await AssertCodesAsync(validator, new LoginRequest { Email = " Ada@example.com ", Password = "secret" });
        await AssertCodesAsync(validator, new LoginRequest { Password = "secret" }, "AUTH_EMAIL_REQUIRED");
        await AssertCodesAsync(validator, new LoginRequest { Email = "ada@example.com" }, "AUTH_PASSWORD_REQUIRED");
        await AssertCodesAsync(validator, new LoginRequest { Email = "not-an-email", Password = "secret" }, "AUTH_EMAIL_INVALID");
        await AssertCodesAsync(validator, new LoginRequest { Email = "ada@example.com", Password = new string('p', 257) }, "AUTH_PASSWORD_TOO_LONG");
        await AssertCodesAsync(validator, new LoginRequest { Email = new string('a', 321) + "@example.com", Password = "secret" }, "AUTH_EMAIL_TOO_LONG");
    }

    [Fact]
    public async Task Change_password_validator_covers_current_new_and_password_strength_codes()
    {
        using var provider = CreateProvider();
        var validator = provider.GetRequiredService<IValidator<ChangePasswordRequest>>();

        await AssertCodesAsync(validator, new ChangePasswordRequest
        {
            CurrentPassword = "CurrentPassword!123",
            NewPassword = "NewPassword!123"
        });
        await AssertCodesAsync(validator, new ChangePasswordRequest { NewPassword = "NewPassword!123" }, "AUTH_CURRENT_PASSWORD_REQUIRED");
        await AssertCodesAsync(validator, new ChangePasswordRequest { CurrentPassword = new string('p', 257), NewPassword = "NewPassword!123" }, "AUTH_CURRENT_PASSWORD_TOO_LONG");
        await AssertCodesAsync(validator, new ChangePasswordRequest { CurrentPassword = "CurrentPassword!123" }, "AUTH_NEW_PASSWORD_REQUIRED");
        await AssertCodesAsync(validator, new ChangePasswordRequest { CurrentPassword = "CurrentPassword!123", NewPassword = "weak" }, "AUTH_PASSWORD_TOO_SHORT");
        await AssertCodesAsync(validator, new ChangePasswordRequest { CurrentPassword = "CurrentPassword!123", NewPassword = "validpassword!1" }, "AUTH_PASSWORD_MISSING_UPPERCASE");
        await AssertCodesAsync(validator, new ChangePasswordRequest { CurrentPassword = "CurrentPassword!123", NewPassword = "VALIDPASSWORD!1" }, "AUTH_PASSWORD_MISSING_LOWERCASE");
        await AssertCodesAsync(validator, new ChangePasswordRequest { CurrentPassword = "CurrentPassword!123", NewPassword = "ValidPassword!" }, "AUTH_PASSWORD_MISSING_DIGIT");
        await AssertCodesAsync(validator, new ChangePasswordRequest { CurrentPassword = "CurrentPassword!123", NewPassword = "ValidPassword1" }, "AUTH_PASSWORD_MISSING_SPECIAL");
        await AssertCodesAsync(validator, new ChangePasswordRequest { CurrentPassword = "CurrentPassword!123", NewPassword = new string('p', 257) }, "AUTH_PASSWORD_TOO_LONG");
    }

    [Fact]
    public async Task Forgot_password_validator_covers_email_codes()
    {
        using var provider = CreateProvider();
        var validator = provider.GetRequiredService<IValidator<ForgotPasswordRequest>>();

        await AssertCodesAsync(validator, new ForgotPasswordRequest { Email = " Ada@example.com " });
        await AssertCodesAsync(validator, new ForgotPasswordRequest(), "AUTH_EMAIL_REQUIRED");
        await AssertCodesAsync(validator, new ForgotPasswordRequest { Email = "not-an-email" }, "AUTH_EMAIL_INVALID");
        await AssertCodesAsync(validator, new ForgotPasswordRequest { Email = new string('a', 321) + "@example.com" }, "AUTH_EMAIL_TOO_LONG");
    }

    [Fact]
    public async Task Reset_password_validator_covers_email_challenge_and_password_codes()
    {
        using var provider = CreateProvider();
        var validator = provider.GetRequiredService<IValidator<ResetPasswordRequest>>();

        await AssertCodesAsync(validator, new ResetPasswordRequest
        {
            Email = " Ada@example.com ",
            Challenge = " challenge-value ",
            NewPassword = "NewPassword!123"
        });
        await AssertCodesAsync(validator, new ResetPasswordRequest { Challenge = "challenge", NewPassword = "NewPassword!123" }, "AUTH_EMAIL_REQUIRED");
        await AssertCodesAsync(validator, new ResetPasswordRequest { Email = "not-an-email", Challenge = "challenge", NewPassword = "NewPassword!123" }, "AUTH_EMAIL_INVALID");
        await AssertCodesAsync(validator, new ResetPasswordRequest { Email = "ada@example.com", NewPassword = "NewPassword!123" }, "AUTH_CHALLENGE_REQUIRED");
        await AssertCodesAsync(validator, new ResetPasswordRequest { Email = "ada@example.com", Challenge = new string('c', 513), NewPassword = "NewPassword!123" }, "AUTH_CHALLENGE_TOO_LONG");
        await AssertCodesAsync(validator, new ResetPasswordRequest { Email = "ada@example.com", Challenge = "challenge", NewPassword = "weak" }, "AUTH_PASSWORD_TOO_SHORT");
        await AssertCodesAsync(validator, new ResetPasswordRequest { Email = "ada@example.com", Challenge = "challenge", NewPassword = new string('p', 257) }, "AUTH_PASSWORD_TOO_LONG");
    }

    [Fact]
    public async Task Verify_email_validator_covers_email_and_challenge_codes()
    {
        using var provider = CreateProvider();
        var validator = provider.GetRequiredService<IValidator<VerifyEmailRequest>>();

        await AssertCodesAsync(validator, new VerifyEmailRequest { Email = " Ada@example.com ", Challenge = " challenge-value " });
        await AssertCodesAsync(validator, new VerifyEmailRequest { Challenge = "challenge" }, "AUTH_EMAIL_REQUIRED");
        await AssertCodesAsync(validator, new VerifyEmailRequest { Email = "not-an-email", Challenge = "challenge" }, "AUTH_EMAIL_INVALID");
        await AssertCodesAsync(validator, new VerifyEmailRequest { Email = "ada@example.com" }, "AUTH_CHALLENGE_REQUIRED");
        await AssertCodesAsync(validator, new VerifyEmailRequest { Email = "ada@example.com", Challenge = new string('c', 513) }, "AUTH_CHALLENGE_TOO_LONG");
    }

    private static ServiceProvider CreateProvider() => new ServiceCollection()
        .AddAuthModuleWeb()
        .BuildServiceProvider();

    private static async Task AssertCodesAsync<T>(
        IValidator<T> validator,
        T request,
        params string[] expectedCodes)
    {
        var result = await validator.ValidateAsync(request);
        result.Errors.Select(error => error.ErrorCode).ShouldBe(expectedCodes);
    }

    [Fact]
    public void Response_contracts_serialize_only_safe_identity_and_error_fields()
    {
        var identityJson = JsonSerializer.Serialize(
            new AuthenticatedIdentityResponse(42, true, false, false));
        var authenticationJson = JsonSerializer.Serialize(
            new AuthenticationResponse(
                42,
                new AuthTokens("access", "refresh", TestNowUtc, TestNowUtc.AddMinutes(30)),
                new AuthenticatedIdentityResponse(42, true, false, false)));
        var errorJson = JsonSerializer.Serialize(
            new AuthError("AUTH_INVALID_CREDENTIALS", "The credentials are invalid."));

        identityJson.ShouldContain("UserId");
        identityJson.ShouldContain("EmailVerified");
        identityJson.ShouldNotContain("Password");
        identityJson.ShouldNotContain("Challenge");
        identityJson.ShouldNotContain("Exception");

        authenticationJson.ShouldContain("AccessToken");
        authenticationJson.ShouldContain("RefreshToken");
        authenticationJson.ShouldContain("Identity");
        authenticationJson.ShouldNotContain("Password");
        authenticationJson.ShouldNotContain("Challenge");
        authenticationJson.ShouldNotContain("Exception");

        errorJson.ShouldContain("Code");
        errorJson.ShouldContain("Message");
        errorJson.ShouldNotContain("Password");
        errorJson.ShouldNotContain("RefreshToken");
        errorJson.ShouldNotContain("Challenge");
        errorJson.ShouldNotContain("StackTrace");
    }
}
