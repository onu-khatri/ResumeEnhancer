using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;
using ResumeEnhancer.AuthModule.DM.Entities;
using ResumeEnhancer.AuthModule.SL.Abstractions;
using ResumeEnhancer.AuthModule.SL.Options;

namespace ResumeEnhancer.AuthModule.Web;

public sealed class AuthProtectedKeyMaterialStore(IDataProtectionProvider provider, AuthSecurityOptions options) : IAuthProtectedKeyMaterialStore
{
    private readonly IDataProtector protector = provider.CreateProtector(options.DataProtectionPurpose);

    public RSA Import(AuthSigningKeyMetadata metadata)
    {
        if (metadata.ProtectedMaterial.Length == 0)
            throw new InvalidOperationException($"Signing key '{metadata.KeyIdentifier}' has no protected material.");
        RSA? rsa = null;
        try
        {
            var pkcs8 = protector.Unprotect(metadata.ProtectedMaterial);
            rsa = RSA.Create();
            rsa.ImportPkcs8PrivateKey(pkcs8, out _);
            if (rsa.KeySize < 2048)
            {
                rsa.Dispose();
                rsa = null;
                throw new InvalidOperationException("The signing key is weaker than the required minimum.");
            }
            return rsa;
        }
        catch (Exception exception) when (exception is CryptographicException or ArgumentException or InvalidOperationException)
        {
            rsa?.Dispose();
            throw new InvalidOperationException($"Signing key '{metadata.KeyIdentifier}' is unavailable.", exception);
        }
    }

    public byte[] Protect(RSA key)
    {
        ArgumentNullException.ThrowIfNull(key);
        return protector.Protect(key.ExportPkcs8PrivateKey());
    }
}
