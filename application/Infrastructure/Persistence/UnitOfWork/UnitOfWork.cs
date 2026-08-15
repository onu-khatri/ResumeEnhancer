using ResumeEnhancer.Core.DomainLibrary.DomainModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ResumeEnhancer.Infrastructure.Persistence;

public class UnitOfWork<TDbContext> : IUnitOfWork<TDbContext>
    where TDbContext : AppDbContext
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<Type, object> _repositories = [];
    private bool _disposed;

    public UnitOfWork(
        TDbContext dbContext,
        IServiceProvider serviceProvider)
    {
        DbContext = dbContext;
        _serviceProvider = serviceProvider;
    }

    public TDbContext DbContext { get; }

    public async Task<IUnitOfWorkTransaction> CreateTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        if (!DbContext.Database.IsRelational())
        {
            return new NonRelationalDbTransaction();
        }

        var currentTransaction = DbContext.Database.CurrentTransaction;

        if (currentTransaction is not null)
        {
            return new NestedDbTransaction(currentTransaction);
        }

        var transaction = await DbContext.Database.BeginTransactionAsync(cancellationToken);

        return new RelationalDbTransaction(transaction);
    }

    public async Task<int> SaveAsync(
        IAudit auditUser,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(auditUser);

        return await DbContext.SaveChangesAsync(auditUser, cancellationToken);
    }

    public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        return await DbContext.SaveChangesAsync(cancellationToken);
    }

    public IAuditEntityRepository<TElement> GetRepo<TElement>()
        where TElement : AuditEntity
    {
        ThrowIfDisposed();

        return GetOrCreateRepository(
            typeof(IAuditEntityRepository<TElement>),
            () => _serviceProvider.GetRequiredService<IAuditEntityRepository<TElement>>());
    }

    public TIRepo GetRepo<TIRepo, TElement>()
        where TIRepo : class, IAuditEntityRepository<TElement>
        where TElement : AuditEntity
    {
        ThrowIfDisposed();

        return GetOrCreateRepository(
            typeof(TIRepo),
            ResolveRepository<TIRepo>);
    }

    public TIRepo GetRepoLight<TIRepo>()
        where TIRepo : class
    {
        ThrowIfDisposed();

        return GetOrCreateRepository(
            typeof(TIRepo),
            ResolveRepository<TIRepo>);
    }

    public void PreloadSetupEntities(params ISetupData[] setupEntities)
    {
        ArgumentNullException.ThrowIfNull(setupEntities);

        PreloadSetupEntities((IEnumerable<ISetupData>)setupEntities);
    }

    public void PreloadSetupEntities(IEnumerable<ISetupData> setupEntities)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(setupEntities);

        foreach (var setupEntity in setupEntities)
        {
            ArgumentNullException.ThrowIfNull(setupEntity);

            var entry = DbContext.Entry(setupEntity);

            if (entry.State == EntityState.Detached)
            {
                DbContext.Attach(setupEntity);
            }
        }
    }

    public void Dispose()
    {
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }

    private TRepository GetOrCreateRepository<TRepository>(
        Type key,
        Func<TRepository> createRepository)
        where TRepository : class
    {
        if (_repositories.TryGetValue(key, out var repository))
        {
            return (TRepository)repository;
        }

        var typedRepository = createRepository();
        _repositories.Add(key, typedRepository);

        return typedRepository;
    }

    private TRepository ResolveRepository<TRepository>()
        where TRepository : class
    {
        var repository = _serviceProvider.GetService<TRepository>();

        if (repository is not null)
        {
            return repository;
        }

        if (!typeof(TRepository).IsInterface && !typeof(TRepository).IsAbstract)
        {
            return ActivatorUtilities.CreateInstance<TRepository>(_serviceProvider);
        }

        throw new InvalidOperationException(
            $"Repository '{typeof(TRepository).FullName}' is not registered.");
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }
}

