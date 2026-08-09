using System.Net;
using System.Net.Http.Json;
using ResumeEnhancer.TestUtilities.IntegrationSupport;
using Shouldly;

namespace ResumeEnhancer.IntegrationTests.Modules.ResumeModule;

public sealed class ResumeEndpointSetup<TRequest>
{
    public ResumeEndpointSetup(
        string description,
        HttpMethod method,
        string route,
        TRequest input,
        Func<ISetupper, ResumeEndpointSetup<TRequest>, CancellationToken, Task>? arrangeAsync,
        Func<ISetupper, HttpResponseMessage, CancellationToken, Task> assertAsync)
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

    public Func<ISetupper, ResumeEndpointSetup<TRequest>, CancellationToken, Task> ArrangeAsync { get; }

    public Func<ISetupper, HttpResponseMessage, CancellationToken, Task> AssertAsync { get; }

    public override string ToString() => Description;
}

public sealed class ResumeEndpointSetup
{
    public ResumeEndpointSetup(
        string description,
        HttpMethod method,
        string route,
        Func<ISetupper, ResumeEndpointSetup, CancellationToken, Task> arrangeAsync,
        Func<ISetupper, HttpResponseMessage, CancellationToken, Task> assertAsync)
    {
        Description = description;
        Method = method;
        Route = route;
        ArrangeAsync = arrangeAsync;
        AssertAsync = assertAsync;
    }

    public string Description { get; }

    public HttpMethod Method { get; }

    public string Route { get; set; }

    public Func<ISetupper, ResumeEndpointSetup, CancellationToken, Task> ArrangeAsync { get; }

    public Func<ISetupper, HttpResponseMessage, CancellationToken, Task> AssertAsync { get; }

    public override string ToString() => Description;
}

internal static class ResumeEndpointAssertions
{
    public static async Task<TResponse> ReadSuccessJsonAsync<TResponse>(
        this HttpResponseMessage response,
        HttpStatusCode statusCode,
        CancellationToken cancellationToken)
        where TResponse : class
    {
        response.StatusCode.ShouldBe(statusCode);
        var body = await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken);

        body.ShouldNotBeNull();

        return body;
    }
}
