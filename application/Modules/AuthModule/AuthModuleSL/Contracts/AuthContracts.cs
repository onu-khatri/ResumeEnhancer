using Mediator;
using ResumeEnhancer.AuthModule.AM.Requests;
using ResumeEnhancer.AuthModule.AM.Responses;

namespace ResumeEnhancer.AuthModule.SL.Contracts;

public sealed record RegisterCommand(RegisterRequest Request, string? IpAddress, string? UserAgent)
    : ICommand<RegisterResponse>;

public sealed record RefreshCommand(RefreshRequest Request) : ICommand<AuthTokens>;

public sealed record LogoutCommand(string RefreshToken) : ICommand<bool>;

public sealed record ResendVerificationCommand(ResendVerificationRequest Request) : ICommand<bool>;

public sealed record LoginCommand(LoginRequest Request, string? IpAddress, string? UserAgent)
    : ICommand<AuthenticationResponse>;
public sealed record ChangePasswordCommand(ChangePasswordRequest Request, int UserId)
    : ICommand<bool>;
public sealed record ForgotPasswordCommand(ForgotPasswordRequest Request, string? IpAddress, string? UserAgent)
    : ICommand<bool>;
public sealed record ResetPasswordCommand(ResetPasswordRequest Request, string? IpAddress)
    : ICommand<bool>;
public sealed record VerifyEmailCommand(VerifyEmailRequest Request, string? IpAddress)
    : ICommand<bool>;
