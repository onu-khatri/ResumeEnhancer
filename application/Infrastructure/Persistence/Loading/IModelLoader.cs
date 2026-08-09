using DomainLibrary.DomainModel;

namespace Persistence;

public interface IModelLoader
{
    List<IncludablePath> GetIncludablePaths();
}

public interface IModelLoader<TModel> : IModelLoader
    where TModel : AuditEntity
{
    IModelLoader<TModel> Build(Action<IModelLoaderNavigator<TModel>> buildAction);
}
