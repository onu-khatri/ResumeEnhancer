using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResumeEnhancer.ResumeModule.DM.Entities;

namespace ResumeEnhancer.ResumeModule.PL.Configurations;

public sealed class EducationConfiguration : IEntityTypeConfiguration<Education>
{
    public void Configure(EntityTypeBuilder<Education> builder)
    {
        builder.HasKey(education => education.Id);

        builder.Property(education => education.Degree).HasMaxLength(200);
        builder.Property(education => education.Institution).HasMaxLength(200);
        builder.Property(education => education.City).HasMaxLength(100);
        builder.Property(education => education.State).HasMaxLength(100);
        builder.Property(education => education.Description).HasMaxLength(1000);
        builder.Property(education => education.Percentage).HasPrecision(5, 2);
        builder.Property(education => education.Grade).HasMaxLength(50);

        builder.HasOne(education => education.Resume)
            .WithMany(resume => resume.Education)
            .HasForeignKey(education => education.ResumeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

