using ResumeEnhancer.Infrastructure.Persistence;

namespace ResumeEnhancer.ResumeModule.PL;

public static class ResumeModuleDatabase
{
    public const string Schema = "resume";

    public static string GetSchema(string? rootEntitySchema = null) =>
        string.IsNullOrWhiteSpace(rootEntitySchema)
            ? ModuleSchemaName.FromModule(Schema)
            : ModuleSchemaName.FromRootAndSupportingModule(rootEntitySchema, Schema);
}

