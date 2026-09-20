using Microsoft.AspNetCore.Http;
using System.Text.Json;
using ResumeEnhancer.AuthModule.AM.Requests;
using ResumeEnhancer.AuthModule.AM.Responses;
using ResumeEnhancer.AuthModule.SL.Options;

namespace ResumeEnhancer.AuthModule.Web.Authentication;

public static class AuthBrowserTransport
{
    public enum RefreshTransport { LegacyBody, BrowserCookie, Ambiguous }

    public sealed record RefreshTransportRead(RefreshTransport Transport, string? Token);

    public static RefreshTransport ClassifyRefreshTransport(HttpRequest request, AuthSecurityOptions options)
    {
        var hasCookie = request.Cookies.ContainsKey(options.RefreshCookieName);
        var hasBody = request.ContentLength is > 0 || request.Headers.TransferEncoding.Count > 0;
        return (hasCookie, hasBody) switch
        {
            (true, true) => RefreshTransport.Ambiguous,
            (true, false) => RefreshTransport.BrowserCookie,
            _ => RefreshTransport.LegacyBody,
        };
    }

    public static async Task<RefreshTransportRead> ReadRefreshTransportAsync(
        HttpRequest request,
        AuthSecurityOptions options,
        CancellationToken cancellationToken = default)
    {
        request.EnableBuffering();
        using var buffer = new MemoryStream();
        await request.Body.CopyToAsync(buffer, cancellationToken);
        request.Body.Position = 0;
        var bodyBytes = buffer.ToArray();
        var hasBody = bodyBytes.Any(byteValue => !char.IsWhiteSpace((char)byteValue));
        var hasCookie = request.Cookies.ContainsKey(options.RefreshCookieName);
        if (hasCookie && hasBody)
            return new RefreshTransportRead(RefreshTransport.Ambiguous, null);
        if (hasCookie)
            return new RefreshTransportRead(RefreshTransport.BrowserCookie, DecodeTransportValue(request.Cookies[options.RefreshCookieName]));
        if (!hasBody)
            return new RefreshTransportRead(RefreshTransport.LegacyBody, null);
        try
        {
            var body = JsonSerializer.Deserialize<RefreshRequest>(bodyBytes, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            return new RefreshTransportRead(RefreshTransport.LegacyBody, DecodeTransportValue(body?.RefreshToken));
        }
        catch (JsonException)
        {
            return new RefreshTransportRead(RefreshTransport.LegacyBody, null);
        }
    }

    private static string? DecodeTransportValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;
        try
        {
            return Uri.UnescapeDataString(value);
        }
        catch (UriFormatException)
        {
            return null;
        }
    }

    public static void SetRefreshCookies(HttpResponse response, AuthTokens tokens, AuthSecurityOptions options)
    {
        var refresh = new CookieOptions
        {
            HttpOnly = options.RefreshCookieHttpOnly, Secure = options.RefreshCookieSecure,
            SameSite = Enum.Parse<SameSiteMode>(options.RefreshCookieSameSite, true), Path = options.RefreshCookiePath,
            Expires = new DateTimeOffset(tokens.RefreshTokenExpiresAtUtc), IsEssential = true,
        };
        response.Cookies.Append(options.RefreshCookieName, tokens.RefreshToken, refresh);
        response.Cookies.Append(options.CsrfCookieName, Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32)), new CookieOptions
        {
            HttpOnly = false, Secure = options.RefreshCookieSecure, SameSite = refresh.SameSite, Path = options.RefreshCookiePath,
            Expires = refresh.Expires, IsEssential = true,
        });
    }

    public static void ClearRefreshCookies(HttpResponse response, AuthSecurityOptions options)
    {
        var clear = new CookieOptions
        {
            Secure = options.RefreshCookieSecure,
            SameSite = Enum.Parse<SameSiteMode>(options.RefreshCookieSameSite, true),
            Path = options.RefreshCookiePath,
            HttpOnly = options.RefreshCookieHttpOnly,
            Expires = DateTimeOffset.UnixEpoch,
            MaxAge = TimeSpan.Zero,
            IsEssential = true,
        };
        response.Cookies.Append(options.RefreshCookieName, string.Empty, clear);
        response.Cookies.Append(options.CsrfCookieName, string.Empty, new CookieOptions
        {
            Secure = clear.Secure,
            SameSite = clear.SameSite,
            Path = clear.Path,
            Expires = clear.Expires,
            MaxAge = TimeSpan.Zero,
            IsEssential = true,
        });
    }

    public static bool HasValidCookieMutationProof(HttpRequest request, AuthSecurityOptions options)
    {
        if (!request.Cookies.ContainsKey(options.RefreshCookieName)) return true;
        var origin = request.Headers.Origin.ToString();
        if (string.IsNullOrWhiteSpace(origin) || !IsTrustedOrigin(origin, options.TrustedOrigins)) return false;
        var cookie = request.Cookies[options.CsrfCookieName];
        var header = DecodeTransportValue(request.Headers[options.CsrfHeaderName].ToString());
        return !string.IsNullOrWhiteSpace(cookie) && !string.IsNullOrWhiteSpace(header) && cookie.Length == header.Length &&
            System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(System.Text.Encoding.UTF8.GetBytes(cookie), System.Text.Encoding.UTF8.GetBytes(header));
    }

    public static bool IsTrustedOrigin(string origin, IEnumerable<string> trustedOrigins)
    {
        if (!Uri.TryCreate(origin, UriKind.Absolute, out var actual) || string.IsNullOrWhiteSpace(actual.Host)) return false;
        foreach (var configured in trustedOrigins)
        {
            if (string.IsNullOrWhiteSpace(configured) || configured == "*") continue;
            var normalized = configured.Trim().TrimEnd('/');
            if (string.Equals(normalized, origin.TrimEnd('/'), StringComparison.OrdinalIgnoreCase)) return true;
            var separator = normalized.IndexOf("://", StringComparison.Ordinal);
            if (separator <= 0) continue;
            var scheme = normalized[..separator];
            var hostPattern = normalized[(separator + 3)..];
            if (string.Equals(scheme, actual.Scheme, StringComparison.OrdinalIgnoreCase)
                && string.Equals(hostPattern, actual.Host, StringComparison.OrdinalIgnoreCase)
                && actual.Port is -1 or 80 or 443) return true;
        }
        return false;
    }
}
