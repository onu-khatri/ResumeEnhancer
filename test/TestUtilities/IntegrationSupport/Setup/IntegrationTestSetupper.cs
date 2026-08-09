using AutoFixture;
using DomainLibrary.DomainModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence;

namespace ResumeEnhancer.TestUtilities.IntegrationSupport;

internal sealed class IntegrationTestSetupper<TProgram> : ISetupper
    where TProgram : class
{
    private readonly IntegrationTestUtilities<TProgram> _utilities;
    private readonly IServiceScope _scope;
    private readonly List<IServiceScope> _freshScopes = [];
    private readonly Fixture _fixture;

    public IntegrationTestSetupper(IntegrationTestUtilities<TProgram> utilities)
    {
        _utilities = utilities;
        _scope = utilities.Services.CreateScope();
        _fixture = new Fixture();

        foreach (var behavior in _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToArray())
        {
            _fixture.Behaviors.Remove(behavior);
        }

        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    public DbContext GetDbContext() => _scope.ServiceProvider.GetRequiredService<AppDbContext>();

    public DbContext GetFreshDbContext()
    {
        var freshScope = _utilities.Services.CreateScope();
        _freshScopes.Add(freshScope);

        return freshScope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    public TServiceType GetRequiredService<TServiceType>()
        where TServiceType : notnull =>
        _scope.ServiceProvider.GetRequiredService<TServiceType>();

    public Task SetAuthenticatedUserDataAsync(IAuditEntity user, IAuditEntity accessProfile)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(accessProfile);

        var externalUserId = user is TestAuthenticatedEntity authenticatedUser
            && !string.IsNullOrWhiteSpace(authenticatedUser.ExternalUserId)
                ? authenticatedUser.ExternalUserId
                : $"user-{user.Id}";
        var privileges = accessProfile is TestAuthenticatedEntity authenticatedProfile
            ? authenticatedProfile.Privileges
            : [];

        _utilities.AuthenticationState.Set(new TestAuthenticatedAccess(
            externalUserId,
            user.Id,
            accessProfile.Id,
            privileges));

        return Task.CompletedTask;
    }

    public IEnumerable<TEntity> GenerateEntities<TEntity>(
        EntityGenerationInstructions instructions,
        Action<int, TEntity>? populator = null)
        where TEntity : class, IAuditEntity
    {
        ArgumentNullException.ThrowIfNull(instructions);

        return Enumerable.Range(0, instructions.Count)
            .Select(index =>
            {
                var entity = _fixture.Create<TEntity>();
                entity.Id = 0;
                entity.App_CreateDate = default;
                entity.App_CreateUserId = null;
                entity.App_UpdateDate = null;
                entity.App_UpdateUserId = null;
                entity.App_Version = [];
                populator?.Invoke(index, entity);

                return entity;
            })
            .ToList();
    }

    public async ValueTask<IList<TEntity>> GenerateAndSaveEntitiesAsync<TEntity>(
        EntityGenerationInstructions instructions,
        Action<int, TEntity>? populator = null)
        where TEntity : class, IAuditEntity
    {
        var entities = GenerateEntities(instructions, populator).ToList();
        var dbContext = (AppDbContext)GetDbContext();

        dbContext.AddRange(entities);
        await dbContext.SaveChangesAsync(new IntegrationTestAudit(_utilities.AuthenticationState.AuditUserId));
        dbContext.ChangeTracker.Clear();

        return entities;
    }

    public void ClearDbContext()
    {
        var dbContext = (AppDbContext)GetDbContext();

        dbContext.ChangeTracker.Clear();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
    }

    public void Dispose()
    {
        foreach (var freshScope in _freshScopes)
        {
            freshScope.Dispose();
        }

        _scope.Dispose();
    }
}
