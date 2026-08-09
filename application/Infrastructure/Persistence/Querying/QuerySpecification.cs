using System.Linq.Expressions;
using DomainLibrary.DomainModel;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class QuerySpecification<T> : IQuerySpecification<T>
    where T : AuditEntity
{
    public Expression<Func<T, bool>> Criteria { get; set; } = _ => true;

    public Expression<Func<T, T>>? Select { get; set; }

    public List<Expression<Func<T, object>>> Includes { get; } = [];

    public Expression<Func<T, object>>? OrderBy { get; protected set; }

    public Expression<Func<T, object>>? OrderByDescending { get; protected set; }

    public virtual IQueryable<T> GetQuery(IQueryable<T> inputQuery)
    {
        ArgumentNullException.ThrowIfNull(inputQuery);

        var query = inputQuery.Where(Criteria);

        foreach (var include in Includes)
        {
            query = query.Include(include);
        }

        if (OrderBy is not null)
        {
            query = query.OrderBy(OrderBy);
        }
        else if (OrderByDescending is not null)
        {
            query = query.OrderByDescending(OrderByDescending);
        }

        if (Select is not null)
        {
            query = query.Select(Select);
        }

        return query;
    }

    protected void ApplyOrderBy(Expression<Func<T, object>> orderBy)
    {
        ArgumentNullException.ThrowIfNull(orderBy);

        OrderBy = orderBy;
        OrderByDescending = null;
    }

    protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescending)
    {
        ArgumentNullException.ThrowIfNull(orderByDescending);

        OrderBy = null;
        OrderByDescending = orderByDescending;
    }
}
