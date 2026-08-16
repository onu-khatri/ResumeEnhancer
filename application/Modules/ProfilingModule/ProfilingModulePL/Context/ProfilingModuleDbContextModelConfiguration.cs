using Microsoft.EntityFrameworkCore;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.ProfilingModule.DM.Entities;

namespace ResumeEnhancer.ProfilingModule.PL;

public sealed class ProfilingModuleDbContextModelConfiguration : IAppDbContextModelConfiguration
{
    private readonly string _schema;

    public ProfilingModuleDbContextModelConfiguration() : this(rootEntitySchema: null)
    {
    }

    public ProfilingModuleDbContextModelConfiguration(string? rootEntitySchema)
    {
        _schema = ProfilingModuleDatabase.GetSchema(rootEntitySchema);
    }

    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProfilingModuleDbContextModelConfiguration).Assembly);
        modelBuilder.ApplyModuleTableMappings(typeof(User).Assembly, _schema);
    }
}
