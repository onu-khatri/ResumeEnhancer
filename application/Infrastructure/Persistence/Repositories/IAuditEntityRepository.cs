using System.Linq.Expressions;
using DomainLibrary.DomainModel;

namespace Persistence;

public interface IAuditEntityRepository<TElement>
    where TElement : AuditEntity
{
    IQueryable<TElement> Query();

    IQueryable<TElement> GetQuery(int id);

    IQueryable<TElement> GetQuery(List<int> ids);

    Task<TElement?> FindAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Expression<Func<TElement, bool>>? filter,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);

    Task AddAsync(TElement element, CancellationToken cancellationToken = default);

    void Update(TElement element);

    void Update(IList<TElement> elements);

    void Delete(TElement element);

    void Delete(IList<TElement> elements);

    void DeleteWhere(Expression<Func<TElement, bool>> expression);

    void Attach(TElement element);

    void Detach(TElement element);

    void Detach(IList<TElement> elements);

    IQueryable<TElement> FindBySpecification(IQuerySpecification<TElement> specification);

    Task<PagedQueryResult<TElement>> FindAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<TElement, bool>>? filter,
        IModelLoader<TElement>? modelLoader = null,
        CancellationToken cancellationToken = default);
}
