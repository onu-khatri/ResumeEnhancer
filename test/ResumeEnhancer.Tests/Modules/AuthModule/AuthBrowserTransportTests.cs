using Microsoft.AspNetCore.Http;
using ResumeEnhancer.AuthModule.AM.Responses;
using ResumeEnhancer.AuthModule.SL.Options;
using ResumeEnhancer.AuthModule.Web.Authentication;
using Shouldly;

namespace ResumeEnhancer.Tests.Unit.Modules.AuthModule;

public sealed class AuthBrowserTransportTests
{
    private static readonly DateTime TestNowUtc = new(2030, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Refresh_and_csrf_cookies_are_secure_and_do_not_set_a_domain()
    {
        var context = new DefaultHttpContext();
        var options = new AuthSecurityOptions();
        var tokens = new AuthTokens("access", "refresh", TestNowUtc.AddMinutes(30), TestNowUtc.AddDays(7));

        AuthBrowserTransport.SetRefreshCookies(context.Response, tokens, options);

        var cookies = context.Response.Headers["Set-Cookie"].ToArray();
        cookies.Length.ShouldBe(2);
        cookies.Count(x => x.StartsWith("__Host-resumeenhancer-refresh=", StringComparison.Ordinal)).ShouldBe(1);
        cookies.ShouldAllBe(x => x.Contains("Path=/", StringComparison.OrdinalIgnoreCase));
        cookies.ShouldAllBe(x => x.Contains("Secure", StringComparison.OrdinalIgnoreCase));
        cookies.ShouldAllBe(x => x.Contains("SameSite=Lax", StringComparison.OrdinalIgnoreCase));
        cookies.ShouldContain(x => x.Contains("HttpOnly", StringComparison.OrdinalIgnoreCase));
        cookies.ShouldContain(x => !x.Contains("Domain=", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Cookie_mutation_proof_accepts_matching_same_origin_and_rejects_ko_variants()
    {
        var options = new AuthSecurityOptions { TrustedOrigins = ["https://resume.example"] };
        var valid = new DefaultHttpContext();
        valid.Request.Headers.Cookie = "__Host-resumeenhancer-refresh=refresh; resumeenhancer-csrf=csrf";
        valid.Request.Headers.Origin = "https://resume.example";
        valid.Request.Headers[options.CsrfHeaderName] = "csrf";

        AuthBrowserTransport.HasValidCookieMutationProof(valid.Request, options).ShouldBeTrue();

        var mismatch = new DefaultHttpContext();
        mismatch.Request.Headers.Cookie = valid.Request.Headers.Cookie;
        mismatch.Request.Headers.Origin = valid.Request.Headers.Origin;
        mismatch.Request.Headers[options.CsrfHeaderName] = "wrong";
        AuthBrowserTransport.HasValidCookieMutationProof(mismatch.Request, options).ShouldBeFalse();

        var untrusted = new DefaultHttpContext();
        untrusted.Request.Headers.Cookie = valid.Request.Headers.Cookie;
        untrusted.Request.Headers.Origin = "https://evil.example";
        untrusted.Request.Headers[options.CsrfHeaderName] = "csrf";
        AuthBrowserTransport.HasValidCookieMutationProof(untrusted.Request, options).ShouldBeFalse();
    }

    [Fact]
    public void Bearer_only_mutation_without_refresh_cookie_does_not_require_csrf_proof()
    {
        var request = new DefaultHttpContext().Request;

        AuthBrowserTransport.HasValidCookieMutationProof(request, new AuthSecurityOptions()).ShouldBeTrue();
    }

    [Fact]
    public void Refresh_transport_distinguishes_legacy_body_cookie_and_ambiguous_requests()
    {
        var options = new AuthSecurityOptions();
        var legacy = new DefaultHttpContext();
        legacy.Request.ContentType = "application/json";

        AuthBrowserTransport.ClassifyRefreshTransport(legacy.Request, options)
            .ShouldBe(AuthBrowserTransport.RefreshTransport.LegacyBody);

        var browser = new DefaultHttpContext();
        browser.Request.Headers.Cookie = $"{options.RefreshCookieName}=refresh";

        AuthBrowserTransport.ClassifyRefreshTransport(browser.Request, options)
            .ShouldBe(AuthBrowserTransport.RefreshTransport.BrowserCookie);

        var ambiguous = new DefaultHttpContext();
        ambiguous.Request.Headers.Cookie = $"{options.RefreshCookieName}=refresh";
        ambiguous.Request.ContentType = "application/json";
        ambiguous.Request.ContentLength = 1;

        AuthBrowserTransport.ClassifyRefreshTransport(ambiguous.Request, options)
            .ShouldBe(AuthBrowserTransport.RefreshTransport.Ambiguous);
    }

    [Fact]
    public async Task Chunked_or_unknown_length_body_is_read_and_classified_from_actual_bytes()
    {
        var request = new DefaultHttpContext().Request;
        request.ContentLength = null;
        request.ContentType = "application/json";
        request.Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("{\"refreshToken\":\"body-token\"}"));

        var result = await AuthBrowserTransport.ReadRefreshTransportAsync(request, new AuthSecurityOptions());

        result.Transport.ShouldBe(AuthBrowserTransport.RefreshTransport.LegacyBody);
        result.Token.ShouldBe("body-token");
    }

    [Fact]
    public async Task Empty_and_malformed_legacy_bodies_do_not_produce_refresh_tokens()
    {
        var emptyRequest = new DefaultHttpContext().Request;
        emptyRequest.Body = new MemoryStream(" \r\n "u8.ToArray());

        var empty = await AuthBrowserTransport.ReadRefreshTransportAsync(emptyRequest, new AuthSecurityOptions());

        empty.Transport.ShouldBe(AuthBrowserTransport.RefreshTransport.LegacyBody);
        empty.Token.ShouldBeNull();

        var malformedRequest = new DefaultHttpContext().Request;
        malformedRequest.Body = new MemoryStream("{not-json}"u8.ToArray());

        var malformed = await AuthBrowserTransport.ReadRefreshTransportAsync(malformedRequest, new AuthSecurityOptions());

        malformed.Transport.ShouldBe(AuthBrowserTransport.RefreshTransport.LegacyBody);
        malformed.Token.ShouldBeNull();
    }

    [Fact]
    public void Refresh_cookie_clearance_expires_both_cookies()
    {
        var context = new DefaultHttpContext();
        var options = new AuthSecurityOptions();

        AuthBrowserTransport.ClearRefreshCookies(context.Response, options);

        var cookies = context.Response.Headers["Set-Cookie"].ToArray();
        cookies.Length.ShouldBe(2);
        cookies.ShouldAllBe(cookie => cookie.Contains("expires=Thu, 01 Jan 1970", StringComparison.OrdinalIgnoreCase));
        cookies.ShouldAllBe(cookie => cookie.Contains("Max-Age=0", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Trusted_origin_matching_accepts_explicit_scheme_host_and_rejects_invalid_origin()
    {
        AuthBrowserTransport.IsTrustedOrigin(
            "https://resume.example/",
            ["", "*", "https://resume.example"])
            .ShouldBeTrue();
        AuthBrowserTransport.IsTrustedOrigin(
            "https://resume.example:444",
            ["https://resume.example"])
            .ShouldBeFalse();
        AuthBrowserTransport.IsTrustedOrigin("not-an-origin", ["https://resume.example"]).ShouldBeFalse();
        AuthBrowserTransport.IsTrustedOrigin("https://resume.example", ["https://resume.example/path"]).ShouldBeFalse();
    }
}
