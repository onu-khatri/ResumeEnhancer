using System.Security.Claims;
using ResumeEnhancer.Core.WebLibrary.Authorization;
using Shouldly;

namespace ResumeEnhancer.Tests.Modules.AuthModule;

public sealed class PrincipalIdentityTests
{
    [Fact]
    public void Subject_claim_is_the_canonical_identity_and_client_headers_are_not_consulted()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim("sub", "42"),
            new Claim(ClaimTypes.NameIdentifier, "99"),
            new Claim("X-User-Id", "777"),
        ],
        "test"));

        principal.GetSubjectId().ShouldBe(42);
        principal.GetAuditActorId().ShouldBe(42);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("not-an-id")]
    public void Invalid_or_missing_subject_claim_does_not_derive_an_identity(string? value)
    {
        var claims = value is null
            ? []
            : new[] { new Claim("sub", value) };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "test"));

        principal.GetSubjectId().ShouldBeNull();
        principal.GetAuditActorId().ShouldBeNull();
    }
}
