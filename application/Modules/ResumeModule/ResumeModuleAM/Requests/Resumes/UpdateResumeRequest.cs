using System.ComponentModel.DataAnnotations;

namespace ResumeEnhancer.ResumeModule.AM.Requests;

public sealed class UpdateResumeRequest
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

    public int? UserId { get; set; }

    public bool RemovePersonalInformation { get; set; }

    public PersonalInformationRequest? PersonalInformation { get; set; }

    public List<EducationRequest>? Education { get; set; }

    public List<CertificationRequest>? Certifications { get; set; }

    public List<SkillRequest>? Skills { get; set; }

    public List<WorkExperienceRequest>? WorkExperiences { get; set; }

    public List<ProjectRequest>? Projects { get; set; }
}

