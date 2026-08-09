namespace ResumeModuleWeb.Validation.Shared;

internal static class ResumeEndpointValidation
{
    public static Dictionary<string, string[]> BodyRequired() =>
        new(StringComparer.Ordinal)
        {
            ["request"] = ["Request body is required."]
        };

    public static Dictionary<string, string[]> ResumeId(int resumeId)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);

        if (resumeId <= 0)
        {
            errors[nameof(resumeId)] = ["Resume id must be greater than 0."];
        }

        return errors;
    }

    public static Dictionary<string, string[]> Merge(
        IDictionary<string, string[]> first,
        IDictionary<string, string[]> second)
    {
        if (first.Count == 0)
        {
            return new Dictionary<string, string[]>(second, StringComparer.Ordinal);
        }

        if (second.Count == 0)
        {
            return new Dictionary<string, string[]>(first, StringComparer.Ordinal);
        }

        var merged = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        AddRange(merged, first);
        AddRange(merged, second);

        return merged.ToDictionary(
            pair => pair.Key,
            pair => pair.Value.ToArray(),
            StringComparer.Ordinal);
    }

    private static void AddRange(
        IDictionary<string, List<string>> target,
        IDictionary<string, string[]> source)
    {
        foreach (var item in source)
        {
            if (!target.TryGetValue(item.Key, out var messages))
            {
                messages = [];
                target[item.Key] = messages;
            }

            messages.AddRange(item.Value);
        }
    }
}
