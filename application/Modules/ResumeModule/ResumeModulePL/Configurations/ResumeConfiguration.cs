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
        builder.Property(resume => resume.TemplateId);
        builder.Property(resume => resume.UserId).IsRequired();

        builder.HasIndex(resume => resume.TemplateId);
        builder.HasIndex(resume => resume.UserId);

        builder.HasOne(resume => resume.Template)
            .WithMany()
            .HasForeignKey(resume => resume.TemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(resume => resume.User)
            .WithMany()
            .HasForeignKey(resume => resume.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

