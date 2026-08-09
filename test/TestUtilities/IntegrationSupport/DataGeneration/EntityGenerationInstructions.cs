namespace ResumeEnhancer.TestUtilities.IntegrationSupport;

public sealed class EntityGenerationInstructions
{
    public EntityGenerationInstructions(int count = 1)
    {
        if (count < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(count),
                count,
                "Entity generation count must be greater than 0.");
        }

        Count = count;
    }

    public int Count { get; }

    public static EntityGenerationInstructions Single() => new();

    public static EntityGenerationInstructions Many(int count) => new(count);
}
