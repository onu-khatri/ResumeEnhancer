namespace ResumeEnhancer.Core.CommonLibrary.Resilience;

public enum ResilienceOperation
{
    Provider,
    DatabaseConflict,
    AuditDelivery,
    NonIdempotentMutation,
}

public delegate bool ResilienceRetryPredicate(Exception exception);

public interface IResilienceExecutor
{
    Task<T> ExecuteAsync<T>(
        string profileName,
        ResilienceOperation operation,
        Func<CancellationToken, Task<T>> callback,
        ResilienceRetryPredicate? retryPredicate = null,
        CancellationToken cancellationToken = default);

    Task ExecuteAsync(
        string profileName,
        ResilienceOperation operation,
        Func<CancellationToken, Task> callback,
        ResilienceRetryPredicate? retryPredicate = null,
        CancellationToken cancellationToken = default);
}
