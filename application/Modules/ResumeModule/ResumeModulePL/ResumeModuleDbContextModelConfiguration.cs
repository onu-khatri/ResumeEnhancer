using Microsoft.EntityFrameworkCore;
using Persistence;

namespace ResumeModulePL;

public sealed class ResumeModuleDbContextModelConfiguration : IAppDbContextModelConfiguration
{
    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ResumeModuleDbContextModelConfiguration).Assembly);
    }
}
