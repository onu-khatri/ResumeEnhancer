using ResumeEnhancer.Core.DomainLibrary.DomainModel;

namespace ResumeEnhancer.TestUtilities.IntegrationSupport;

public interface IDataMocker
{
    IEnumerable<TEntity> GenerateEntities<TEntity>(
        EntityGenerationInstructions instructions,
        Action<int, TEntity>? populator = null)
        where TEntity : class, IAuditEntity;
}

