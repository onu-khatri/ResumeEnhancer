using Microsoft.Extensions.DependencyInjection;

namespace Persistence;

public sealed class UnitOfWorkFactory<TDbContext> : IUnitOfWorkFactory<TDbContext>
    where TDbContext : AppDbContext
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public UnitOfWorkFactory(
        IServiceProvider serviceProvider,
        IServiceScopeFactory serviceScopeFactory)
    {
        _serviceProvider = serviceProvider;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public IUnitOfWork<TDbContext> Current =>
        _serviceProvider.GetRequiredService<IUnitOfWork<TDbContext>>();

    public IUnitOfWorkScope<TDbContext> CreateScope()
    {
        var scope = _serviceScopeFactory.CreateScope();

        try
        {
            return new UnitOfWorkScope<TDbContext>(
                scope,
                scope.ServiceProvider.GetRequiredService<TDbContext>(),
                scope.ServiceProvider.GetRequiredService<IUnitOfWork<TDbContext>>());
        }
        catch
        {
            scope.Dispose();
            throw;
        }
    }
}
