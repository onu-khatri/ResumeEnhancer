namespace ResumeModuleAM.Responses;

public sealed class LanguageResponse
{
    public int Id { get; set; }

    public string LanguageName { get; set; } = string.Empty;

    public string? ProficiencyLevel { get; set; }

    public string? Description { get; set; }
}
