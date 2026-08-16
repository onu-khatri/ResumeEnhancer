using Microsoft.EntityFrameworkCore;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.TemplateModule.DM.Entities;

namespace ResumeEnhancer.TemplateModule.PL;

public sealed class TemplateModuleDbContextModelConfiguration : IAppDbContextModelConfiguration
{
    private readonly string _schema;

    public TemplateModuleDbContextModelConfiguration() : this(rootEntitySchema: null)
    {
    }

    public TemplateModuleDbContextModelConfiguration(string? rootEntitySchema)
    {
        _schema = TemplateModuleDatabase.GetSchema(rootEntitySchema);
    }

    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TemplateModuleDbContextModelConfiguration).Assembly);
        modelBuilder.ApplyModuleTableMappings(typeof(TemplateCategory).Assembly, _schema);
    }
}
