using System.ComponentModel.DataAnnotations;

namespace ResumeEnhancer.ResumeModule.AM.Requests;

public sealed class DeleteResumesRequest
{
    [Required]
    public List<int> ResumeIds { get; set; } = [];
}

