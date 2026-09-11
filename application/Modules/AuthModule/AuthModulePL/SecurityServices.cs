using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using ResumeEnhancer.AuthModule.SL.Abstractions;

namespace ResumeEnhancer.AuthModule.PL;

public sealed class AuthPasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 210_000, HashAlgorithmName.SHA512, 32);
        return $"pbkdf2-sha512$210000${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public bool Verify(string encoded, string password)
    {
        var p = encoded.Split('$');
        if (p.Length != 4 || p[0] != "pbkdf2-sha512" || !int.TryParse(p[1], out var iterations))
            return false;
        try
        {
            var actual = Rfc2898DeriveBytes.Pbkdf2(
                password,
                Convert.FromBase64String(p[2]),
                iterations,
                HashAlgorithmName.SHA512,
                32
            );
            return CryptographicOperations.FixedTimeEquals(actual, Convert.FromBase64String(p[3]));
        }
        catch (FormatException)
        {
            return false;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
    }
}

public sealed class AuthTokenService(IConfiguration configuration) : ITokenService
{
    private readonly byte[] key = LoadSigningKey(configuration);

    public string CreateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
    }

    public string HashRefreshToken(string token)
    {
        return Convert
            .ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)))
            .ToLowerInvariant();
    }

    public (string AccessToken, DateTime ExpiresAtUtc) CreateAccessToken(int userId, Guid sessionId)
    {
        var expiry = DateTime.UtcNow.AddMinutes(15);
        var body = $"{userId}:{sessionId:N}:{new DateTimeOffset(expiry):O}";
        var sig = Convert.ToBase64String(HMACSHA256.HashData(key, Encoding.UTF8.GetBytes(body)));
        return ($"v1.{Convert.ToBase64String(Encoding.UTF8.GetBytes(body))}.{sig}", expiry);
    }

    private static byte[] LoadSigningKey(IConfiguration configuration)
    {
        var value = configuration["Auth:SigningKey"];
        if (string.IsNullOrWhiteSpace(value) || Encoding.UTF8.GetByteCount(value) < 32)
            throw new InvalidOperationException(
                "Auth:SigningKey must be configured with at least 32 bytes."
            );
        return Encoding.UTF8.GetBytes(value);
    }
}

public sealed class InMemoryRegistrationThrottle : IRegistrationThrottle
{
    private static readonly TimeSpan Window = TimeSpan.FromMinutes(15);
    private const int Limit = 5;
    private readonly ConcurrentDictionary<string, ConcurrentQueue<DateTime>> attempts = new(
        StringComparer.Ordinal
    );

    public Task<bool> IsAllowedAsync(
        string normalizedEmail,
        string? ipAddress,
        CancellationToken cancellationToken = default
    )
    {
        var now = DateTime.UtcNow;
        var allowed = true;
        foreach (var key in new[] { $"email:{normalizedEmail}", $"ip:{ipAddress ?? "unknown"}" })
        {
            var queue = attempts.GetOrAdd(key, _ => new ConcurrentQueue<DateTime>());
            while (queue.TryPeek(out var timestamp) && now - timestamp > Window)
                queue.TryDequeue(out _);
            if (queue.Count >= Limit)
                allowed = false;
            else
                queue.Enqueue(now);
        }
        return Task.FromResult(allowed);
    }

    public void Reset() => attempts.Clear();
}
