using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResumeModuleDM.Entities;

namespace ResumeModulePL.Configurations;

public sealed class WorkExperienceConfiguration : IEntityTypeConfiguration<WorkExperience>
{
    public void Configure(EntityTypeBuilder<WorkExperience> builder)
    {
        builder.ToTable("WorkExperiences", "resume");

        builder.HasKey(workExperience => workExperience.Id);

        builder.Property(workExperience => workExperience.JobTitle).HasMaxLength(150);
        builder.Property(workExperience => workExperience.CompanyName).HasMaxLength(200);
        builder.Property(workExperience => workExperience.Location).HasMaxLength(200);
        builder.Property(workExperience => workExperience.Description).HasMaxLength(1000);

        builder.HasOne(workExperience => workExperience.Resume)
            .WithMany(resume => resume.WorkExperiences)
            .HasForeignKey(workExperience => workExperience.ResumeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
