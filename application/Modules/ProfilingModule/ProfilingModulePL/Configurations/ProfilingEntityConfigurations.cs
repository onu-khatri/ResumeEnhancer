using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResumeEnhancer.ProfilingModule.DM.Entities;
using ResumeEnhancer.ProfilingModule.DM.Enums;

namespace ResumeEnhancer.ProfilingModule.PL.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(user => user.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(user => user.LastName).HasMaxLength(100).IsRequired();
        builder.Property(user => user.Email).HasMaxLength(320).IsRequired();
        builder.Property(user => user.IsDeactivated).HasDefaultValue(false);

        builder.HasIndex(user => user.Email).IsUnique();
    }
}

public sealed class UserAddressConfiguration : IEntityTypeConfiguration<UserAddress>
{
    public void Configure(EntityTypeBuilder<UserAddress> builder)
    {
        builder.Property(address => address.AddressTypeId).IsRequired();
        builder.Property(address => address.AddressLine1).HasMaxLength(200);
        builder.Property(address => address.City).HasMaxLength(100);
        builder.Property(address => address.Country).HasMaxLength(100);

        builder.HasIndex(address => new { address.UserId, address.AddressTypeId }).IsUnique();

        builder.HasOne(address => address.User)
            .WithMany(user => user.UserAddresses)
            .HasForeignKey(address => address.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(address => address.AddressType)
            .WithMany(addressType => addressType.UserAddresses)
            .HasForeignKey(address => address.AddressTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class UserAddressTypeSetupConfiguration : IEntityTypeConfiguration<UserAddressTypeSetup>
{
    public void Configure(EntityTypeBuilder<UserAddressTypeSetup> builder)
    {
        builder.Property(addressType => addressType.Code).HasMaxLength(100).IsRequired();
        builder.Property(addressType => addressType.Description).HasMaxLength(1000).IsRequired();
        builder.Property(addressType => addressType.DisplayName).HasMaxLength(200).IsRequired();
        builder.Property(addressType => addressType.Order).IsRequired();

        builder.HasIndex(addressType => addressType.Code).IsUnique();
    }
}

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.Property(role => role.DisplayName).HasMaxLength(200).IsRequired();
        builder.Property(role => role.Order).IsRequired();
    }
}

public sealed class AccessProfileConfiguration : IEntityTypeConfiguration<AccessProfile>
{
    public void Configure(EntityTypeBuilder<AccessProfile> builder)
    {
        builder.Property(accessProfile => accessProfile.DisplayName).HasMaxLength(200).IsRequired();
        builder.Property(accessProfile => accessProfile.Order).IsRequired();
    }
}

public sealed class UserAccessProfileConfiguration : IEntityTypeConfiguration<UserAccessProfile>
{
    public void Configure(EntityTypeBuilder<UserAccessProfile> builder)
    {
        builder.HasIndex(item => new { item.UserId, item.AccessProfileId }).IsUnique();

        builder.HasOne(item => item.User)
            .WithMany(user => user.UserAccessProfiles)
            .HasForeignKey(item => item.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(item => item.AccessProfile)
            .WithMany(accessProfile => accessProfile.UserAccessProfiles)
            .HasForeignKey(item => item.AccessProfileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class AccessProfileRoleConfiguration : IEntityTypeConfiguration<AccessProfileRole>
{
    public void Configure(EntityTypeBuilder<AccessProfileRole> builder)
    {
        builder.HasIndex(item => new { item.AccessProfileId, item.RoleId }).IsUnique();

        builder.HasOne(item => item.AccessProfile)
            .WithMany(accessProfile => accessProfile.AccessProfileRoles)
            .HasForeignKey(item => item.AccessProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(item => item.Role)
            .WithMany(role => role.AccessProfileRoles)
            .HasForeignKey(item => item.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
