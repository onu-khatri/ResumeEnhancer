namespace ResumeEnhancer.ResumeModule.SL.Handlers;

internal static partial class ResumeModelMapper
{
    private static string NormalizeRequired(string? value) =>
        value?.Trim() ?? string.Empty;

    private static string? NormalizeOptional(string? value)
    {
        var normalizedValue = value?.Trim();

        return string.IsNullOrWhiteSpace(normalizedValue)
            ? null
            : normalizedValue;
    }
}

