using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Shouldly;
using ResumeEnhancer.ResumeModule.Web.MiniApis;
using ResumeEnhancer.ResumeModule.Web.Validation.Shared;

namespace ResumeEnhancer.Tests.Unit.Modules.ResumeModule.Web;

public sealed class ResumeEndpointValidationTests
{
    [Fact]
    public void BodyRequired_ReturnsRequestError()
    {
        var errors = ResumeEndpointValidation.BodyRequired();

        errors.Keys.ShouldBe(["request"]);
        errors["request"].ShouldBe(["Request body is required."]);
    }

    [Fact]
    public void ResumeId_ValidAndInvalidIds_ReturnExpectedErrors()
    {
        ResumeEndpointValidation.ResumeId(1).ShouldBeEmpty();

        var errors = ResumeEndpointValidation.ResumeId(0);

        errors.Keys.ShouldBe(["resumeId"]);
        errors["resumeId"].ShouldBe(["Resume id must be greater than 0."]);
    }

    [Fact]
    public void Merge_EmptyAndNonEmptyInputs_ReturnExpectedCopies()
    {
        var first = new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            ["a"] = ["one"]
        };
        var second = new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            ["b"] = ["two"]
        };

        var fromFirstEmpty = ResumeEndpointValidation.Merge(
            new Dictionary<string, string[]>(StringComparer.Ordinal),
            second);
        var fromSecondEmpty = ResumeEndpointValidation.Merge(
            first,
            new Dictionary<string, string[]>(StringComparer.Ordinal));

        fromFirstEmpty["b"].ShouldBe(["two"]);
        fromSecondEmpty["a"].ShouldBe(["one"]);
    }

    [Fact]
    public void Merge_OverlappingInputs_CombinesMessagesByKey()
    {
        var merged = ResumeEndpointValidation.Merge(
            new Dictionary<string, string[]>(StringComparer.Ordinal)
            {
                ["title"] = ["required"],
                ["summary"] = ["too long"]
            },
            new Dictionary<string, string[]>(StringComparer.Ordinal)
            {
                ["title"] = ["must be unique"]
            });

        merged["title"].ShouldBe(["required", "must be unique"]);
        merged["summary"].ShouldBe(["too long"]);
    }

    [Fact]
    public void ResumeEndpointHeaders_DerivesIdentityFromPrincipal()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim("sub", "7")],
            "Test"));

        ResumeEndpointHeaders.GetPrincipalAuditUserId(httpContext).ShouldBe(7);
        ResumeEndpointHeaders.GetPrincipalUserId(httpContext).ShouldBe(7);
    }
}


