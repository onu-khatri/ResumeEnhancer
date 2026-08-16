using System.ComponentModel.DataAnnotations;
using ResumeEnhancer.Core.DomainLibrary.DomainModel;

namespace ResumeEnhancer.ProfilingModule.DM.Entities;

public sealed class User : BusinessEntity, IDeactivateable
{
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(320)]
    public string Email { get; set; } = string.Empty;

    public bool IsDeactivated { get; set; } = false;

    public ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();

    public ICollection<UserAccessProfile> UserAccessProfiles { get; set; } = new List<UserAccessProfile>();
}
