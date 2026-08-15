using Microsoft.AspNetCore.Http;
using Shouldly;
using ResumeEnhancer.Core.WebLibrary.Http;

namespace ResumeEnhancer.Tests.Unit.Core.WebLibrary;

public sealed class HttpRequestHeaderReaderTests
{
    [Fact]
    public void ReadOptional_HeaderMissing_ReturnsNull()
    {
        var httpContext = new DefaultHttpContext();

        var value = HttpRequestHeaderReader.ReadOptional(httpContext, "X-Test");

        value.ShouldBeNull();
    }

    [Fact]
    public void ReadOptional_HeaderBlank_ReturnsNull()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["X-Test"] = "   ";

        var value = HttpRequestHeaderReader.ReadOptional(httpContext, "X-Test");

        value.ShouldBeNull();
    }

    [Fact]
    public void ReadOptional_HeaderHasSpaces_ReturnsTrimmedValue()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["X-Test"] = "  value  ";

        var value = HttpRequestHeaderReader.ReadOptional(httpContext, "X-Test");

        value.ShouldBe("value");
    }

    [Fact]
    public void ReadOptionalInt32_HeaderMissing_ReturnsNull()
    {
        var httpContext = new DefaultHttpContext();

        var value = HttpRequestHeaderReader.ReadOptionalInt32(httpContext, "X-Test");

        value.ShouldBeNull();
    }

    [Fact]
    public void ReadOptionalInt32_HeaderContainsInteger_ReturnsParsedValue()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["X-Test"] = " 42 ";

        var value = HttpRequestHeaderReader.ReadOptionalInt32(httpContext, "X-Test");

        value.ShouldBe(42);
    }

    [Fact]
    public void ReadOptionalInt32_HeaderContainsNonInteger_ThrowsArgumentException()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["X-Test"] = "forty-two";

        var exception = Should.Throw<ArgumentException>(
            () => HttpRequestHeaderReader.ReadOptionalInt32(httpContext, "X-Test"));

        exception.ParamName.ShouldBe("X-Test");
    }
}


