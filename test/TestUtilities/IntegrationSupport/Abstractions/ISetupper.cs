using ResumeEnhancer.Core.DomainLibrary.DomainModel;
using Microsoft.EntityFrameworkCore;

namespace ResumeEnhancer.TestUtilities.IntegrationSupport;

public interface ISetupper : IDataMocker, IDisposable
{
    DbContext GetDbContext();

    DbContext GetFreshDbContext();

    TServiceType GetRequiredService<TServiceType>()
        where TServiceType : notnull;

    Task SetAuthenticatedUserDataAsync(IAuditEntity user, IAuditEntity accessProfile);

    ValueTask<IList<TEntity>> GenerateAndSaveEntitiesAsync<TEntity>(
        EntityGenerationInstructions instructions,
        Action<int, TEntity>? populator = null)
        where TEntity : class, IAuditEntity;

    void ClearDbContext();
}

