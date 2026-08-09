using System.ComponentModel.DataAnnotations;

namespace ResumeModuleAM.Requests;

public sealed class DeleteResumesRequest
{
    [Required]
    public List<int> ResumeIds { get; set; } = [];
}
