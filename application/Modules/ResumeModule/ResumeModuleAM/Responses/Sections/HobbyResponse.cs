namespace ResumeModuleAM.Responses;

public sealed class HobbyResponse
{
    public int Id { get; set; }

    public string HobbyName { get; set; } = string.Empty;

    public string? Description { get; set; }
}
