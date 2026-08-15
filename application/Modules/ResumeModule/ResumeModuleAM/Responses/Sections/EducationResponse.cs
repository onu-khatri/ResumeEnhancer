namespace ResumeEnhancer.ResumeModule.AM.Responses;

public sealed class EducationResponse
{
    public int Id { get; set; }

    public int? PassingYear { get; set; }

    public string? Degree { get; set; }

    public string? Institution { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? Description { get; set; }

    public decimal? Percentage { get; set; }

    public string? Grade { get; set; }

    public bool IsCurrent { get; set; }
}

