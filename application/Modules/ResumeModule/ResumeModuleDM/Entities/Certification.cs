using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResumeModuleDM.Entities;

public class Certification
{
    [Key]
    public int Id { get; set; }

    public int ResumeId { get; set; }

    [ForeignKey(nameof(ResumeId))]
    public Resume Resume { get; set; } = null!;

    [MaxLength(200)]
    public string CertificationName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? IssuingOrganization { get; set; }

    public DateTime? IssueDate { get; set; }

    public DateTime? ExpirationDate { get; set; }

    [MaxLength(100)]
    public string? CredentialId { get; set; }

    [MaxLength(500)]
    public string? CredentialUrl { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }
}
