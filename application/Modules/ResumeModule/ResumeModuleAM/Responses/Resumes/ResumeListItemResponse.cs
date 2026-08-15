namespace ResumeEnhancer.ResumeModule.AM.Responses;

public sealed class ResumeListItemResponse
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Summary { get; set; }

    public string? Photo { get; set; }

    public string? ResumeTemplate { get; set; }

    public string UserId { get; set; } = string.Empty;

    public DateTime App_CreateDate { get; set; }

    public DateTime? App_UpdateDate { get; set; }

    public int EducationCount { get; set; }

    public int CertificationCount { get; set; }

    public int SkillCount { get; set; }

    public int WorkExperienceCount { get; set; }

    public int ProjectCount { get; set; }
}

