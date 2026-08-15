using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using ResumeEnhancer.Core.DomainLibrary.DomainModel;
using Microsoft.EntityFrameworkCore;

namespace ResumeEnhancer.Infrastructure.Persistence;

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
            ConfigureBaseColumns(modelBuilder, entityType.ClrType);

            modelBuilder.Entity(entityType.ClrType)
                .ToTable(GetTableName(entityType.ClrType), normalizedSchema);
        }

        return modelBuilder;
    }

    private static void ConfigureBaseColumns(ModelBuilder modelBuilder, Type entityType)
    {
        var builder = modelBuilder.Entity(entityType);

        if (typeof(AuditEntity).IsAssignableFrom(entityType))
        {
            builder.HasKey(nameof(AuditEntity.Id));
            builder.Property<DateTime>(nameof(AuditEntity.App_CreateDate))
                .HasDefaultValueSql("SYSUTCDATETIME()");
            builder.Property<byte[]>(nameof(AuditEntity.App_Version))
                .IsRowVersion();
        }

        if (typeof(ISetupData).IsAssignableFrom(entityType))
        {
            builder.Property<string>(nameof(ISetupData.Code))
                .HasMaxLength(100)
                .IsRequired();
            builder.Property<string>(nameof(ISetupData.Description))
                .HasMaxLength(1000)
                .IsRequired();
            builder.Property<System.Guid?>(nameof(ISetupData.Guid))
                .IsRequired();
            builder.Property<bool>(nameof(ISetupData.ObsoleteFlag))
                .HasDefaultValue(false);

            builder.HasIndex(nameof(ISetupData.Code)).IsUnique();
            builder.HasIndex(nameof(ISetupData.Guid)).IsUnique();
        }
    }

    private static string GetTableName(Type entityType)
    {
        var tableAttribute = entityType.GetCustomAttribute<TableAttribute>(inherit: false);
        var tableName = string.IsNullOrWhiteSpace(tableAttribute?.Name)
            ? entityType.Name
            : tableAttribute.Name;
        var prefix = GetTablePrefix(entityType);

        if (string.IsNullOrWhiteSpace(prefix) || tableName.StartsWith($"{prefix}_", StringComparison.Ordinal))
        {
            return tableName;
        }

        return $"{prefix}_{tableName}";
    }

    private static string GetTablePrefix(Type entityType)
    {
        if (typeof(SetupRelation).IsAssignableFrom(entityType))
        {
            return "SR";
        }

        if (typeof(SetupEntity).IsAssignableFrom(entityType))
        {
            return "S";
        }

        if (typeof(BusinessRelation).IsAssignableFrom(entityType))
        {
            return "BR";
        }

        if (typeof(BusinessEntity).IsAssignableFrom(entityType))
        {
            return "B";
        }

        return string.Empty;
    }
}

