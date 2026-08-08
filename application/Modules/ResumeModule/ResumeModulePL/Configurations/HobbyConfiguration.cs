using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResumeModuleDM.Entities;

namespace ResumeModulePL.Configurations;

public sealed class HobbyConfiguration : IEntityTypeConfiguration<Hobby>
{
    public void Configure(EntityTypeBuilder<Hobby> builder)
    {
        builder.HasKey(hobby => hobby.Id);

        builder.Property(hobby => hobby.HobbyName).HasMaxLength(100).IsRequired();
        builder.Property(hobby => hobby.Description).HasMaxLength(500);

        builder.HasOne(hobby => hobby.PersonalInformation)
            .WithMany(personalInformation => personalInformation.Hobbies)
            .HasForeignKey(hobby => hobby.PersonalInformationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
