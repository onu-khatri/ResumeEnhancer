using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace ResumeEnhancer.Tests.Unit.TestInfrastructure;

internal static class ResultAssertionHelpers
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly IServiceProvider RequestServices = new ServiceCollection()
        .AddLogging()
        .AddOptions()
        .AddProblemDetails()
        .BuildServiceProvider();

    public static async Task<ResultSnapshot> ExecuteAsync(this IResult result)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.RequestServices = RequestServices;
        httpContext.Response.Body = new MemoryStream();

        await result.ExecuteAsync(httpContext);

        httpContext.Response.Body.Position = 0;
        using var reader = new StreamReader(httpContext.Response.Body);
        var body = await reader.ReadToEndAsync();

        return new ResultSnapshot(httpContext.Response.StatusCode, body);
    }

    public static JsonElement ReadJson(this ResultSnapshot snapshot)
    {
        snapshot.Body.ShouldNotBeNullOrWhiteSpace();

        return JsonSerializer.Deserialize<JsonElement>(snapshot.Body, JsonOptions);
    }
}

internal sealed record ResultSnapshot(int StatusCode, string Body);

