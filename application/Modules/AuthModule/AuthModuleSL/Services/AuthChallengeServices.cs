using System.Security.Cryptography;
using System.Text;
using ResumeEnhancer.AuthModule.DM.Entities;
using ResumeEnhancer.AuthModule.SL.Abstractions;
using ResumeEnhancer.ProfilingModule.SL.Integrations;

namespace ResumeEnhancer.AuthModule.SL.Services;

internal sealed class AuthChallengeFactory(IAuthRepository repository) : IAuthChallengeFactory
{
    public async Task<(AuthChallenge Challenge, string RawChallenge)> CreateAsync(
        AuthenticationIdentity identity, string purposeCode, DateTime issuedAtUtc, DateTime expiresAtUtc,
        string? ipAddress, string? userAgent, CancellationToken cancellationToken = default)
    {
        var purposeId = await repository.FindActiveChallengePurposeIdAsync(purposeCode, cancellationToken)
            ?? throw new AuthException("AUTH_CONFIGURATION_UNAVAILABLE", "Authentication challenge delivery is unavailable.", 503);
        var raw = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        return (new AuthChallenge
        {
            AuthenticationIdentityId = identity.Id, AuthChallengePurposeId = purposeId,
            TokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(raw))).ToLowerInvariant(),
            IssuedAtUtc = issuedAtUtc, ExpiresAtUtc = expiresAtUtc, IpAddress = ipAddress, UserAgent = userAgent,
        }, raw);
    }
}

internal sealed class AuthCurrentStateService(IAuthRepository repository, IUserLookupService users, TimeProvider timeProvider) : IAuthCurrentStateService
{
    public async Task<AuthCurrentState> EvaluateAsync(int userId, Guid sessionKey, CancellationToken cancellationToken = default)
    {
        var session = await repository.FindSessionBySessionKeyAsync(sessionKey, cancellationToken);
        var identity = await repository.FindIdentityForUserIdAsync(userId, cancellationToken);
        var profile = await users.GetUserStateAsync(userId, cancellationToken);
        var now = timeProvider.GetUtcNow().UtcDateTime;
        if (session is null
            || session.UserId != userId
            || session.ExpiresAtUtc <= now
            || session.RevokedAtUtc is not null
            || session.RotatedAtUtc is not null)
            return new(false, "session_invalid");
        if (identity is null || !identity.EmailVerified || identity.LockedUntilUtc > now)
            return new(false, "identity_invalid");
        if (profile is null || profile.IsDeactivated || profile.IsDeleted)
            return new(false, "profile_invalid");
        return new(true);
    }
}
