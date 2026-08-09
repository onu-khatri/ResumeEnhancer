namespace ResumeModuleAM.Responses;

public sealed class ResumeDetailResponse
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Summary { get; set; }

    public string? Photo { get; set; }

    public string? ResumeTemplate { get; set; }

    public string UserId { get; set; } = string.Empty;

    public DateTime App_CreateDate { get; set; }

    public DateTime? App_UpdateDate { get; set; }

    public byte[] App_Version { get; set; } = [];

    public PersonalInformationResponse? PersonalInformation { get; set; }

    public IReadOnlyList<EducationResponse> Education { get; set; } = [];

    public IReadOnlyList<CertificationResponse> Certifications { get; set; } = [];

    public IReadOnlyList<SkillResponse> Skills { get; set; } = [];

    public IReadOnlyList<WorkExperienceResponse> WorkExperiences { get; set; } = [];

    public IReadOnlyList<ProjectResponse> Projects { get; set; } = [];
}
