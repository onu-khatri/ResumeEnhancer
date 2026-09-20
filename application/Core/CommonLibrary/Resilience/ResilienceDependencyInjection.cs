using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Resilience;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using Polly.Registry;
using System.Runtime.ExceptionServices;

namespace ResumeEnhancer.Core.CommonLibrary.Resilience;

public static class ResilienceDependencyInjection
{
    public static IServiceCollection AddCoreResilience(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection(ResilienceOptions.SectionName);
        var options = section.Get<ResilienceOptions>() ?? new ResilienceOptions();
        ResilienceOptionsValidator.Validate(options);

        services.AddOptions<ResilienceOptions>()
            .Bind(section)
            .Validate(ResilienceOptionsValidator.Validate)
            .ValidateOnStart();
        services.TryAddSingleton<IResilienceExecutor, ResilienceExecutor>();

        foreach (var (name, profile) in options.Profiles)
        {
            services.AddResiliencePipeline(name, builder =>
            {
                builder.AddConcurrencyLimiter(new System.Threading.RateLimiting.ConcurrencyLimiterOptions
                {
                    PermitLimit = profile.ConcurrencyLimit,
                    QueueLimit = 0,
                });
                builder.AddTimeout(new TimeoutStrategyOptions
                {
                    Timeout = profile.Timeout,
                });
                builder.AddRetry(new RetryStrategyOptions
                {
                    MaxRetryAttempts = profile.RetryCount,
                    ShouldHandle = new PredicateBuilder()
                        .Handle<Exception>(exception => exception is not OperationCanceledException
                            && exception is not NonRetryableResilienceException),
                    DelayGenerator = arguments =>
                    {
                        var exponentialMilliseconds = profile.BaseDelay.TotalMilliseconds *
                            Math.Pow(2, arguments.AttemptNumber);
                        var boundedMilliseconds = Math.Min(exponentialMilliseconds, profile.MaxDelay.TotalMilliseconds);
                        var jitterMilliseconds = profile.JitterMax == profile.JitterMin
                            ? profile.JitterMin.TotalMilliseconds
                            : Random.Shared.NextDouble() *
                                (profile.JitterMax.TotalMilliseconds - profile.JitterMin.TotalMilliseconds) +
                                profile.JitterMin.TotalMilliseconds;
                        var delay = TimeSpan.FromMilliseconds(Math.Min(
                            profile.MaxDelay.TotalMilliseconds,
                            boundedMilliseconds + jitterMilliseconds));
                        return new ValueTask<TimeSpan?>(delay);
                    },
                });
                builder.AddCircuitBreaker(new CircuitBreakerStrategyOptions
                {
                    FailureRatio = 1.0,
                    MinimumThroughput = profile.CircuitBreakerFailureThreshold,
                    SamplingDuration = profile.CircuitBreakerSamplingWindow,
                    BreakDuration = profile.CircuitBreakerBreakDuration,
                });
            });
        }

        return services;
    }

    private sealed class NonRetryableResilienceException(Exception innerException)
        : Exception("The resilience operation is not retryable.", innerException);

    private sealed class ResilienceExecutor(
        ResiliencePipelineProvider<string> pipelineProvider) : IResilienceExecutor
    {
        public Task ExecuteAsync(
            string profileName,
            ResilienceOperation operation,
            Func<CancellationToken, Task> callback,
            ResilienceRetryPredicate? retryPredicate = null,
            CancellationToken cancellationToken = default) =>
            ExecuteAsync<object?>(
                profileName,
                operation,
                async token =>
                {
                    await callback(token);
                    return null;
                },
                retryPredicate,
                cancellationToken);

        public async Task<T> ExecuteAsync<T>(
            string profileName,
            ResilienceOperation operation,
            Func<CancellationToken, Task<T>> callback,
            ResilienceRetryPredicate? retryPredicate = null,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(profileName);
            ArgumentNullException.ThrowIfNull(callback);
            cancellationToken.ThrowIfCancellationRequested();

            if (operation == ResilienceOperation.NonIdempotentMutation)
                return await callback(cancellationToken);

            var pipeline = pipelineProvider.GetPipeline(profileName);
            try
            {
                return await pipeline.ExecuteAsync(async token =>
                {
                    try
                    {
                        return await callback(token);
                    }
                    catch (Exception exception) when (
                        exception is not OperationCanceledException &&
                        retryPredicate is not null &&
                        !retryPredicate(exception))
                    {
                        throw new NonRetryableResilienceException(exception);
                    }
                }, cancellationToken);
            }
            catch (NonRetryableResilienceException exception)
            {
                ExceptionDispatchInfo.Capture(exception.InnerException!).Throw();
                throw;
            }
        }
    }
}
