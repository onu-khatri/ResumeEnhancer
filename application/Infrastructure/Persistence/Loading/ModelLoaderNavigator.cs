using System.Linq.Expressions;

namespace ResumeEnhancer.Infrastructure.Persistence;

public sealed class ModelLoaderNavigator<TModel> : IModelLoaderNavigator<TModel>
{
    private readonly List<IncludablePath> _paths;
    private readonly IReadOnlyList<string> _currentSegments;

    public ModelLoaderNavigator()
        : this([], [])
    {
    }

    private ModelLoaderNavigator(
        List<IncludablePath> paths,
        IReadOnlyList<string> currentSegments)
    {
        _paths = paths;
        _currentSegments = currentSegments;
    }

    public IModelLoaderNavigator<TModel> Load<TResult>(
        Expression<Func<TModel, TResult>> expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        _paths.Add(
            IncludablePath.FromSegments(
                _currentSegments.Concat(
                    IncludablePath.MemberPath.FromExpression(expression.Body))));

        return this;
    }

    public IModelLoaderNavigator<TModel> LoadRelated()
    {
        if (_currentSegments.Count > 0)
        {
            _paths.Add(IncludablePath.FromSegments(_currentSegments));
        }

        return this;
    }

    public IModelLoaderNavigator<TNavigation> Navigate<TNavigation>(
        Expression<Func<TModel, TNavigation>> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);

        return new ModelLoaderNavigator<TNavigation>(
            _paths,
            _currentSegments.Concat(
                IncludablePath.MemberPath.FromExpression(selector.Body)).ToArray());
    }

    public IModelLoaderNavigator<TNavigation> NavigateCollection<TNavigation>(
        Expression<Func<TModel, IEnumerable<TNavigation>>> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);

        return new ModelLoaderNavigator<TNavigation>(
            _paths,
            _currentSegments.Concat(
                IncludablePath.MemberPath.FromExpression(selector.Body)).ToArray());
    }

    internal List<IncludablePath> GetPaths() => _paths;
}

