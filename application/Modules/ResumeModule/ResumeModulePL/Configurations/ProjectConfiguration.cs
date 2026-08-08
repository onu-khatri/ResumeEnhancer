using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResumeModuleDM.Entities;

namespace ResumeModulePL.Configurations;

public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasKey(project => project.Id);

        builder.Property(project => project.ProjectName).HasMaxLength(200).IsRequired();
        builder.Property(project => project.Role).HasMaxLength(150);
        builder.Property(project => project.Description).HasMaxLength(1000);
        builder.Property(project => project.TechnologiesUsed).HasMaxLength(500);

        builder.HasOne(project => project.Resume)
            .WithMany(resume => resume.Projects)
            .HasForeignKey(project => project.ResumeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
