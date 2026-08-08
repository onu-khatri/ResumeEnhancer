using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResumeModuleDM.Entities;

namespace ResumeModulePL.Configurations;

public sealed class CertificationConfiguration : IEntityTypeConfiguration<Certification>
{
    public void Configure(EntityTypeBuilder<Certification> builder)
    {
        builder.HasKey(certification => certification.Id);

        builder.Property(certification => certification.CertificationName).HasMaxLength(200).IsRequired();
        builder.Property(certification => certification.IssuingOrganization).HasMaxLength(200);
        builder.Property(certification => certification.CredentialId).HasMaxLength(100);
        builder.Property(certification => certification.CredentialUrl).HasMaxLength(500);
        builder.Property(certification => certification.Description).HasMaxLength(1000);

        builder.HasOne(certification => certification.Resume)
            .WithMany(resume => resume.Certifications)
            .HasForeignKey(certification => certification.ResumeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
