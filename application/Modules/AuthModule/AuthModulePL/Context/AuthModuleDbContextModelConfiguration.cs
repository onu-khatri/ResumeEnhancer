using Microsoft.EntityFrameworkCore;
using ResumeEnhancer.AuthModule.DM.Entities;
using ResumeEnhancer.Infrastructure.Persistence;

namespace ResumeEnhancer.AuthModule.PL;

public sealed class AuthModuleDbContextModelConfiguration(string? rootEntitySchema = null)
    : IAppDbContextModelConfiguration
{
    private readonly string schema = AuthModuleDatabase.GetSchema(rootEntitySchema);

    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AuthModuleDbContextModelConfiguration).Assembly
        );
        modelBuilder.ApplyModuleTableMappings(typeof(AuthenticationIdentity).Assembly, schema);
    }
}
