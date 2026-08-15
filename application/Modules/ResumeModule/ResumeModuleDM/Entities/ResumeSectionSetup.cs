using ResumeEnhancer.Core.DomainLibrary.DomainModel;
using ResumeEnhancer.ResumeModule.DM.Enums;

namespace ResumeEnhancer.ResumeModule.DM.Entities;

public class ResumeSectionSetup : SetupEntity
{
    public ResumeSectionType SectionType { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsVisible { get; set; } = true;
}

