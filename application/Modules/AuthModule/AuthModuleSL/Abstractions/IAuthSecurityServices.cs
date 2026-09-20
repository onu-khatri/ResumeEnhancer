using System.Security.Claims;

namespace ResumeEnhancer.AuthModule.SL.Abstractions;

public interface IPasswordHasher
{
    public string Hash(string password);
    public bool Verify(string hash, string password);
}

public interface ITokenService
{
    public Task<(string AccessToken, DateTime ExpiresAtUtc)> CreateAccessTokenAsync(
        int userId,
        Guid sessionId,
        CancellationToken cancellationToken = default
    );
    public string CreateRefreshToken();
    public string HashRefreshToken(string token);
    public Task<ClaimsPrincipal?> ValidateAccessTokenAsync(string token, CancellationToken cancellationToken = default);
    public Task InvalidateKeyAsync(string keyIdentifier, CancellationToken cancellationToken = default);
}

public interface IAuthSideEffectQueue
{
    public Task EnqueueAsync(
        string type,
        int userId,
        string email,
        CancellationToken cancellationToken = default
    );
}

public interface IAuthSideEffectHandler
{
    public Task HandleAsync(
        string type,
        int userId,
        string email,
        string? challenge,
        CancellationToken cancellationToken = default
    );
}

public interface IRegistrationThrottle
{
    public Task<bool> IsAllowedAsync(
        string normalizedEmail,
        string? ipAddress,
        CancellationToken cancellationToken = default
    );

    public Task<LimiterDecision> TryConsumeAsync(
        LimiterOperation operation,
        string? normalizedEmail,
        string? subject,
        string? ipAddress,
        CancellationToken cancellationToken = default);
}

public enum LimiterOperation
{
    Login,
    Registration,
    Refresh,
    Verification,
    Recovery,
    PasswordChange,
    Logout,
}

public sealed record LimiterDecision(
    bool Allowed,
    int Count,
    int Limit,
    DateTime RetryAtUtc,
    bool Degraded = false)
{
    public TimeSpan RetryAfter(DateTime nowUtc) =>
        RetryAtUtc > nowUtc ? RetryAtUtc - nowUtc : TimeSpan.Zero;
}
