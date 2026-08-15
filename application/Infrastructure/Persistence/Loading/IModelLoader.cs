using ResumeEnhancer.Core.DomainLibrary.DomainModel;

namespace ResumeEnhancer.Infrastructure.Persistence;

public interface IModelLoader
{
    List<IncludablePath> GetIncludablePaths();
}

public interface IModelLoader<TModel> : IModelLoader
    where TModel : AuditEntity
{
    IModelLoader<TModel> Build(Action<IModelLoaderNavigator<TModel>> buildAction);
}

