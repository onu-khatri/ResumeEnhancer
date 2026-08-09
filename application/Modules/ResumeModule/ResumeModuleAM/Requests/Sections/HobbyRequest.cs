using System.ComponentModel.DataAnnotations;

namespace ResumeModuleAM.Requests;

public sealed class HobbyRequest
{
    [Range(0, int.MaxValue)]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string HobbyName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
}
