using ResumeEnhancer.Core.DomainLibrary.DomainModel;

namespace ResumeEnhancer.ProfilingModule.DM.Entities;

public sealed class UserAccessProfile : BusinessRelation
{
    public int UserId { get; set; }

    public User? User { get; set; }

    public int AccessProfileId { get; set; }

    public AccessProfile? AccessProfile { get; set; }

    public DateTime AssignedOnUtc { get; set; } = DateTime.UtcNow;
}
