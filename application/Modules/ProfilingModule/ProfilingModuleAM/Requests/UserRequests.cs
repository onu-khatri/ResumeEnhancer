using System.ComponentModel.DataAnnotations;

namespace ResumeEnhancer.ProfilingModule.AM.Requests;

public sealed class CreateUserRequest
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(320)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? BillingAddressLine1 { get; set; }

    [MaxLength(100)]
    public string? BillingCity { get; set; }

    [MaxLength(100)]
    public string? BillingCountry { get; set; }

    [MaxLength(200)]
    public string? CommunicationAddressLine1 { get; set; }

    [MaxLength(100)]
    public string? CommunicationCity { get; set; }

    [MaxLength(100)]
    public string? CommunicationCountry { get; set; }

    public bool IsDeactivated { get; set; } = false;

    public List<int> AccessProfileIds { get; set; } = [];
}

public sealed class UpdateUserRequest
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(320)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? BillingAddressLine1 { get; set; }

    [MaxLength(100)]
    public string? BillingCity { get; set; }

    [MaxLength(100)]
    public string? BillingCountry { get; set; }

    [MaxLength(200)]
    public string? CommunicationAddressLine1 { get; set; }

    [MaxLength(100)]
    public string? CommunicationCity { get; set; }

    [MaxLength(100)]
    public string? CommunicationCountry { get; set; }

    public bool IsDeactivated { get; set; }

    public List<int> AccessProfileIds { get; set; } = [];
}
