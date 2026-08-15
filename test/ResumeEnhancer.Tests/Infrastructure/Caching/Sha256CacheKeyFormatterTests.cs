using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Shouldly;
using ResumeEnhancer.Infrastructure.Caching;

namespace ResumeEnhancer.Tests.Unit.Infrastructure.Caching;

public sealed class Sha256CacheKeyFormatterTests
{
    [Fact]
    public void Format_KeyIsWhitespace_ThrowsArgumentException()
    {
        var formatter = CreateFormatter("cache");

        var exception = Should.Throw<ArgumentException>(() => formatter.Format("   "));

        exception.ParamName.ShouldBe("key");
    }

    [Fact]
    public void Format_PrefixHasUnsafeCharacters_SanitizesAndHashesKey()
    {
        var formatter = CreateFormatter(" Resume Enhancer! ");
        var expectedHash = Convert
            .ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes("user:1")))
            .ToLowerInvariant();

        var formatted = formatter.Format("user:1");

        formatted.ShouldBe($"Resume_Enhancer_:{expectedHash}");
    }

    [Fact]
    public void Format_PrefixIsBlank_ReturnsHashOnly()
    {
        var formatter = CreateFormatter(" ");
        var expectedHash = Convert
            .ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes("key")))
            .ToLowerInvariant();

        var formatted = formatter.Format("key");

        formatted.ShouldBe(expectedHash);
    }

    private static Sha256CacheKeyFormatter CreateFormatter(string prefix) =>
        new(Options.Create(new CacheOptions { KeyPrefix = prefix }));
}


