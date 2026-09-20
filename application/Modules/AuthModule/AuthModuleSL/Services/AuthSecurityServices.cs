using System.Collections.Concurrent;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using ResumeEnhancer.AuthModule.SL.Abstractions;
using ResumeEnhancer.AuthModule.SL.Options;

namespace ResumeEnhancer.AuthModule.SL.Services;

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
        if (p.Length != 4 || p[0] != "pbkdf2-sha512" || !int.TryParse(p[1], out var iterations)) return false;
        try
        {
            var actual = Rfc2898DeriveBytes.Pbkdf2(password, Convert.FromBase64String(p[2]), iterations, HashAlgorithmName.SHA512, 32);
            return CryptographicOperations.FixedTimeEquals(actual, Convert.FromBase64String(p[3]));
        }
        catch (Exception) { return false; }
    }
}

public sealed class AuthTokenService : ITokenService
{
    private const string SessionClaim = "sid";
    private readonly AuthSecurityOptions options;
    private readonly IAuthSigningKeyProvider keyProvider;
    private readonly TimeProvider timeProvider;

    public AuthTokenService(AuthSecurityOptions options, IAuthSigningKeyProvider keyProvider, TimeProvider? timeProvider = null)
    {
        ArgumentNullException.ThrowIfNull(keyProvider);
        this.options = options;
        this.keyProvider = keyProvider;
        this.timeProvider = timeProvider ?? TimeProvider.System;
        AuthSecurityOptionsValidator.Validate(options);
    }

    public string CreateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
    public string HashRefreshToken(string token) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token))).ToLowerInvariant();

    public async Task<(string AccessToken, DateTime ExpiresAtUtc)> CreateAccessTokenAsync(int userId, Guid sessionId, CancellationToken cancellationToken = default)
    {
        if (userId <= 0 || sessionId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(userId));
        var now = timeProvider.GetUtcNow().UtcDateTime;
        using var keySet = await keyProvider.GetIssuanceKeySetAsync(now, cancellationToken)
            ?? throw new InvalidOperationException("Authentication signing-key state is unavailable.");
        return CreateToken(keySet.Active.Key, keySet.Active.KeyIdentifier, userId, sessionId, now);
    }

    public async Task<ClaimsPrincipal?> ValidateAccessTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token)) return null;
        try
        {
            using var keySet = await keyProvider.GetValidationKeySetAsync(timeProvider.GetUtcNow().UtcDateTime, cancellationToken);
            if (keySet is null) return null;
            return await ValidateAsync(
                token,
                keySet.ValidationKeys.Select(CreateSecurityKey).ToArray());
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
        catch (Exception) { return null; }
    }

    public Task InvalidateKeyAsync(string keyIdentifier, CancellationToken cancellationToken = default) =>
        keyProvider.InvalidateAsync(keyIdentifier, timeProvider.GetUtcNow().UtcDateTime, cancellationToken);

    private async Task<ClaimsPrincipal?> ValidateAsync(string token, IReadOnlyCollection<SecurityKey> keys)
    {
        if (string.IsNullOrWhiteSpace(token)) return null;
        try
        {
            var result = await new JsonWebTokenHandler().ValidateTokenAsync(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true, IssuerSigningKeys = keys, ValidateIssuer = true, ValidIssuer = options.Issuer,
                ValidateAudience = true, ValidAudience = options.Audience, ValidateLifetime = true, ClockSkew = options.ClockSkew,
                RequireSignedTokens = true, ValidAlgorithms = [SecurityAlgorithms.RsaSha256], RequireExpirationTime = true,
                NameClaimType = JwtRegisteredClaimNames.Sub,
            });
            if (!result.IsValid || result.SecurityToken is not JsonWebToken jwt || jwt.Alg != SecurityAlgorithms.RsaSha256 || string.IsNullOrWhiteSpace(jwt.Kid)) return null;
            var claims = jwt.Claims.Select(x => x.Type).ToHashSet(StringComparer.Ordinal);
            if (!new[] { JwtRegisteredClaimNames.Iss, JwtRegisteredClaimNames.Aud, JwtRegisteredClaimNames.Sub, JwtRegisteredClaimNames.Iat, JwtRegisteredClaimNames.Nbf, JwtRegisteredClaimNames.Exp, SessionClaim }.All(claims.Contains)) return null;
            if (!HasValidIssuedAt(jwt)) return null;
            if (!IsCanonicalPositiveSubject(jwt.Subject) || !Guid.TryParseExact(jwt.GetClaim(SessionClaim)?.Value, "N", out _)) return null;
            return result.ClaimsIdentity is null ? null : new ClaimsPrincipal(result.ClaimsIdentity);
        }
        catch (Exception) { return null; }
    }

    private (string AccessToken, DateTime ExpiresAtUtc) CreateToken(RSA key, string keyId, int userId, Guid sessionId, DateTime now)
    {
        var expiry = now.Add(options.AccessTokenLifetime);
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = options.Issuer, Audience = options.Audience, NotBefore = now, IssuedAt = now, Expires = expiry,
            Subject = new ClaimsIdentity([new Claim(JwtRegisteredClaimNames.Sub, userId.ToString(System.Globalization.CultureInfo.InvariantCulture)), new Claim(SessionClaim, sessionId.ToString("N"))]),
            SigningCredentials = new SigningCredentials(CreateSecurityKey(key, keyId), SecurityAlgorithms.RsaSha256),
        };
        return (new JsonWebTokenHandler().CreateToken(descriptor), expiry);
    }

    private static bool IsCanonicalPositiveSubject(string? subject) =>
        !string.IsNullOrEmpty(subject) && int.TryParse(subject, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var id) && id > 0 && id.ToString(System.Globalization.CultureInfo.InvariantCulture) == subject;

    private static RsaSecurityKey CreateSecurityKey(AuthSigningKey key) =>
        CreateSecurityKey(key.Key, key.KeyIdentifier);

    private static RsaSecurityKey CreateSecurityKey(RSA key, string keyIdentifier) => new(key)
    {
        KeyId = keyIdentifier,
        CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false },
    };

    private bool HasValidIssuedAt(JsonWebToken token)
    {
        var value = token.GetClaim(JwtRegisteredClaimNames.Iat)?.Value;
        if (!long.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var seconds))
            return false;
        var issuedAt = DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime;
        return issuedAt <= timeProvider.GetUtcNow().UtcDateTime.Add(options.ClockSkew);
    }

}

public sealed class InMemoryRegistrationThrottle : IRegistrationThrottle
{
    private static readonly TimeSpan Window = TimeSpan.FromMinutes(15); private const int Limit = 5;
    private readonly ConcurrentDictionary<string, ConcurrentQueue<DateTime>> attempts = new(StringComparer.Ordinal);
    private readonly TimeProvider timeProvider;

    public InMemoryRegistrationThrottle(TimeProvider? timeProvider = null) => this.timeProvider = timeProvider ?? TimeProvider.System;
    public Task<bool> IsAllowedAsync(string normalizedEmail, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime; var allowed = true;
        foreach (var key in new[] { $"email:{normalizedEmail}", $"ip:{ipAddress ?? "unknown"}" })
        {
            var queue = attempts.GetOrAdd(key, _ => new ConcurrentQueue<DateTime>());
            while (queue.TryPeek(out var timestamp) && now - timestamp > Window) queue.TryDequeue(out _);
            if (queue.Count >= Limit) allowed = false; else queue.Enqueue(now);
        }
        return Task.FromResult(allowed);
    }
    public void Reset() => attempts.Clear();

    public Task<LimiterDecision> TryConsumeAsync(
        LimiterOperation operation,
        string? normalizedEmail,
        string? subject,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var key = string.Join('|', operation, normalizedEmail ?? string.Empty, subject ?? string.Empty, ipAddress ?? "unknown");
        var queue = attempts.GetOrAdd(key, _ => new ConcurrentQueue<DateTime>());
        while (queue.TryPeek(out var timestamp) && now - timestamp > Window) queue.TryDequeue(out _);
        if (queue.Count >= Limit)
            return Task.FromResult(new LimiterDecision(false, queue.Count, Limit, queue.TryPeek(out var first) ? first.Add(Window) : now.Add(Window)));
        queue.Enqueue(now);
        return Task.FromResult(new LimiterDecision(true, queue.Count, Limit, now.Add(Window)));
    }
}
