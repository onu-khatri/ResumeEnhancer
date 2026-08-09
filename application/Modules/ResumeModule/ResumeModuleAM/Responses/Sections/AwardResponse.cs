namespace ResumeModuleAM.Responses;

public sealed class AwardResponse
{
    public int Id { get; set; }

    public string AwardName { get; set; } = string.Empty;

    public string? IssuingOrganization { get; set; }

    public DateTime? AwardDate { get; set; }

    public string? Description { get; set; }
}
