using System.Security.Cryptography;
using ResumeEnhancer.AuthModule.DM.Entities;

namespace ResumeEnhancer.AuthModule.SL.Abstractions;

public sealed record AuthSigningKey(
    string KeyIdentifier,
    RSA Key,
    DateTime ActivatedAtUtc,
    DateTime? RetiredAtUtc,
    DateTime? InvalidatedAtUtc);

public sealed class AuthSigningKeySet(AuthSigningKey active, IReadOnlyList<AuthSigningKey> validationKeys) : IDisposable
{
    public AuthSigningKey Active { get; } = active;
    public IReadOnlyList<AuthSigningKey> ValidationKeys { get; } = validationKeys;

    public void Dispose()
    {
        var disposed = new HashSet<RSA>();
        foreach (var key in ValidationKeys.Append(Active).Select(x => x.Key))
        {
            if (disposed.Add(key))
                key.Dispose();
        }
    }
}

public interface IAuthSigningKeyProvider
{
    Task<AuthSigningKeySet?> GetValidationKeySetAsync(
        DateTime nowUtc,
        CancellationToken cancellationToken = default);

    Task<AuthSigningKeySet?> GetIssuanceKeySetAsync(
        DateTime nowUtc,
        CancellationToken cancellationToken = default);

    Task InvalidateAsync(string keyIdentifier, DateTime nowUtc, CancellationToken cancellationToken = default);
}
