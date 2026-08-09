namespace Persistence;

public interface IUnitOfWorkTransaction : IDisposable, IAsyncDisposable
{
    bool IsCompleted { get; }

    Task CommitAsync(CancellationToken cancellationToken = default);

    Task RollbackAsync(CancellationToken cancellationToken = default);
}
