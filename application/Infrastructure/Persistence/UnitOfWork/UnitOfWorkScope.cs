using Microsoft.Extensions.DependencyInjection;

namespace Persistence;

internal sealed class UnitOfWorkScope<TDbContext> : IUnitOfWorkScope<TDbContext>
    where TDbContext : AppDbContext
{
    private readonly IServiceScope _scope;

    public UnitOfWorkScope(
        IServiceScope scope,
        TDbContext dbContext,
        IUnitOfWork<TDbContext> unitOfWork)
    {
        _scope = scope;
        DbContext = dbContext;
        UnitOfWork = unitOfWork;
    }

    public IServiceProvider ServiceProvider => _scope.ServiceProvider;

    public TDbContext DbContext { get; }

    public IUnitOfWork<TDbContext> UnitOfWork { get; }

    public void Dispose()
    {
        _scope.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }
}
