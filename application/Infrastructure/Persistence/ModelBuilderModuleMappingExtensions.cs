using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public static class ModelBuilderModuleMappingExtensions
{
    public static ModelBuilder ApplyModuleTableMappings(
        this ModelBuilder modelBuilder,
        Assembly entityAssembly,
        string schema)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        ArgumentNullException.ThrowIfNull(entityAssembly);

        var normalizedSchema = ModuleSchemaName.FromModule(schema);
        var entityTypes = modelBuilder.Model
            .GetEntityTypes()
            .Where(entityType =>
                !entityType.IsOwned()
                && entityType.ClrType is not null
                && entityType.ClrType.Assembly == entityAssembly)
            .OrderBy(entityType => entityType.ClrType.FullName);

        foreach (var entityType in entityTypes)
        {
            modelBuilder.Entity(entityType.ClrType)
                .ToTable(GetTableName(entityType.ClrType), normalizedSchema);
        }

        return modelBuilder;
    }

    private static string GetTableName(MemberInfo entityType)
    {
        var tableAttribute = entityType.GetCustomAttribute<TableAttribute>(inherit: false);

        return string.IsNullOrWhiteSpace(tableAttribute?.Name)
            ? entityType.Name
            : tableAttribute.Name;
    }
}
