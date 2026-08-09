using System.ComponentModel.DataAnnotations;

namespace ResumeModuleAM.Requests;

public sealed class AddressRequest
{
    [Range(0, int.MaxValue)]
    public int Id { get; set; }

    [MaxLength(200)]
    public string? StreetAddress { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? State { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; }

    [MaxLength(20)]
    public string? ZipCode { get; set; }
}
