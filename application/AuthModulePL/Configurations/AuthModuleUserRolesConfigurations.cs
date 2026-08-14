using System;
using System.Collections.Generic;
using System.Text;
using AuthModuleDM.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace AuthModulePL.Configurations;

public sealed class AuthModuleUserRoleConfigurations
    : IEntityTypeConfiguration<UserRoles>
{
    public void Configure(EntityTypeBuilder<UserRoles> builder)
    {
        builder.HasKey(x => new //composite primary-key
        {
            x.UserId,
            x.RoleId
        });

        builder.HasOne(UserRoles => UserRoles.User)
               .WithMany(users => users.UserRoles)
               .HasForeignKey(UserRoles => UserRoles.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(UserRoles => UserRoles.Role)
               .WithMany(Roles => Roles.UserRoles)
               .HasForeignKey(UserRoles => UserRoles.RoleId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
