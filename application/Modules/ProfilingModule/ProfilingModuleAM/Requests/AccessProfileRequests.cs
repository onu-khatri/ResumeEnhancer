using System.ComponentModel.DataAnnotations;

namespace ResumeEnhancer.ProfilingModule.AM.Requests;

public sealed class CreateAccessProfileRequest
{
    [Required]
    [MaxLength(100)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string DisplayName { get; set; } = string.Empty;

    public List<int> RoleIds { get; set; } = [];
}

public sealed class UpdateAccessProfileRequest
{
    [Required]
    [MaxLength(100)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string DisplayName { get; set; } = string.Empty;

    public bool ObsoleteFlag { get; set; }

    public List<int> RoleIds { get; set; } = [];
}
