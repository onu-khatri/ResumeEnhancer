using ResumeEnhancer.Infrastructure.Persistence;

namespace ResumeEnhancer.TemplateModule.PL;

public static class TemplateModuleDatabase
{
    public const string Schema = "template";

    public static string GetSchema(string? rootEntitySchema = null) =>
        string.IsNullOrWhiteSpace(rootEntitySchema)
            ? ModuleSchemaName.FromModule(Schema)
            : ModuleSchemaName.FromRootAndSupportingModule(rootEntitySchema, Schema);
}
