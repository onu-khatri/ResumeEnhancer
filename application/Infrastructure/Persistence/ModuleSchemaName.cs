namespace Persistence;

public static class ModuleSchemaName
{
    private const char IntegratedSchemaSeparator = '_';

    public static string FromModule(string moduleSchema) =>
        NormalizeSegment(moduleSchema, nameof(moduleSchema));

    public static string FromRootAndSupportingModule(
        string rootEntitySchema,
        string supportingModuleSchema) =>
        string.Join(
            IntegratedSchemaSeparator,
            NormalizeSegment(rootEntitySchema, nameof(rootEntitySchema)),
            NormalizeSegment(supportingModuleSchema, nameof(supportingModuleSchema)));

    private static string NormalizeSegment(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Schema name cannot be empty.", parameterName);
        }

        var normalizedValue = value.Trim();

        if (!IsValidFirstCharacter(normalizedValue[0]))
        {
            throw new ArgumentException(
                "Schema name must start with a letter or underscore.",
                parameterName);
        }

        if (normalizedValue.Any(character => !IsValidSchemaCharacter(character)))
        {
            throw new ArgumentException(
                "Schema name can contain only letters, numbers, and underscores.",
                parameterName);
        }

        return normalizedValue;
    }

    private static bool IsValidFirstCharacter(char character) =>
        char.IsAsciiLetter(character) || character == IntegratedSchemaSeparator;

    private static bool IsValidSchemaCharacter(char character) =>
        char.IsAsciiLetterOrDigit(character) || character == IntegratedSchemaSeparator;
}
