using ResumeEnhancer.Infrastructure.Persistence;

namespace ResumeEnhancer.BillingModule.PL;

public static class BillingModuleDatabase
{
    public const string Schema = "billing";

    public static string GetSchema(string? rootEntitySchema = null) =>
        string.IsNullOrWhiteSpace(rootEntitySchema)
            ? ModuleSchemaName.FromModule(Schema)
            : ModuleSchemaName.FromRootAndSupportingModule(rootEntitySchema, Schema);
}
