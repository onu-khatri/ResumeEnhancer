using System.Linq.Expressions;
using DomainLibrary.DomainModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Persistence;

public class AuditEntityRepository<TElement> : IAuditEntityRepository<TElement>
    where TElement : AuditEntity
{
    public const int MaxPageSize = 500;

    private readonly DbContext _dbContext;
    private readonly DbSet<TElement> _set;

    public AuditEntityRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _set = dbContext.Set<TElement>();
    }

    public IQueryable<TElement> Query() => _set;

    public IQueryable<TElement> GetQuery(int id) =>
        Query().Where(element => element.Id == id);

    public IQueryable<TElement> GetQuery(List<int> ids)
    {
        ArgumentNullException.ThrowIfNull(ids);

        var distinctIds = ids.Distinct().ToArray();

        return distinctIds.Length == 0
            ? Query().Where(_ => false)
            : Query().Where(element => distinctIds.Contains(element.Id));
    }

    public async Task<TElement?> FindAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        await _set.FindAsync([id], cancellationToken);

    public async Task<bool> ExistsAsync(
        Expression<Func<TElement, bool>>? filter,
        CancellationToken cancellationToken = default) =>
        filter is null
            ? await _set.AnyAsync(cancellationToken)
            : await _set.AnyAsync(filter, cancellationToken);

    public async Task<bool> ExistsAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        await _set.AnyAsync(element => element.Id == id, cancellationToken);

    public async Task<bool> ExistsAsync(
        IEnumerable<int> ids,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ids);

        var distinctIds = ids.Distinct().ToArray();

        if (distinctIds.Length == 0)
        {
            return true;
        }

        var existingCount = await _set.CountAsync(
            element => distinctIds.Contains(element.Id),
            cancellationToken);

        return existingCount == distinctIds.Length;
    }

    public async Task AddAsync(
        TElement element,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(element);

        await _set.AddAsync(element, cancellationToken);
    }

    public void Update(TElement element)
    {
        ArgumentNullException.ThrowIfNull(element);

        _set.Update(element);
    }

    public void Update(IList<TElement> elements)
    {
        ArgumentNullException.ThrowIfNull(elements);

        _set.UpdateRange(elements);
    }

    public void Delete(TElement element)
    {
        ArgumentNullException.ThrowIfNull(element);

        _set.Remove(element);
    }

    public void Delete(IList<TElement> elements)
    {
        ArgumentNullException.ThrowIfNull(elements);

        _set.RemoveRange(elements);
    }

    public void DeleteWhere(Expression<Func<TElement, bool>> expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        _set.RemoveRange(_set.Where(expression));
    }

    public void Attach(TElement element)
    {
        ArgumentNullException.ThrowIfNull(element);

        _set.Attach(element);
    }

    public void Detach(TElement element)
    {
        ArgumentNullException.ThrowIfNull(element);

        _dbContext.Entry(element).State = EntityState.Detached;
    }

    public void Detach(IList<TElement> elements)
    {
        ArgumentNullException.ThrowIfNull(elements);

        foreach (var element in elements)
        {
            Detach(element);
        }
    }

    public IQueryable<TElement> FindBySpecification(
        IQuerySpecification<TElement> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);

        return specification.GetQuery(Query());
    }

    public async Task<PagedQueryResult<TElement>> FindAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<TElement, bool>>? filter,
        IModelLoader<TElement>? modelLoader = null,
        CancellationToken cancellationToken = default)
    {
        ValidatePaging(pageNumber, pageSize);

        var filteredQuery = Query();

        if (filter is not null)
        {
            filteredQuery = filteredQuery.Where(filter);
        }

        var totalCount = await filteredQuery.CountAsync(cancellationToken);

        var pageQuery = ApplyModelLoader(filteredQuery, modelLoader)
            .OrderBy(element => element.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        var items = await pageQuery.ToListAsync(cancellationToken);

        return new PagedQueryResult<TElement>(
            items,
            pageNumber,
            pageSize,
            totalCount);
    }

    private IQueryable<TElement> ApplyModelLoader(
        IQueryable<TElement> query,
        IModelLoader<TElement>? modelLoader)
    {
        if (modelLoader is null)
        {
            return query;
        }

        var includePaths = modelLoader
            .GetIncludablePaths()
            .Select(ResolveNavigationIncludePath)
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .Distinct(StringComparer.Ordinal);

        foreach (var includePath in includePaths)
        {
            query = query.Include(includePath!);
        }

        return query;
    }

    private string? ResolveNavigationIncludePath(IncludablePath path)
    {
        IReadOnlyEntityType? entityType = _dbContext.Model.FindEntityType(typeof(TElement));

        if (entityType is null)
        {
            return null;
        }

        var includeSegments = new List<string>();

        foreach (var segment in path.Segments)
        {
            var navigation = FindNavigation(entityType, segment);

            if (navigation is null)
            {
                break;
            }

            includeSegments.Add(segment);
            entityType = navigation.TargetEntityType;
        }

        return includeSegments.Count == 0
            ? null
            : string.Join(".", includeSegments);
    }

    private static IReadOnlyNavigationBase? FindNavigation(
        IReadOnlyEntityType entityType,
        string segment)
    {
        var navigation = entityType.FindNavigation(segment);

        if (navigation is not null)
        {
            return navigation;
        }

        return entityType.FindSkipNavigation(segment);
    }

    private static void ValidatePaging(int pageNumber, int pageSize)
    {
        if (pageNumber < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pageNumber),
                pageNumber,
                "Page number must be greater than or equal to 1.");
        }

        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pageSize),
                pageSize,
                "Page size must be greater than or equal to 1.");
        }

        if (pageSize > MaxPageSize)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pageSize),
                pageSize,
                $"Page size must be less than or equal to {MaxPageSize}.");
        }
    }
}
