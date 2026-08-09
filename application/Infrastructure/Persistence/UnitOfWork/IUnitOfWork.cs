using DomainLibrary.DomainModel;

namespace Persistence;

public interface IUnitOfWork<TDbContext> : IDisposable, IAsyncDisposable
    where TDbContext : AppDbContext
{
    TDbContext DbContext { get; }

    Task<IUnitOfWorkTransaction> CreateTransactionAsync(CancellationToken cancellationToken = default);

    Task<int> SaveAsync(IAudit auditUser, CancellationToken cancellationToken = default);

    Task<int> SaveAsync(CancellationToken cancellationToken = default);

    IAuditEntityRepository<TElement> GetRepo<TElement>()
        where TElement : AuditEntity;

    TIRepo GetRepo<TIRepo, TElement>()
        where TIRepo : class, IAuditEntityRepository<TElement>
        where TElement : AuditEntity;

    TIRepo GetRepoLight<TIRepo>()
        where TIRepo : class;

    void PreloadSetupEntities(params ISetupData[] setupEntities);

    void PreloadSetupEntities(IEnumerable<ISetupData> setupEntities);
}
