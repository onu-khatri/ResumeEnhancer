using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResumeModuleDM.Entities;

namespace ResumeModulePL.Configurations;

public sealed class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.HasKey(address => address.Id);

        builder.Property(address => address.StreetAddress).HasMaxLength(200);
        builder.Property(address => address.City).HasMaxLength(100);
        builder.Property(address => address.State).HasMaxLength(100);
        builder.Property(address => address.Country).HasMaxLength(100);
        builder.Property(address => address.ZipCode).HasMaxLength(20);

        builder.HasOne(address => address.PersonalInformation)
            .WithOne(personalInformation => personalInformation.Address)
            .HasForeignKey<Address>(address => address.PersonalInformationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
