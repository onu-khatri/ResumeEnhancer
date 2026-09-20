using System.Security.Cryptography;
using ResumeEnhancer.AuthModule.DM.Entities;

namespace ResumeEnhancer.AuthModule.SL.Abstractions;

public interface IAuthProtectedKeyMaterialStore
{
    RSA Import(AuthSigningKeyMetadata metadata);
    byte[] Protect(RSA key);
}
