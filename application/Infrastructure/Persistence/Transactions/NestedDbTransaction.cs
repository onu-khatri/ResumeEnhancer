using Microsoft.EntityFrameworkCore.Storage;

namespace Persistence;

public sealed class NestedDbTransaction : IUnitOfWorkTransaction
{
    private readonly IDbContextTransaction _currentTransaction;

    public NestedDbTransaction(IDbContextTransaction currentTransaction)
    {
        _currentTransaction = currentTransaction;
    }

    public bool IsCompleted { get; private set; }

    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IsCompleted = true;
        return Task.CompletedTask;
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (IsCompleted)
        {
            return;
        }

        await _currentTransaction.RollbackAsync(cancellationToken);
        IsCompleted = true;
    }

    public void Dispose()
    {
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
