using ResumeEnhancer.AuthModule.DM.Entities;

namespace ResumeEnhancer.AuthModule.SL.Abstractions;

public sealed record AuthCurrentState(bool IsValid, string? FailureCode = null);

public sealed record AuthChallengeDeliveryEnvelope(
    int Version,
    string Purpose,
    int UserId,
    string Email,
    string ProtectedValue);

public sealed record AuthSideEffectPayload(
    int UserId,
    string Email,
    AuthChallengeDeliveryEnvelope? Challenge = null);

public interface IAuthCurrentStateService
{
    Task<AuthCurrentState> EvaluateAsync(int userId, Guid sessionKey, CancellationToken cancellationToken = default);
}

public interface IAuthChallengeFactory
{
    Task<(AuthChallenge Challenge, string RawChallenge)> CreateAsync(
        AuthenticationIdentity identity, string purposeCode, DateTime issuedAtUtc, DateTime expiresAtUtc,
        string? ipAddress, string? userAgent, CancellationToken cancellationToken = default);
}

public interface IAuthChallengeDeliveryProtector
{
    string Protect(string rawChallenge, string purposeCode, int userId, string email);
    string Unprotect(string protectedChallenge, string purposeCode, int userId, string email);
}
