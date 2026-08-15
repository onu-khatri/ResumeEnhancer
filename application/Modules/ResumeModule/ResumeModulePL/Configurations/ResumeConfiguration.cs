using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResumeEnhancer.ResumeModule.DM.Entities;

namespace ResumeEnhancer.ResumeModule.PL.Configurations;

public sealed class ResumeConfiguration : IEntityTypeConfiguration<Resume>
{
    public void Configure(EntityTypeBuilder<Resume> builder)
    {
        builder.HasKey(resume => resume.Id);

        builder.Property(resume => resume.Title).HasMaxLength(200).IsRequired();
        builder.Property(resume => resume.Summary).HasMaxLength(2000);
        builder.Property(resume => resume.Photo).HasMaxLength(500);
        builder.Property(resume => resume.ResumeTemplate).HasMaxLength(100);
        builder.Property(resume => resume.UserId).HasMaxLength(450).IsRequired();

        builder.HasIndex(resume => resume.UserId);
    }
}

