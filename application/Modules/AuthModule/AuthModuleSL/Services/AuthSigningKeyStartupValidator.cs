using ResumeEnhancer.AuthModule.DM.Entities;
using ResumeEnhancer.AuthModule.SL.Abstractions;

namespace ResumeEnhancer.AuthModule.SL.Services;

public sealed class AuthSigningKeyStartupValidator(
    IAuthRepository repository,
    IAuthProtectedKeyMaterialStore materialStore)
{
    public async Task ValidateAsync(CancellationToken cancellationToken = default)
    {
        var metadata = await repository.GetSigningKeyMetadataAsync(cancellationToken);
        var active = metadata.Where(IsUsableActive).ToArray();
        if (active.Length != 1)
            throw new InvalidOperationException("Authentication signing-key startup validation failed: exactly one active key is required.");
        using var key = materialStore.Import(active[0]);
    }

    private static bool IsUsableActive(AuthSigningKeyMetadata key) =>
        key.IsActive && key.RetiredAtUtc is null && key.InvalidatedAtUtc is null;
}
