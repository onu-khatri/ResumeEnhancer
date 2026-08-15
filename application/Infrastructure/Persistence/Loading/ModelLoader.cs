using ResumeEnhancer.Core.DomainLibrary.DomainModel;

namespace ResumeEnhancer.Infrastructure.Persistence;

public class ModelLoader<TModel> : IModelLoader<TModel>
    where TModel : AuditEntity
{
    private List<IncludablePath> _navigationPaths = [];

    public IModelLoader<TModel> Build(Action<IModelLoaderNavigator<TModel>> buildAction)
    {
        ArgumentNullException.ThrowIfNull(buildAction);

        var navigator = new ModelLoaderNavigator<TModel>();

        buildAction.Invoke(navigator);

        _navigationPaths.AddRange(navigator.GetPaths());
        _navigationPaths = _navigationPaths.Distinct().ToList();

        return this;
    }

    public List<IncludablePath> GetIncludablePaths() =>
        _navigationPaths;
}

