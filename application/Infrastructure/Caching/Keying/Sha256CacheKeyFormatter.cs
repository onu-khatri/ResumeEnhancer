using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace Caching;

internal sealed class Sha256CacheKeyFormatter(IOptions<CacheOptions> options) : ICacheKeyFormatter
{
    public string Format(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Cache key cannot be empty.", nameof(key));
        }

        var prefix = SanitizePrefix(options.Value.KeyPrefix);
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(key))).ToLowerInvariant();

        return string.IsNullOrWhiteSpace(prefix)
            ? hash
            : $"{prefix}:{hash}";
    }

    private static string SanitizePrefix(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var builder = new StringBuilder(value.Length);

        foreach (var character in value.Trim())
        {
            builder.Append(IsSafeKeyCharacter(character) ? character : '_');
        }

        return builder.ToString();
    }

    private static bool IsSafeKeyCharacter(char character) =>
        char.IsAsciiLetterOrDigit(character)
        || character is ':' or '_' or '-' or '.';
}
