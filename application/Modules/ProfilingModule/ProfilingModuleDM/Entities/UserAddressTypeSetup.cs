using System.ComponentModel.DataAnnotations;
using ResumeEnhancer.Core.DomainLibrary.DomainModel;

namespace ResumeEnhancer.ProfilingModule.DM.Entities;

public sealed class UserAddressTypeSetup : SetupEntity, IHasOrderedValues
{
    [MaxLength(200)]
    public string DisplayName { get; set; } = string.Empty;

    public int Order { get; set; }

    public ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();
}
