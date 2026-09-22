using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;
using ResumeEnhancer.AuthModule.SL.Abstractions;

namespace ResumeEnhancer.AuthModule.Web.Outbox;

public sealed class AuthChallengeDeliveryProtector(IDataProtectionProvider provider) : IAuthChallengeDeliveryProtector
{
    public string Protect(string rawChallenge, string purposeCode, int userId, string email) =>
        provider.CreateProtector("ResumeEnhancer.Auth.ChallengeDelivery.v1", purposeCode)
            .Protect($"{userId}:{email}:{rawChallenge}");

    public string Unprotect(string protectedChallenge, string purposeCode, int userId, string email)
    {
        var value = provider.CreateProtector("ResumeEnhancer.Auth.ChallengeDelivery.v1", purposeCode).Unprotect(protectedChallenge);
        var prefix = $"{userId}:{email}:";
        if (!value.StartsWith(prefix, StringComparison.Ordinal))
            throw new CryptographicException("Challenge delivery envelope subject mismatch.");
        return value[prefix.Length..];
    }
}
