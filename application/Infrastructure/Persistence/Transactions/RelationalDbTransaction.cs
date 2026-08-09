using Microsoft.EntityFrameworkCore.Storage;

namespace Persistence;

public sealed class RelationalDbTransaction : IUnitOfWorkTransaction
{
    private readonly IDbContextTransaction _transaction;

    public RelationalDbTransaction(IDbContextTransaction transaction)
    {
        _transaction = transaction;
    }

    public bool IsCompleted { get; private set; }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (IsCompleted)
        {
            return;
        }

        await _transaction.CommitAsync(cancellationToken);
        IsCompleted = true;
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (IsCompleted)
        {
            return;
        }

        await _transaction.RollbackAsync(cancellationToken);
        IsCompleted = true;
    }

    public void Dispose()
    {
        _transaction.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _transaction.DisposeAsync();
    }
}
