using System.ComponentModel.DataAnnotations;

namespace ResumeEnhancer.ResumeModule.AM.Requests;

public sealed class ResumeSearchRequest
{
    public List<int>? Ids { get; set; }

    public int? UserId { get; set; }

    [MaxLength(200)]
    public string? SearchText { get; set; }

    [MaxLength(100)]
    public string? ResumeTemplate { get; set; }

    public int? TemplateId { get; set; }

    public bool? HasPhoto { get; set; }

    public DateTime? CreatedFromUtc { get; set; }

    public DateTime? CreatedToUtc { get; set; }

    public DateTime? UpdatedFromUtc { get; set; }

    public DateTime? UpdatedToUtc { get; set; }

    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 25;

    [EnumDataType(typeof(ResumeSearchSortBy))]
    public ResumeSearchSortBy SortBy { get; set; } = ResumeSearchSortBy.UpdatedDate;

    [EnumDataType(typeof(SortDirection))]
    public SortDirection SortDirection { get; set; } = SortDirection.Descending;
}

