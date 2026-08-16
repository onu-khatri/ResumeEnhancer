using ResumeEnhancer.Core.DomainLibrary.DomainModel;

namespace ResumeEnhancer.ProfilingModule.DM.Entities;

public sealed class AccessProfileRole : SetupRelation
{
    public int AccessProfileId { get; set; }

    public AccessProfile? AccessProfile { get; set; }

    public int RoleId { get; set; }

    public Role? Role { get; set; }
}
