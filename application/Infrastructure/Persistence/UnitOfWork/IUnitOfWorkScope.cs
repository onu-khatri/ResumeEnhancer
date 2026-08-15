namespace ResumeEnhancer.Infrastructure.Persistence;

public interface IUnitOfWorkScope<TDbContext> : IDisposable, IAsyncDisposable
    where TDbContext : AppDbContext
{
    IServiceProvider ServiceProvider { get; }

    TDbContext DbContext { get; }

    IUnitOfWork<TDbContext> UnitOfWork { get; }
}

