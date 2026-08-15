using System.Linq.Expressions;

namespace ResumeEnhancer.Infrastructure.Persistence;

public interface IModelLoaderNavigator<TModel>
{
    IModelLoaderNavigator<TModel> Load<TResult>(
        Expression<Func<TModel, TResult>> expression);

    IModelLoaderNavigator<TModel> LoadRelated();

    IModelLoaderNavigator<TNavigation> Navigate<TNavigation>(
        Expression<Func<TModel, TNavigation>> selector);

    IModelLoaderNavigator<TNavigation> NavigateCollection<TNavigation>(
        Expression<Func<TModel, IEnumerable<TNavigation>>> selector);
}

