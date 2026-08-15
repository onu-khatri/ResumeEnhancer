using Microsoft.EntityFrameworkCore;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.ResumeModule.DM.Entities;

namespace ResumeEnhancer.ResumeModule.PL;

public sealed class ResumeModuleDbContextModelConfiguration : IAppDbContextModelConfiguration
{
    private readonly string _schema;

    public ResumeModuleDbContextModelConfiguration()
        : this(rootEntitySchema: null)
    {
    }

    public ResumeModuleDbContextModelConfiguration(string? rootEntitySchema)
    {
        _schema = ResumeModuleDatabase.GetSchema(rootEntitySchema);
    }

    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ResumeModuleDbContextModelConfiguration).Assembly);
        modelBuilder.ApplyModuleTableMappings(typeof(Resume).Assembly, _schema);
    }
}

