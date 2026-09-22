using System.Security.Cryptography;
using ResumeEnhancer.AuthModule.DM.Entities;
using ResumeEnhancer.AuthModule.SL.Abstractions;
using ResumeEnhancer.AuthModule.SL.Options;

namespace ResumeEnhancer.AuthModule.SL.Services;

public sealed class AuthSigningKeyProvider(
    IAuthRepository repository,
    IAuthProtectedKeyMaterialStore materialStore,
    AuthSecurityOptions options) : IAuthSigningKeyProvider
{
    public Task<AuthSigningKeySet?> GetValidationKeySetAsync(DateTime nowUtc, CancellationToken cancellationToken = default) =>
        LoadKeySetAsync(nowUtc, cancellationToken);

    public async Task<AuthSigningKeySet?> GetIssuanceKeySetAsync(DateTime nowUtc, CancellationToken cancellationToken = default)
    {
        var metadata = await repository.GetSigningKeyMetadataAsync(cancellationToken);
        var active = RequireActive(metadata);
        if (nowUtc < active.ActivatedAtUtc.Add(options.KeyRotationPeriod))
            return await MaterializeAsync(metadata, nowUtc, cancellationToken);

        using var candidate = RSA.Create(2048);
        var replacement = new AuthSigningKeyMetadata
        {
            KeyIdentifier = $"{active.KeyIdentifier}-{Guid.NewGuid():N}",
            ProtectedMaterial = materialStore.Protect(candidate),
            ActivatedAtUtc = nowUtc,
            LifecycleVersion = active.LifecycleVersion + 1,
            IsActive = true,
        };
        await repository.TryRotateSigningKeyAsync(
            active.KeyIdentifier,
            active.LifecycleVersion,
            replacement,
            nowUtc,
            cancellationToken);
        // Whether this writer won or lost, only a fresh durable read is authoritative.
        // Never publish the candidate after a compare-and-transition miss.
        var committed = await repository.GetSigningKeyMetadataAsync(cancellationToken);
        return await MaterializeAsync(committed, nowUtc, cancellationToken);
    }

    public Task InvalidateAsync(string keyIdentifier, DateTime nowUtc, CancellationToken cancellationToken = default) =>
        repository.InvalidateSigningKeyAsync(keyIdentifier, nowUtc, cancellationToken);

    private async Task<AuthSigningKeySet?> LoadKeySetAsync(DateTime nowUtc, CancellationToken cancellationToken)
    {
        var metadata = await repository.GetSigningKeyMetadataAsync(cancellationToken);
        return await MaterializeAsync(metadata, nowUtc, cancellationToken);
    }

    private Task<AuthSigningKeySet?> MaterializeAsync(
        IReadOnlyList<AuthSigningKeyMetadata> metadata,
        DateTime nowUtc,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var active = RequireActive(metadata);
        var materialized = new List<AuthSigningKey>();
        try
        {
            var activeKey = new AuthSigningKey(active.KeyIdentifier, materialStore.Import(active), active.ActivatedAtUtc, null, null);
            materialized.Add(activeKey);
            var previous = metadata
                .Where(x => !x.IsActive && x.RetiredAtUtc is not null && x.InvalidatedAtUtc is null)
                .OrderByDescending(x => x.RetiredAtUtc)
                .FirstOrDefault();
            if (previous?.RetiredAtUtc is not null && nowUtc < previous.RetiredAtUtc.Value.Add(options.PreviousKeyOverlap))
                materialized.Add(new AuthSigningKey(previous.KeyIdentifier, materialStore.Import(previous), previous.ActivatedAtUtc, previous.RetiredAtUtc, null));
            return Task.FromResult<AuthSigningKeySet?>(new AuthSigningKeySet(materialized[0], materialized));
        }
        catch
        {
            foreach (var key in materialized)
                key.Key.Dispose();
            throw;
        }
    }

    private static AuthSigningKeyMetadata RequireActive(IReadOnlyList<AuthSigningKeyMetadata> metadata)
    {
        var active = metadata.Where(x => x.IsActive && x.RetiredAtUtc is null && x.InvalidatedAtUtc is null).ToArray();
        if (active.Length != 1)
            throw new InvalidOperationException("Authentication signing-key state must contain exactly one active key.");
        return active[0];
    }
}
