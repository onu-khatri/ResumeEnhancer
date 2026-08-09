using Microsoft.EntityFrameworkCore;
using Persistence;
using ResumeModuleDM.Entities;

namespace ResumeModulePL;

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
