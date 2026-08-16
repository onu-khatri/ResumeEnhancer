using Microsoft.EntityFrameworkCore;
using ResumeEnhancer.BillingModule.DM.Entities;
using ResumeEnhancer.Infrastructure.Persistence;

namespace ResumeEnhancer.BillingModule.PL;

public sealed class BillingModuleDbContextModelConfiguration : IAppDbContextModelConfiguration
{
    private readonly string _schema;

    public BillingModuleDbContextModelConfiguration() : this(rootEntitySchema: null)
    {
    }

    public BillingModuleDbContextModelConfiguration(string? rootEntitySchema)
    {
        _schema = BillingModuleDatabase.GetSchema(rootEntitySchema);
    }

    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BillingModuleDbContextModelConfiguration).Assembly);
        modelBuilder.ApplyModuleTableMappings(typeof(BillingAccount).Assembly, _schema);
    }
}
