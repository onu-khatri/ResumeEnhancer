namespace ResumeEnhancer.ResumeModule.AM.Responses;

public sealed class SkillResponse
{
    public int Id { get; set; }

    public string SkillName { get; set; } = string.Empty;

    public string? ProficiencyLevel { get; set; }

    public decimal? YearsOfExperience { get; set; }

    public string? Description { get; set; }
}

