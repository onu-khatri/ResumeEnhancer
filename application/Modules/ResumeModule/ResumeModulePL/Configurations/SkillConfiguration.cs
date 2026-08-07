using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResumeModuleDM.Entities;

namespace ResumeModulePL.Configurations;

public sealed class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        builder.ToTable("Skills", "resume");

        builder.HasKey(skill => skill.Id);

        builder.Property(skill => skill.SkillName).HasMaxLength(100).IsRequired();
        builder.Property(skill => skill.ProficiencyLevel).HasMaxLength(100);
        builder.Property(skill => skill.YearsOfExperience).HasPrecision(4, 1);
        builder.Property(skill => skill.Description).HasMaxLength(500);

        builder.HasOne(skill => skill.Resume)
            .WithMany(resume => resume.Skills)
            .HasForeignKey(skill => skill.ResumeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
