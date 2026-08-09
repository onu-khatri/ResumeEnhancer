using System.ComponentModel.DataAnnotations;

namespace ResumeModuleAM.Requests;

public sealed class CertificationRequest
{
    [Range(0, int.MaxValue)]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string CertificationName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? IssuingOrganization { get; set; }

    public DateTime? IssueDate { get; set; }

    public DateTime? ExpirationDate { get; set; }

    [MaxLength(100)]
    public string? CredentialId { get; set; }

    [Url]
    [MaxLength(500)]
    public string? CredentialUrl { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }
}
