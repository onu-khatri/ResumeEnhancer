namespace ResumeEnhancer.AuthModule.SL.Abstractions;

public interface IPasswordHasher
{
    public string Hash(string password);
    public bool Verify(string hash, string password);
}

public interface ITokenService
{
    public (string AccessToken, DateTime ExpiresAtUtc) CreateAccessToken(
        int userId,
        Guid sessionId
    );
    public string CreateRefreshToken();
    public string HashRefreshToken(string token);
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
}
