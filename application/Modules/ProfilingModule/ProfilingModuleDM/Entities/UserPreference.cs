using System.ComponentModel.DataAnnotations;
using ResumeEnhancer.Core.DomainLibrary.DomainModel;

namespace ResumeEnhancer.ProfilingModule.DM.Entities;

public sealed class UserPreference : BusinessEntity
{
    public int UserId { get; set; }
    public User? User { get; set; }

    [MaxLength(10)]
    public string Locale { get; set; } = "en";
    public bool OnboardingCompleted { get; set; }
}
