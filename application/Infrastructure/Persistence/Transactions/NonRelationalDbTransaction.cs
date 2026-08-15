namespace ResumeEnhancer.Infrastructure.Persistence;

public sealed class NonRelationalDbTransaction : IUnitOfWorkTransaction
{
    public bool IsCompleted { get; private set; }

    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IsCompleted = true;
        return Task.CompletedTask;
    }

    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IsCompleted = true;
        return Task.CompletedTask;
    }

    public void Dispose()
    {
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

