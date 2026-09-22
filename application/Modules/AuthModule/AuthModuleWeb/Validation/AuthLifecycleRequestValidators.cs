using FluentValidation;
using ResumeEnhancer.AuthModule.AM.Requests;

namespace ResumeEnhancer.AuthModule.Web;

internal static class AuthValidationRules
{
    public const int EmailMaximumLength = 320;
    public const int PasswordMaximumLength = 256;
    public const int ChallengeMaximumLength = 512;

    public static IRuleBuilderOptions<T, string> Email<T>(
        IRuleBuilderInitial<T, string> ruleBuilder
    )
    {
        return ruleBuilder
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode("AUTH_EMAIL_REQUIRED")
            .Must(value => value.Trim().Length <= EmailMaximumLength)
            .WithErrorCode("AUTH_EMAIL_TOO_LONG")
            .Must(value => new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(value.Trim()))
            .WithErrorCode("AUTH_EMAIL_INVALID");
    }

    public static IRuleBuilderOptions<T, string> Password<T>(
        IRuleBuilderInitial<T, string> ruleBuilder,
        string requiredCode = "AUTH_PASSWORD_REQUIRED"
    )
    {
        return ruleBuilder
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(requiredCode)
            .MinimumLength(12)
            .WithErrorCode("AUTH_PASSWORD_TOO_SHORT")
            .MaximumLength(PasswordMaximumLength)
            .WithErrorCode("AUTH_PASSWORD_TOO_LONG")
            .Matches("[A-Z]")
            .WithErrorCode("AUTH_PASSWORD_MISSING_UPPERCASE")
            .Matches("[a-z]")
            .WithErrorCode("AUTH_PASSWORD_MISSING_LOWERCASE")
            .Matches("[0-9]")
            .WithErrorCode("AUTH_PASSWORD_MISSING_DIGIT")
            .Matches("[^a-zA-Z0-9]")
            .WithErrorCode("AUTH_PASSWORD_MISSING_SPECIAL");
    }

    public static IRuleBuilderOptions<T, string> Challenge<T>(
        IRuleBuilderInitial<T, string> ruleBuilder
    )
    {
        return ruleBuilder
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode("AUTH_CHALLENGE_REQUIRED")
            .MaximumLength(ChallengeMaximumLength)
            .WithErrorCode("AUTH_CHALLENGE_TOO_LONG");
    }
}

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        AuthValidationRules.Email(RuleFor(x => x.Email));
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithErrorCode("AUTH_PASSWORD_REQUIRED")
            .MaximumLength(AuthValidationRules.PasswordMaximumLength)
            .WithErrorCode("AUTH_PASSWORD_TOO_LONG");
    }
}

public sealed class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .WithErrorCode("AUTH_CURRENT_PASSWORD_REQUIRED")
            .MaximumLength(AuthValidationRules.PasswordMaximumLength)
            .WithErrorCode("AUTH_CURRENT_PASSWORD_TOO_LONG");
        AuthValidationRules.Password(RuleFor(x => x.NewPassword), "AUTH_NEW_PASSWORD_REQUIRED");
    }
}

public sealed class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator()
    {
        AuthValidationRules.Email(RuleFor(x => x.Email));
    }
}

public sealed class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        AuthValidationRules.Email(RuleFor(x => x.Email));
        AuthValidationRules.Challenge(RuleFor(x => x.Challenge));
        AuthValidationRules.Password(RuleFor(x => x.NewPassword), "AUTH_NEW_PASSWORD_REQUIRED");
    }
}

public sealed class VerifyEmailRequestValidator : AbstractValidator<VerifyEmailRequest>
{
    public VerifyEmailRequestValidator()
    {
        AuthValidationRules.Email(RuleFor(x => x.Email));
        AuthValidationRules.Challenge(RuleFor(x => x.Challenge));
    }
}

public sealed class ResendVerificationRequestValidator : AbstractValidator<ResendVerificationRequest>
{
    public ResendVerificationRequestValidator()
    {
        AuthValidationRules.Email(RuleFor(x => x.Email));
    }
}
