namespace ResumeEnhancer.Core.CommonLibrary.Resilience;

public sealed class ResilienceOptions
{
    public const string SectionName = "Resilience";

    public Dictionary<string, ResilienceProfileOptions> Profiles { get; set; } =
        new(StringComparer.OrdinalIgnoreCase);
}

public sealed class ResilienceProfileOptions
{
    public int RetryCount { get; set; }
    public TimeSpan BaseDelay { get; set; }
    public TimeSpan MaxDelay { get; set; }
    public TimeSpan JitterMin { get; set; }
    public TimeSpan JitterMax { get; set; }
    public TimeSpan Timeout { get; set; }
    public int CircuitBreakerFailureThreshold { get; set; }
    public TimeSpan CircuitBreakerSamplingWindow { get; set; }
    public TimeSpan CircuitBreakerBreakDuration { get; set; }
    public int ConcurrencyLimit { get; set; }
}

public static class ResilienceOptionsValidator
{
    private static readonly string[] RequiredProfiles =
        ["LimiterProvider", "DbCache", "AuditOutbox", "SafeOutboundHttp"];

    public static bool Validate(ResilienceOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        foreach (var profileName in RequiredProfiles)
        {
            if (!options.Profiles.TryGetValue(profileName, out var profile))
                throw new InvalidOperationException($"Resilience:Profiles:{profileName} is required.");

            ValidateProfile(profileName, profile);
        }

        var unknown = options.Profiles.Keys
            .Where(key => !RequiredProfiles.Contains(key, StringComparer.OrdinalIgnoreCase))
            .ToArray();
        if (unknown.Length > 0)
            throw new InvalidOperationException("Resilience:Profiles contains an unknown profile.");

        return true;
    }

    private static void ValidateProfile(string name, ResilienceProfileOptions profile)
    {
        if (profile.RetryCount is < 0 or > 5)
            throw new InvalidOperationException($"Resilience:Profiles:{name}:RetryCount is outside the permitted bounds.");
        if (profile.BaseDelay <= TimeSpan.Zero || profile.BaseDelay > TimeSpan.FromSeconds(5))
            throw new InvalidOperationException($"Resilience:Profiles:{name}:BaseDelay is outside the permitted bounds.");
        if (profile.MaxDelay < profile.BaseDelay || profile.MaxDelay > TimeSpan.FromMinutes(5))
            throw new InvalidOperationException($"Resilience:Profiles:{name}:MaxDelay is outside the permitted bounds.");
        if (profile.JitterMin < TimeSpan.Zero || profile.JitterMax < profile.JitterMin || profile.JitterMax > TimeSpan.FromSeconds(10))
            throw new InvalidOperationException($"Resilience:Profiles:{name}:JitterMin/JitterMax are outside the permitted bounds.");
        if (profile.Timeout <= TimeSpan.Zero || profile.Timeout > TimeSpan.FromSeconds(30))
            throw new InvalidOperationException($"Resilience:Profiles:{name}:Timeout is outside the permitted bounds.");
        if (profile.CircuitBreakerFailureThreshold is < 1 or > 100)
            throw new InvalidOperationException($"Resilience:Profiles:{name}:CircuitBreakerFailureThreshold is outside the permitted bounds.");
        if (profile.CircuitBreakerSamplingWindow <= TimeSpan.Zero || profile.CircuitBreakerSamplingWindow > TimeSpan.FromMinutes(10))
            throw new InvalidOperationException($"Resilience:Profiles:{name}:CircuitBreakerSamplingWindow is outside the permitted bounds.");
        if (profile.CircuitBreakerBreakDuration <= TimeSpan.Zero || profile.CircuitBreakerBreakDuration > TimeSpan.FromMinutes(10))
            throw new InvalidOperationException($"Resilience:Profiles:{name}:CircuitBreakerBreakDuration is outside the permitted bounds.");
        if (profile.ConcurrencyLimit is < 1 or > 512)
            throw new InvalidOperationException($"Resilience:Profiles:{name}:ConcurrencyLimit is outside the permitted bounds.");
    }
}
