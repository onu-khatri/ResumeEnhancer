using Mediator;
using ResumeEnhancer.AuthModule.AM.Responses;
using ResumeEnhancer.AuthModule.SL.Contracts;
using ResumeEnhancer.AuthModule.SL.Services;

namespace ResumeEnhancer.AuthModule.SL.Handlers;

public sealed class RegisterCommandHandler(IRegistrationService service)
    : ICommandHandler<RegisterCommand, RegisterResponse>
{
    public ValueTask<RegisterResponse> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken
    )
    {
        return new(service.RegisterAsync(request, cancellationToken));
    }
}

public sealed class RefreshCommandHandler(IRegistrationService service)
    : ICommandHandler<RefreshCommand, AuthTokens>
{
    public ValueTask<AuthTokens> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        return new(service.RefreshAsync(request, cancellationToken));
    }
}

public sealed class LogoutCommandHandler(IRegistrationService service)
    : ICommandHandler<LogoutCommand, bool>
{
    public ValueTask<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        return new(service.LogoutAsync(request, cancellationToken));
    }
}

public sealed class ResendVerificationCommandHandler(IRegistrationService service)
    : ICommandHandler<ResendVerificationCommand, bool>
{
    public ValueTask<bool> Handle(
        ResendVerificationCommand request,
        CancellationToken cancellationToken
    )
    {
        return new(service.ResendVerificationAsync(request, cancellationToken));
    }
}

public sealed class LoginCommandHandler(IAuthLifecycleService service) : ICommandHandler<LoginCommand, AuthenticationResponse>
{
    public ValueTask<AuthenticationResponse> Handle(LoginCommand request, CancellationToken cancellationToken) => new(service.LoginAsync(request, cancellationToken));
}
public sealed class ChangePasswordCommandHandler(IAuthLifecycleService service) : ICommandHandler<ChangePasswordCommand, bool>
{
    public ValueTask<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken) => new(service.ChangePasswordAsync(request, cancellationToken));
}
public sealed class ForgotPasswordCommandHandler(IAuthLifecycleService service) : ICommandHandler<ForgotPasswordCommand, bool>
{
    public ValueTask<bool> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken) => new(service.ForgotPasswordAsync(request, cancellationToken));
}
public sealed class ResetPasswordCommandHandler(IAuthLifecycleService service) : ICommandHandler<ResetPasswordCommand, bool>
{
    public ValueTask<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken) => new(service.ResetPasswordAsync(request, cancellationToken));
}
public sealed class VerifyEmailCommandHandler(IAuthLifecycleService service) : ICommandHandler<VerifyEmailCommand, bool>
{
    public ValueTask<bool> Handle(VerifyEmailCommand request, CancellationToken cancellationToken) => new(service.VerifyEmailAsync(request, cancellationToken));
}
