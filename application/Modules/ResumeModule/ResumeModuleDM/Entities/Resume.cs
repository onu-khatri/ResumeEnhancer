using System.ComponentModel.DataAnnotations;
using ResumeEnhancer.Core.DomainLibrary.DomainModel;

namespace ResumeEnhancer.ResumeModule.DM.Entities;

public class Resume : BusinessEntity
{
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Summary { get; set; }

    [MaxLength(500)]
    public string? Photo { get; set; }

    [MaxLength(100)]
    public string? ResumeTemplate { get; set; }

    [MaxLength(450)]
    public string UserId { get; set; } = string.Empty;

    public PersonalInformation? PersonalInformation { get; set; }

    public ICollection<Education> Education { get; set; } = new List<Education>();

    public ICollection<Certification> Certifications { get; set; } = new List<Certification>();

    public ICollection<Skill> Skills { get; set; } = new List<Skill>();

    public ICollection<WorkExperience> WorkExperiences { get; set; } = new List<WorkExperience>();

    public ICollection<Project> Projects { get; set; } = new List<Project>();
}

