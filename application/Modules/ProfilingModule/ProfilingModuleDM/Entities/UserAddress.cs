using System.ComponentModel.DataAnnotations;
using ResumeEnhancer.Core.DomainLibrary.DomainModel;
namespace ResumeEnhancer.ProfilingModule.DM.Entities;

public sealed class UserAddress : BusinessEntity
{
    public int UserId { get; set; }

    public User? User { get; set; }

    public int AddressTypeId { get; set; }

    public UserAddressTypeSetup? AddressType { get; set; }

    [MaxLength(200)]
    public string? AddressLine1 { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; }
}
