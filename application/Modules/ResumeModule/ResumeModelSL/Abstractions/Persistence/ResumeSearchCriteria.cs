namespace ResumeEnhancer.ResumeModule.SL.Abstractions.Persistence;

public sealed class ResumeSearchCriteria
{
    public IReadOnlyList<int>? Ids { get; set; }

    public int? UserId { get; set; }

    public string? SearchText { get; set; }

    public string? ResumeTemplate { get; set; }

    public int? TemplateId { get; set; }

    public bool? HasPhoto { get; set; }

    public DateTime? CreatedFromUtc { get; set; }

    public DateTime? CreatedToUtc { get; set; }

    public DateTime? UpdatedFromUtc { get; set; }

    public DateTime? UpdatedToUtc { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 25;

    public ResumeSortBy SortBy { get; set; } = ResumeSortBy.UpdatedDate;

    public ResumeSortDirection SortDirection { get; set; } = ResumeSortDirection.Descending;
}

public enum ResumeSortBy
{
    Id = 0,
    Title = 1,
    CreatedDate = 2,
    UpdatedDate = 3,
    ResumeTemplate = 4
}

public enum ResumeSortDirection
{
    Ascending = 0,
    Descending = 1
}

