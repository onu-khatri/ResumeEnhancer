using System.Security.Cryptography;

namespace ResumeEnhancer.AuthModule.SL.Options;

public sealed class AuthSecurityOptions
{
    public const string SectionName = "Auth:Security";

    public int LockoutThreshold { get; set; } = 5;
    public TimeSpan LockoutWindow { get; set; } = TimeSpan.FromMinutes(15);
    public TimeSpan[] ProgressiveLoginDelays { get; set; } =
    [
        TimeSpan.Zero,
        TimeSpan.FromSeconds(1),
        TimeSpan.FromSeconds(2),
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(15),
    ];

    public string Issuer { get; set; } = "ResumeEnhancer";
    public string Audience { get; set; } = "ResumeEnhancer.Api";
    public TimeSpan AccessTokenLifetime { get; set; } = TimeSpan.FromMinutes(30);
    public TimeSpan ClockSkew { get; set; } = TimeSpan.FromSeconds(30);
    public TimeSpan KeyRotationPeriod { get; set; } = TimeSpan.FromDays(180);
    public TimeSpan PreviousKeyOverlap { get; set; } = TimeSpan.FromHours(48);
    public string ActiveKeyId { get; set; } = "primary";
    public string DataProtectionKeyRingPath { get; set; } = string.Empty;
    public string DataProtectionApplicationName { get; set; } = "ResumeEnhancer";
    public string DataProtectionPurpose { get; set; } = "AuthModule.SigningKeyMaterial.v1";
    public string RefreshCookieName { get; set; } = "__Host-resumeenhancer-refresh";
    public string RefreshCookiePath { get; set; } = "/";
    public bool RefreshCookieHttpOnly { get; set; } = true;
    public bool RefreshCookieSecure { get; set; } = true;
    public string RefreshCookieSameSite { get; set; } = "Lax";
    public string CsrfCookieName { get; set; } = "resumeenhancer-csrf";
    public string CsrfHeaderName { get; set; } = "X-CSRF-Token";
    public string[] TrustedOrigins { get; set; } = [];

}

public static class AuthSecurityOptionsValidator
{
    public static void Validate(AuthSecurityOptions options, bool requirePersistedKeyRing = false)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (string.IsNullOrWhiteSpace(options.Issuer) || string.IsNullOrWhiteSpace(options.Audience))
            throw new InvalidOperationException("Auth:Security:Issuer and Audience are required.");
        if (options.AccessTokenLifetime <= TimeSpan.Zero || options.AccessTokenLifetime > TimeSpan.FromHours(1))
            throw new InvalidOperationException("Auth:Security:AccessTokenLifetime must be positive and no longer than one hour.");
        if (options.ClockSkew < TimeSpan.Zero || options.ClockSkew > TimeSpan.FromMinutes(5))
            throw new InvalidOperationException("Auth:Security:ClockSkew must be between zero and five minutes.");
        if (options.KeyRotationPeriod <= TimeSpan.Zero || options.PreviousKeyOverlap <= TimeSpan.Zero || options.PreviousKeyOverlap > options.KeyRotationPeriod)
            throw new InvalidOperationException("Auth:Security key rotation periods are invalid.");
        if (string.IsNullOrWhiteSpace(options.ActiveKeyId) || options.ActiveKeyId.Length > 128)
            throw new InvalidOperationException("Auth:Security:ActiveKeyId is required and must be at most 128 characters.");
        if (requirePersistedKeyRing && string.IsNullOrWhiteSpace(options.DataProtectionKeyRingPath))
            throw new InvalidOperationException("Auth:Security:DataProtectionKeyRingPath is required.");
        if (string.IsNullOrWhiteSpace(options.DataProtectionApplicationName) || string.IsNullOrWhiteSpace(options.DataProtectionPurpose))
            throw new InvalidOperationException("Auth:Security Data Protection application and purpose are required.");
        if (requirePersistedKeyRing && !Directory.Exists(options.DataProtectionKeyRingPath))
            throw new InvalidOperationException("Auth:Security:DataProtectionKeyRingPath must be an existing persisted key-ring directory.");
        if (!options.RefreshCookieName.StartsWith("__Host-", StringComparison.Ordinal) || options.RefreshCookiePath != "/" || !options.RefreshCookieHttpOnly || !options.RefreshCookieSecure)
            throw new InvalidOperationException("Auth:Security refresh cookies must be __Host-, HttpOnly, Secure, and Path=/.");
        if (!string.Equals(options.RefreshCookieSameSite, "Lax", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Auth:Security:RefreshCookieSameSite must be Lax.");
        if (options.TrustedOrigins.Any(string.IsNullOrWhiteSpace) || options.TrustedOrigins.Any(x => x.Contains('*', StringComparison.Ordinal)))
            throw new InvalidOperationException("Auth:Security:TrustedOrigins must be explicit origins and cannot contain wildcards.");
        foreach (var origin in options.TrustedOrigins)
        {
            var separator = origin.IndexOf("://", StringComparison.Ordinal);
            if (separator <= 0 || origin[(separator + 3)..].Contains('/'))
                throw new InvalidOperationException("Auth:Security:TrustedOrigins must contain origin patterns only.");
        }
        if (options.LockoutThreshold < 1 || options.LockoutThreshold > 100)
            throw new InvalidOperationException("Auth:Security:LockoutThreshold must be between 1 and 100.");
        if (options.LockoutWindow <= TimeSpan.Zero || options.LockoutWindow > TimeSpan.FromDays(1))
            throw new InvalidOperationException("Auth:Security:LockoutWindow must be greater than zero and no longer than one day.");
        if (options.ProgressiveLoginDelays is null || options.ProgressiveLoginDelays.Length == 0)
        {
            options.ProgressiveLoginDelays =
            [
                TimeSpan.Zero,
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(15),
            ];
        }
        else if (options.ProgressiveLoginDelays.Length != 5)
            throw new InvalidOperationException("Auth:Security:ProgressiveLoginDelays must contain exactly five values.");
        if (options.ProgressiveLoginDelays.Any(delay => delay < TimeSpan.Zero || delay > TimeSpan.FromHours(1)))
            throw new InvalidOperationException("Auth:Security:ProgressiveLoginDelays must contain values from zero through one hour.");
        if (options.ProgressiveLoginDelays.Zip(options.ProgressiveLoginDelays.Skip(1)).Any(pair => pair.First > pair.Second))
            throw new InvalidOperationException("Auth:Security:ProgressiveLoginDelays must be non-decreasing.");
    }
}
