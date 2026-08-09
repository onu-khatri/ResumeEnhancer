using System.Linq.Expressions;
using DomainLibrary.DomainModel;

namespace Persistence;

public interface IQuerySpecification<T>
    where T : AuditEntity
{
    Expression<Func<T, bool>> Criteria { get; set; }

    Expression<Func<T, T>>? Select { get; set; }

    List<Expression<Func<T, object>>> Includes { get; }

    Expression<Func<T, object>>? OrderBy { get; }

    Expression<Func<T, object>>? OrderByDescending { get; }

    IQueryable<T> GetQuery(IQueryable<T> inputQuery);
}
