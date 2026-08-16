using ResumeEnhancer.Infrastructure.Persistence;

namespace ResumeEnhancer.ProfilingModule.PL;

public static class ProfilingModuleDatabase
{
    public const string Schema = "profiling";

    public static string GetSchema(string? rootEntitySchema = null) =>
        string.IsNullOrWhiteSpace(rootEntitySchema)
            ? ModuleSchemaName.FromModule(Schema)
            : ModuleSchemaName.FromRootAndSupportingModule(rootEntitySchema, Schema);
}
