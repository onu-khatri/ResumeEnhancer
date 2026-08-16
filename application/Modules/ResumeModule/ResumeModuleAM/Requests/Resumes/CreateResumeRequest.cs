using System.ComponentModel.DataAnnotations;

namespace ResumeEnhancer.ResumeModule.AM.Requests;

public sealed class CreateResumeRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Summary { get; set; }

    [MaxLength(500)]
    public string? Photo { get; set; }

    [MaxLength(100)]
    public string? ResumeTemplate { get; set; }

    public int? TemplateId { get; set; }

    [Required]
    public int UserId { get; set; }

    public PersonalInformationRequest? PersonalInformation { get; set; }

    [Required]
    public List<EducationRequest> Education { get; set; } = [];

    [Required]
    public List<CertificationRequest> Certifications { get; set; } = [];

    [Required]
    public List<SkillRequest> Skills { get; set; } = [];

    [Required]
    public List<WorkExperienceRequest> WorkExperiences { get; set; } = [];

    [Required]
    public List<ProjectRequest> Projects { get; set; } = [];
}

