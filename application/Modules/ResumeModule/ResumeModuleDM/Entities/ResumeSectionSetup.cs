using ResumeEnhancer.Core.DomainLibrary.DomainModel;

namespace ResumeEnhancer.ResumeModule.DM.Entities;

public class ResumeSectionSetup : SetupEntity, IHasOrderedValues
{
    public int Order { get; set; }

    public bool IsVisible { get; set; } = true;
}

