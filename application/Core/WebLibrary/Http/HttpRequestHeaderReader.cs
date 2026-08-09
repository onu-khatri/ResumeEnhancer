using Microsoft.AspNetCore.Http;

namespace WebLibrary.Http;

public static class HttpRequestHeaderReader
{
    public static string? ReadOptional(HttpContext httpContext, string headerName)
    {
        if (!httpContext.Request.Headers.TryGetValue(headerName, out var values))
        {
            return null;
        }

        var value = values.FirstOrDefault()?.Trim();

        return string.IsNullOrWhiteSpace(value)
            ? null
            : value;
    }

    public static int? ReadOptionalInt32(HttpContext httpContext, string headerName)
    {
        var value = ReadOptional(httpContext, headerName);

        if (value is null)
        {
            return null;
        }

        if (int.TryParse(value, out var parsedValue))
        {
            return parsedValue;
        }

        throw new ArgumentException(
            $"{headerName} must be a valid integer.",
            headerName);
    }
}
