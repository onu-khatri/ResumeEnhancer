using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using AuthModuleDM.Entities;
using Persistence;
namespace AuthModulePL.Context;

public static class AuthModuleEntitySetExtensions
{
    public static DbSet<Users> Registers(this AppDbContext context) => context.Set<Users>();
    public static DbSet<RoleClaims> RoleClaims(this AppDbContext context) => context.Set<RoleClaims>();
    public static DbSet<Roles> Roles(this AppDbContext context) => context.Set<Roles>();
    public static DbSet<UserClaims> UserClaims(this AppDbContext context) => context.Set<UserClaims>();
    public static DbSet<UserRoles> UserRoles(this AppDbContext context) => context.Set<UserRoles>();
}
