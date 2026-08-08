using DomainLibrary.DomainModel;
using ResumeModuleDM.Enums;

namespace ResumeModuleDM.Entities;

public class ResumeSectionSetup : SetupEntity
{
    public ResumeSectionType SectionType { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsVisible { get; set; } = true;
}
