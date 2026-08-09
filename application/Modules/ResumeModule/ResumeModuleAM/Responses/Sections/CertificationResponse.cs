namespace ResumeModuleAM.Responses;

public sealed class CertificationResponse
{
    public int Id { get; set; }

    public string CertificationName { get; set; } = string.Empty;

    public string? IssuingOrganization { get; set; }

    public DateTime? IssueDate { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public string? CredentialId { get; set; }

    public string? CredentialUrl { get; set; }

    public string? Description { get; set; }
}
