using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Shouldly;

namespace ResumeEnhancer.TestUtilities.IntegrationSupport;

public sealed class EndpointSetup<TRequest>
{
    public EndpointSetup(
        string description,
        HttpMethod method,
        string route,
        TRequest input,
        Func<ISetupper, EndpointSetup<TRequest>, CancellationToken, Task>? arrangeAsync,
        Func<ISetupper, HttpResponseMessage, CancellationToken, Task> assertAsync
    )
    {
        Description = description;
        Method = method;
        Route = route;
        Input = input;
        ArrangeAsync = arrangeAsync ?? ((_, _, _) => Task.CompletedTask);
        AssertAsync = assertAsync;
    }

    public string Description { get; }
    public HttpMethod Method { get; }
    public string Route { get; set; }
    public TRequest Input { get; }
    public Func<ISetupper, EndpointSetup<TRequest>, CancellationToken, Task> ArrangeAsync { get; }
    public Func<ISetupper, HttpResponseMessage, CancellationToken, Task> AssertAsync { get; }

    public override string ToString() => Description;
}

public sealed class EndpointSetup
{
    public EndpointSetup(
        string description,
        HttpMethod method,
        string route,
        Func<ISetupper, EndpointSetup, CancellationToken, Task>? arrangeAsync,
        Func<ISetupper, HttpResponseMessage, CancellationToken, Task> assertAsync
    )
    {
        Description = description;
        Method = method;
        Route = route;
        ArrangeAsync = arrangeAsync ?? ((_, _, _) => Task.CompletedTask);
        AssertAsync = assertAsync;
    }

    public string Description { get; }
    public HttpMethod Method { get; }
    public string Route { get; set; }
    public Func<ISetupper, EndpointSetup, CancellationToken, Task> ArrangeAsync { get; }
    public Func<ISetupper, HttpResponseMessage, CancellationToken, Task> AssertAsync { get; }

    public override string ToString() => Description;
}

public static class EndpointAssertions
{
    public static async Task<TResponse> ReadSuccessJsonAsync<TResponse>(
        this HttpResponseMessage response,
        HttpStatusCode expectedStatus,
        CancellationToken cancellationToken
    )
        where TResponse : class
    {
        response.StatusCode.ShouldBe(
            expectedStatus,
            await response.Content.ReadAsStringAsync(cancellationToken)
        );
        var body = await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken);
        body.ShouldNotBeNull();
        return body;
    }

    public static async Task<JsonDocument> ReadJsonAsync(
        this HttpResponseMessage response,
        HttpStatusCode expectedStatus,
        CancellationToken cancellationToken
    )
    {
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        response.StatusCode.ShouldBe(expectedStatus, body);
        return JsonDocument.Parse(body);
    }
}
