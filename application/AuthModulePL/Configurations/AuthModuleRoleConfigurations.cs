using System;
using System.Collections.Generic;
using System.Text;
using AuthModuleDM.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthModulePL.Configurations;

public sealed class AuthModuleRoleConfigurations
    : IEntityTypeConfiguration<Roles>
{
    public void Configure(EntityTypeBuilder<Roles> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasMany(Roles => Roles.UserRoles)
               .WithOne(UserRoles => UserRoles.Role)
               .HasForeignKey(UserRoles => UserRoles.RoleId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(Roles => Roles.RoleClaims)
               .WithOne(RoleClaims => RoleClaims.Role)
               .HasForeignKey(x => x.RoleId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

