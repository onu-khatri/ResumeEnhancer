using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ResumeEnhancer.AuthModule.AM.Requests;
using ResumeEnhancer.AuthModule.AM.Responses;
using ResumeEnhancer.AuthModule.DM.Entities;
using ResumeEnhancer.AuthModule.SL.Abstractions;
using ResumeEnhancer.AuthModule.SL.Contracts;
using ResumeEnhancer.AuthModule.SL.Options;
using ResumeEnhancer.ProfilingModule.SL.Integrations;

namespace ResumeEnhancer.AuthModule.SL.Services;

internal sealed class AuthLifecycleService(
    IAuthRepository repository,
    IUserLookupService users,
    IPasswordHasher hasher,
    ITokenService tokens,
    IAuthSecurityStateService securityState,
    IRegistrationThrottle limiter,
    IProfilingAuthorizationService profilingAuthorization,
    IAuthAuditRecorder auditRecorder,
    TimeProvider timeProvider,
    IAuthChallengeFactory challengeFactory,
    IAuthChallengeDeliveryProtector deliveryProtector) : IAuthLifecycleService
{
    private const string ResetPurpose = "password-reset";
    private const string VerificationPurpose = "email-verification";

    public async Task<AuthenticationResponse> LoginAsync(LoginCommand command, CancellationToken ct)
    {
        var email = RegistrationService.NormalizeEmail(command.Request.Email);
        await EnsureAllowedAsync(LimiterOperation.Login, email, null, command.IpAddress, ct);
        var identity = await repository.FindIdentityAsync(email, ct);
        var profile = identity is null ? null : await users.GetUserStateAsync(identity.UserId, ct);
        var valid = identity is not null
            && profile is not null
            && !profile.IsDeactivated
            && !profile.IsDeleted
            && identity.EmailVerified
            && (identity.LockedUntilUtc is null || identity.LockedUntilUtc <= timeProvider.GetUtcNow().UtcDateTime)
            && hasher.Verify(identity.PasswordHash, command.Request.Password);
        if (!valid)
        {
            if (identity is not null && profile is not null && !profile.IsDeactivated && !profile.IsDeleted)
            {
                var failure = await securityState.RecordFailedLoginAsync(identity.Id, timeProvider.GetUtcNow().UtcDateTime, ct);
                if (failure.Delay > TimeSpan.Zero) await Task.Delay(failure.Delay, timeProvider, ct);
                await auditRecorder.RecordAsync("login_failed", identity.UserId, command.IpAddress, cancellationToken: ct);
            }
            else
            {
                await auditRecorder.RecordAsync("login_failed", null, command.IpAddress, cancellationToken: ct);
            }
            throw new AuthException("AUTH_INVALID_CREDENTIALS", "The credentials are invalid.", 401);
        }

        await repository.ClearLockoutAsync(identity!.Id, ct);
        var refresh = tokens.CreateRefreshToken();
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var session = new RefreshSession
        {
            UserId = identity.UserId,
            TokenHash = tokens.HashRefreshToken(refresh),
            FamilyId = Guid.NewGuid(),
            ExpiresAtUtc = now.AddDays(30),
            CreatedAtUtc = now,
            IpAddress = command.IpAddress,
            UserAgent = command.UserAgent,
        };
        await repository.AddSessionAsync(session, ct);
        await repository.SaveAsync(ct);
        var (access, accessExpiry) = await tokens.CreateAccessTokenAsync(identity.UserId, session.SessionKey, ct);
        await auditRecorder.RecordAsync("login_succeeded", identity.UserId, command.IpAddress, cancellationToken: ct);
        return new AuthenticationResponse(
            identity.UserId,
            new AuthTokens(access, refresh, accessExpiry, session.ExpiresAtUtc),
            new AuthenticatedIdentityResponse(identity.UserId, identity.EmailVerified, profile!.IsDeactivated, identity.LockedUntilUtc is not null));
    }

    public async Task<bool> ChangePasswordAsync(ChangePasswordCommand command, CancellationToken ct)
    {
        var identity = await repository.FindIdentityForUserIdAsync(command.UserId, ct)
            ?? throw new AuthException("AUTH_INVALID_CREDENTIALS", "The credentials are invalid.", 401);
        var authorization = await profilingAuthorization.GetUserAuthorizationAsync(command.UserId, ct);
        if (authorization is null || authorization.IsDeactivated || authorization.IsDeleted)
        {
            await auditRecorder.RecordAsync("authorization_denied", command.UserId, null,
                JsonSerializer.Serialize(new { outcome = "denied", reason = "account_inactive" }), ct);
            throw new AuthException("AUTH_ACCOUNT_UNAVAILABLE", "The account is not available.", 403);
        }
        await EnsureAllowedAsync(LimiterOperation.PasswordChange, identity.NormalizedEmail, command.UserId.ToString(), null, ct);
        if (!hasher.Verify(identity.PasswordHash, command.Request.CurrentPassword))
            throw new AuthException("AUTH_INVALID_CREDENTIALS", "The credentials are invalid.", 401);
        if (await securityState.IsPasswordReusedAsync(identity.Id, command.Request.NewPassword, ct))
            throw new AuthException("AUTH_PASSWORD_REUSED", "The password cannot be reused.", 422);
        await repository.UpdatePasswordAndRevokeSessionsAsync(identity.Id, identity.UserId, hasher.Hash(command.Request.NewPassword), timeProvider.GetUtcNow().UtcDateTime, "password_changed", ct);
        await auditRecorder.RecordAsync("password_changed", identity.UserId, null, cancellationToken: ct);
        return true;
    }

    public async Task<bool> ForgotPasswordAsync(ForgotPasswordCommand command, CancellationToken ct)
    {
        var email = RegistrationService.NormalizeEmail(command.Request.Email);
        await EnsureAllowedAsync(LimiterOperation.Recovery, email, null, command.IpAddress, ct);
        var identity = await repository.FindIdentityAsync(email, ct);
        if (identity is null) return true;
        var purposeId = await repository.FindActiveChallengePurposeIdAsync(ResetPurpose, ct);
        if (purposeId is null) return true;
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var (challengeRecord, rawChallenge) = await challengeFactory.CreateAsync(
            identity, ResetPurpose, now, now.AddMinutes(30), command.IpAddress, command.UserAgent, ct);
        await repository.AddChallengeAsync(challengeRecord, ct);
        var resetEnvelope = new AuthChallengeDeliveryEnvelope(
            1,
            ResetPurpose,
            identity.UserId,
            email,
            deliveryProtector.Protect(rawChallenge, ResetPurpose, identity.UserId, email));
        await repository.AddOutboxAsync(new AuthOutboxMessage
        {
            Type = "password-reset-email",
            PayloadJson = JsonSerializer.Serialize(
                new AuthSideEffectPayload(identity.UserId, email, resetEnvelope)),
            AvailableAtUtc = now,
        }, ct);
        await repository.SaveAsync(ct);
        await auditRecorder.RecordAsync("password_reset_requested", identity.UserId, command.IpAddress, cancellationToken: ct);
        return true;
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordCommand command, CancellationToken ct)
    {
        var email = RegistrationService.NormalizeEmail(command.Request.Email);
        await EnsureAllowedAsync(LimiterOperation.Recovery, email, null, command.IpAddress, ct);
        var identity = await repository.FindIdentityAsync(email, ct);
        if (identity is null) return true;
        if (await securityState.IsPasswordReusedAsync(identity.Id, command.Request.NewPassword, ct))
            throw new AuthException("AUTH_PASSWORD_REUSED", "The password cannot be reused.", 422);
        var changed = await repository.ConsumeChallengeAndUpdatePasswordAsync(identity.Id, identity.UserId, ResetPurpose, HashChallenge(command.Request.Challenge), hasher.Hash(command.Request.NewPassword), timeProvider.GetUtcNow().UtcDateTime, "password_reset", ct);
        if (changed) await auditRecorder.RecordAsync("password_reset_completed", identity.UserId, command.IpAddress, cancellationToken: ct);
        return changed;
    }

    public async Task<bool> VerifyEmailAsync(VerifyEmailCommand command, CancellationToken ct)
    {
        var email = RegistrationService.NormalizeEmail(command.Request.Email);
        await EnsureAllowedAsync(LimiterOperation.Verification, email, null, command.IpAddress, ct);
        var identity = await repository.FindIdentityAsync(email, ct);
        if (identity is null) return true;
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var consumed = await repository.TryConsumeChallengeAsync(identity.Id, VerificationPurpose, HashChallenge(command.Request.Challenge), now, ct);
        if (consumed != ChallengeConsumeResult.Consumed) return true;
        await repository.MarkEmailVerifiedAsync(identity.Id, now, ct);
        await repository.SaveAsync(ct);
        await auditRecorder.RecordAsync("email_verified", identity.UserId, command.IpAddress, cancellationToken: ct);
        return true;
    }

    public async Task<AuthenticatedIdentityResponse?> GetMeAsync(int userId, CancellationToken ct)
    {
        var identity = await repository.FindIdentityForUserIdAsync(userId, ct);
        var profile = await users.GetUserStateAsync(userId, ct);
        return identity is null || profile is null
            ? null
            : new AuthenticatedIdentityResponse(userId, identity.EmailVerified, profile.IsDeactivated || profile.IsDeleted, identity.LockedUntilUtc > timeProvider.GetUtcNow().UtcDateTime);
    }

    private async Task EnsureAllowedAsync(LimiterOperation operation, string? email, string? subject, string? ip, CancellationToken ct)
    {
        var decision = await limiter.TryConsumeAsync(operation, email, subject, ip, ct);
        if (decision.Degraded)
        {
            await auditRecorder.RecordAsync("limiter_degraded", null, ip,
                JsonSerializer.Serialize(new { operation = operation.ToString(), outcome = "fail_open" }), ct);
        }
        if (!decision.Allowed)
        {
            await auditRecorder.RecordAsync("throttled", null, ip, JsonSerializer.Serialize(new { operation = operation.ToString(), outcome = "denied" }), ct);
            throw new AuthException("AUTH_RATE_LIMITED", "Too many authentication attempts.", 429);
        }
    }

    private static string HashChallenge(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();
}

public interface IAuthLifecycleService
{
    Task<AuthenticationResponse> LoginAsync(LoginCommand command, CancellationToken cancellationToken = default);
    Task<bool> ChangePasswordAsync(ChangePasswordCommand command, CancellationToken cancellationToken = default);
    Task<bool> ForgotPasswordAsync(ForgotPasswordCommand command, CancellationToken cancellationToken = default);
    Task<bool> ResetPasswordAsync(ResetPasswordCommand command, CancellationToken cancellationToken = default);
    Task<bool> VerifyEmailAsync(VerifyEmailCommand command, CancellationToken cancellationToken = default);
    Task<AuthenticatedIdentityResponse?> GetMeAsync(int userId, CancellationToken cancellationToken = default);
}
