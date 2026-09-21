using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly.CircuitBreaker;
using Polly.Timeout;
using ResumeEnhancer.Core.CommonLibrary.Resilience;

namespace ResumeEnhancer.Tests.Unit.Core.CommonLibrary;

public sealed class ResilienceCoverageTests
{
    [Fact]
    public void Validator_accepts_required_profiles_and_rejects_missing_or_unknown_profiles()
    {
        var options = CreateOptions();

        Assert.True(ResilienceOptionsValidator.Validate(options));

        var missing = CreateOptions();
        missing.Profiles.Remove("AuditOutbox");
        Assert.Throws<InvalidOperationException>(() => ResilienceOptionsValidator.Validate(missing));

        var unknown = CreateOptions();
        unknown.Profiles["Unexpected"] = CreateProfile();
        Assert.Throws<InvalidOperationException>(() => ResilienceOptionsValidator.Validate(unknown));
    }

    [Fact]
    public void Validator_rejects_each_profile_boundary_that_would_make_a_pipeline_unsafe()
    {
        var invalidOptions = new List<Action<ResilienceProfileOptions>>
        {
            profile => profile.RetryCount = -1,
            profile => profile.BaseDelay = TimeSpan.Zero,
            profile => profile.MaxDelay = TimeSpan.Zero,
            profile => profile.JitterMin = TimeSpan.FromSeconds(2),
            profile => profile.Timeout = TimeSpan.FromSeconds(31),
            profile => profile.CircuitBreakerFailureThreshold = 0,
            profile => profile.CircuitBreakerSamplingWindow = TimeSpan.Zero,
            profile => profile.CircuitBreakerBreakDuration = TimeSpan.FromMinutes(11),
            profile => profile.ConcurrencyLimit = 0,
        };

        foreach (var mutate in invalidOptions)
        {
            var options = CreateOptions();
            mutate(options.Profiles["LimiterProvider"]);
            Assert.Throws<InvalidOperationException>(() => ResilienceOptionsValidator.Validate(options));
        }
    }

    [Fact]
    public void Validator_accepts_the_configured_profile_boundaries()
    {
        var options = CreateOptions();
        foreach (var profile in options.Profiles.Values)
        {
            profile.RetryCount = 5;
            profile.BaseDelay = TimeSpan.FromSeconds(5);
            profile.MaxDelay = TimeSpan.FromMinutes(5);
            profile.JitterMin = TimeSpan.FromSeconds(10);
            profile.JitterMax = TimeSpan.FromSeconds(10);
            profile.Timeout = TimeSpan.FromSeconds(30);
            profile.CircuitBreakerFailureThreshold = 100;
            profile.CircuitBreakerSamplingWindow = TimeSpan.FromMinutes(10);
            profile.CircuitBreakerBreakDuration = TimeSpan.FromMinutes(10);
            profile.ConcurrencyLimit = 512;
        }

        Assert.True(ResilienceOptionsValidator.Validate(options));
    }

    [Fact]
    public void Validator_rejects_null_options()
    {
        Assert.Throws<ArgumentNullException>(() => ResilienceOptionsValidator.Validate(null!));
    }

    [Fact]
    public async Task Executor_retries_retryable_failures_but_does_not_retry_non_idempotent_mutations()
    {
        using var provider = BuildProvider(retryCount: 1);
        var executor = provider.GetRequiredService<IResilienceExecutor>();
        var attempts = 0;

        var value = await executor.ExecuteAsync(
            "LimiterProvider",
            ResilienceOperation.Provider,
            _ =>
            {
                attempts++;
                return attempts == 1
                    ? Task.FromException<int>(new InvalidOperationException("transient"))
                    : Task.FromResult(42);
            });

        Assert.Equal(42, value);
        Assert.Equal(2, attempts);

        attempts = 0;
        await Assert.ThrowsAsync<InvalidOperationException>(() => executor.ExecuteAsync(
            "LimiterProvider",
            ResilienceOperation.NonIdempotentMutation,
            _ =>
            {
                attempts++;
                return Task.FromException<int>(new InvalidOperationException("must-not-retry"));
            }));
        Assert.Equal(1, attempts);
    }

    [Fact]
    public async Task Executor_honors_retry_predicate_and_cancellation_before_pipeline_execution()
    {
        using var provider = BuildProvider(retryCount: 1);
        var executor = provider.GetRequiredService<IResilienceExecutor>();
        var attempts = 0;

        await Assert.ThrowsAsync<InvalidOperationException>(() => executor.ExecuteAsync(
            "LimiterProvider",
            ResilienceOperation.Provider,
            _ =>
            {
                attempts++;
                return Task.FromException<int>(new InvalidOperationException("not-retryable"));
            },
            _ => false));
        Assert.Equal(1, attempts);

        using var cancelled = new CancellationTokenSource();
        cancelled.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => executor.ExecuteAsync(
            "LimiterProvider",
            ResilienceOperation.Provider,
            _ => Task.FromResult(1),
            cancellationToken: cancelled.Token));
    }

    [Fact]
    public async Task Executor_validates_profile_and_callback_arguments()
    {
        using var provider = BuildProvider(retryCount: 0);
        var executor = provider.GetRequiredService<IResilienceExecutor>();

        await Assert.ThrowsAsync<ArgumentException>(() => executor.ExecuteAsync(
            " ", ResilienceOperation.Provider, _ => Task.FromResult(1)));
        await Assert.ThrowsAsync<ArgumentNullException>(() => executor.ExecuteAsync<int>(
            "LimiterProvider", ResilienceOperation.Provider, null!));
    }

    [Fact]
    public async Task Executor_applies_timeout_and_preserves_cancellation()
    {
        using var provider = BuildProvider(retryCount: 1, timeout: TimeSpan.FromMilliseconds(10));
        var executor = provider.GetRequiredService<IResilienceExecutor>();

        await Assert.ThrowsAsync<TimeoutRejectedException>(() => executor.ExecuteAsync(
            "LimiterProvider",
            ResilienceOperation.Provider,
            async token =>
            {
                await Task.Delay(Timeout.InfiniteTimeSpan, token);
                return 1;
            }));

        using var cancelled = new CancellationTokenSource();
        var callbackStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var operation = executor.ExecuteAsync(
            "LimiterProvider",
            ResilienceOperation.Provider,
            async token =>
            {
                callbackStarted.SetResult();
                await Task.Delay(Timeout.InfiniteTimeSpan, token);
                return 1;
            },
            cancellationToken: cancelled.Token);
        await callbackStarted.Task;
        cancelled.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => operation);
    }

    [Fact]
    public async Task Executor_uses_jittered_backoff_and_opens_the_circuit_after_failure_threshold()
    {
        using var provider = BuildProvider(
            retryCount: 1,
            baseDelay: TimeSpan.FromMilliseconds(1),
            maxDelay: TimeSpan.FromMilliseconds(10),
            jitterMin: TimeSpan.FromMilliseconds(1),
            jitterMax: TimeSpan.FromMilliseconds(2),
            circuitBreakerFailureThreshold: 2,
            circuitBreakerBreakDuration: TimeSpan.FromSeconds(1));
        var executor = provider.GetRequiredService<IResilienceExecutor>();
        var attempts = 0;

        await Assert.ThrowsAsync<InvalidOperationException>(() => executor.ExecuteAsync(
            "LimiterProvider",
            ResilienceOperation.Provider,
            _ =>
            {
                attempts++;
                return Task.FromException<int>(new InvalidOperationException("dependency-down"));
            }));

        Assert.Equal(2, attempts);
        await Assert.ThrowsAsync<BrokenCircuitException>(() => executor.ExecuteAsync(
            "LimiterProvider",
            ResilienceOperation.Provider,
            _ => Task.FromResult(42)));
    }

    private static ServiceProvider BuildProvider(
        int retryCount,
        TimeSpan? baseDelay = null,
        TimeSpan? maxDelay = null,
        TimeSpan? jitterMin = null,
        TimeSpan? jitterMax = null,
        TimeSpan? timeout = null,
        int circuitBreakerFailureThreshold = 10,
        TimeSpan? circuitBreakerBreakDuration = null)
    {
        var values = new Dictionary<string, string?>();
        foreach (var (name, profile) in CreateOptions(retryCount).Profiles)
        {
            values[$"Resilience:Profiles:{name}:RetryCount"] = profile.RetryCount.ToString();
            values[$"Resilience:Profiles:{name}:BaseDelay"] = (baseDelay ?? profile.BaseDelay).ToString();
            values[$"Resilience:Profiles:{name}:MaxDelay"] = (maxDelay ?? profile.MaxDelay).ToString();
            values[$"Resilience:Profiles:{name}:JitterMin"] = (jitterMin ?? profile.JitterMin).ToString();
            values[$"Resilience:Profiles:{name}:JitterMax"] = (jitterMax ?? profile.JitterMax).ToString();
            values[$"Resilience:Profiles:{name}:Timeout"] = (timeout ?? profile.Timeout).ToString();
            values[$"Resilience:Profiles:{name}:CircuitBreakerFailureThreshold"] = circuitBreakerFailureThreshold.ToString();
            values[$"Resilience:Profiles:{name}:CircuitBreakerSamplingWindow"] = profile.CircuitBreakerSamplingWindow.ToString();
            values[$"Resilience:Profiles:{name}:CircuitBreakerBreakDuration"] = (circuitBreakerBreakDuration ?? profile.CircuitBreakerBreakDuration).ToString();
            values[$"Resilience:Profiles:{name}:ConcurrencyLimit"] = profile.ConcurrencyLimit.ToString();
        }

        var services = new ServiceCollection();
        services.AddCoreResilience(new ConfigurationBuilder().AddInMemoryCollection(values).Build());
        return services.BuildServiceProvider();
    }

    private static ResilienceOptions CreateOptions(int retryCount = 0) =>
        new()
        {
            Profiles = new(StringComparer.OrdinalIgnoreCase)
            {
                ["LimiterProvider"] = CreateProfile(retryCount),
                ["DbCache"] = CreateProfile(retryCount),
                ["AuditOutbox"] = CreateProfile(retryCount),
                ["SafeOutboundHttp"] = CreateProfile(retryCount),
            },
        };

    private static ResilienceProfileOptions CreateProfile(int retryCount = 0) => new()
    {
        RetryCount = retryCount,
        BaseDelay = TimeSpan.FromMilliseconds(1),
        MaxDelay = TimeSpan.FromMilliseconds(5),
        JitterMin = TimeSpan.Zero,
        JitterMax = TimeSpan.Zero,
        Timeout = TimeSpan.FromSeconds(5),
        CircuitBreakerFailureThreshold = 10,
        CircuitBreakerSamplingWindow = TimeSpan.FromMinutes(1),
        CircuitBreakerBreakDuration = TimeSpan.FromMinutes(1),
        ConcurrencyLimit = 10,
    };
}
