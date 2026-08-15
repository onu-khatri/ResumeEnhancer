namespace ResumeEnhancer.ResumeModule.AM.Responses;

public sealed class ProjectResponse
{
    public int Id { get; set; }

    public string ProjectName { get; set; } = string.Empty;

    public string? Role { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Description { get; set; }

    public string? TechnologiesUsed { get; set; }

    public bool IsCurrent { get; set; }
}

