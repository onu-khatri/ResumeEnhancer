using System.ComponentModel.DataAnnotations;
using ResumeModuleDM.Enums;

namespace ResumeModuleDM.Entities;

public class ResumeSectionSetup
{
    [Key]
    public int Id { get; set; }

    public ResumeSectionType SectionType { get; set; }

    [MaxLength(100)]
    public string SectionTitle { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    public bool IsVisible { get; set; } = true;
}
