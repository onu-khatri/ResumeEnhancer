namespace ResumeEnhancer.Infrastructure.Persistence;

public interface IUnitOfWorkFactory<TDbContext>
    where TDbContext : AppDbContext
{
    IUnitOfWork<TDbContext> Current { get; }

    IUnitOfWorkScope<TDbContext> CreateScope();
}

