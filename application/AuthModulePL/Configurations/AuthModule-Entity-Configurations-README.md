# Auth Module — Entity Configurations

## Overview

This module implements a custom authentication and authorization data model using:

- ASP.NET Core
- Entity Framework Core
- SQL Server
- JWT-based Authentication
- Custom User and Role Management
- Custom Claims and Role Claims
- Explicit `UserRoles` join entity

> **Note:** ASP.NET Core Identity is not used in this module. All authentication and authorization entities are custom-built.

---

## 1. Entity Relationship Overview

The Auth Module contains five primary entities:

```text
Users
  │
  ├────────────── 1 : M ────────────── UserClaims
  │
  └────────────── 1 : M ────────────── UserRoles
                                           │
                                           │ M : 1
                                           ▼
                                         Roles
                                           │
                                           └──── 1 : M ───── RoleClaims
```

### Complete Relationship

```text
                    ┌─────────────────┐
                    │      Users      │
                    │─────────────────│
                    │ Id              │
                    │ FirstName       │
                    │ LastName        │
                    │ UserName        │
                    │ Email           │
                    │ PasswordHash    │
                    │ ...             │
                    └────────┬────────┘
                             │
                         1 : M
                             │
              ┌──────────────┴──────────────┐
              │                             │
              ▼                             ▼
      ┌─────────────────┐           ┌─────────────────┐
      │    UserRoles    │           │   UserClaims    │
      │─────────────────│           │─────────────────│
      │ UserId          │           │ Id              │
      │ RoleId          │           │ UserId          │
      │ AssignedAt      │           │ ClaimType       │
      └────────┬────────┘           │ ClaimValue      │
               │                    └─────────────────┘
               │
             M : 1
               │
               ▼
      ┌─────────────────┐
      │      Roles      │
      │─────────────────│
      │ Id              │
      │ Name            │
      │ NormalizedName  │
      │ Description     │
      │ IsActive        │
      └────────┬────────┘
               │
             1 : M
               │
               ▼
      ┌─────────────────┐
      │   RoleClaims    │
      │─────────────────│
      │ Id              │
      │ RoleId          │
      │ ClaimType       │
      │ ClaimValue      │
      └─────────────────┘
```

---

## 2. Users Configuration

The `Users` entity represents application users.

```csharp
internal class AuthModuleUserConfigurations
    : IEntityTypeConfiguration<Users>
{
    public void Configure(EntityTypeBuilder<Users> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasMany(x => x.UserRoles)
               .WithOne(x => x.User)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.UserClaims)
               .WithOne(x => x.User)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
```

### Relationships

- `Users 1 : M UserRoles`
- `Users 1 : M UserClaims`

---

## 3. Roles Configuration

The `Roles` entity stores all available application roles.

Example roles:

```text
Admin
Manager
User
Developer
Moderator
```

Configuration:

```csharp
internal class AuthModuleRoleConfigurations
    : IEntityTypeConfiguration<Roles>
{
    public void Configure(EntityTypeBuilder<Roles> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasMany(x => x.UserRoles)
               .WithOne(x => x.Role)
               .HasForeignKey(x => x.RoleId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.RoleClaims)
               .WithOne(x => x.Role)
               .HasForeignKey(x => x.RoleId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
```

### Relationships

- `Roles 1 : M UserRoles`
- `Roles 1 : M RoleClaims`

---

## 4. UserRoles Configuration

`UserRoles` is an explicit join entity between `Users` and `Roles`.

It represents:

```text
Users M : M Roles
```

through:

```text
Users 1 : M UserRoles M : 1 Roles
```

Configuration:

```csharp
internal class AuthModuleUserRoleConfigurations
    : IEntityTypeConfiguration<UserRoles>
{
    public void Configure(EntityTypeBuilder<UserRoles> builder)
    {
        builder.HasKey(x => new
        {
            x.UserId,
            x.RoleId
        });

        builder.HasOne(x => x.User)
               .WithMany(x => x.UserRoles)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Role)
               .WithMany(x => x.UserRoles)
               .HasForeignKey(x => x.RoleId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
```

### Composite Primary Key

```csharp
builder.HasKey(x => new
{
    x.UserId,
    x.RoleId
});
```

This prevents the same role from being assigned to the same user more than once.

Example:

```text
UserId    RoleId
----------------
1         1
1         2
2         1
```

But:

```text
UserId    RoleId
----------------
1         1
1         1    ❌ Duplicate
```

---

## 5. UserClaims Configuration

`UserClaims` stores claims that belong directly to a specific user.

Example:

```text
User: Montu

country    = India
department = IT
permission = create_user
```

Configuration:

```csharp
internal class AuthModuleUserClaimConfigurations
    : IEntityTypeConfiguration<UserClaims>
{
    public void Configure(EntityTypeBuilder<UserClaims> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ClaimType)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.ClaimValue)
               .IsRequired()
               .HasMaxLength(500);

        builder.HasOne(x => x.User)
               .WithMany(x => x.UserClaims)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
```

### Relationship

```text
Users 1 : M UserClaims
```

Example database records:

```text
Id    UserId    ClaimType      ClaimValue
------------------------------------------
1     10        country        India
2     10        department     IT
3     10        permission     create_user
```

---

## 6. RoleClaims Configuration

`RoleClaims` stores claims associated with a role.

Example:

```text
Admin
 ├── permission = create_user
 ├── permission = update_user
 └── permission = delete_user
```

Configuration:

```csharp
internal class AuthModuleRoleClaimConfigurations
    : IEntityTypeConfiguration<RoleClaims>
{
    public void Configure(EntityTypeBuilder<RoleClaims> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ClaimType)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.ClaimValue)
               .IsRequired()
               .HasMaxLength(500);

        builder.HasOne(x => x.Role)
               .WithMany(x => x.RoleClaims)
               .HasForeignKey(x => x.RoleId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
```

### Relationship

```text
Roles 1 : M RoleClaims
```

Example:

```text
Id    RoleId    ClaimType     ClaimValue
-----------------------------------------
1     1         permission    create_user
2     1         permission    update_user
3     1         permission    delete_user
```

---

## 7. Configuration Summary

| Entity | Relationship | Purpose |
|---|---|---|
| `Users` | 1 : M `UserRoles` | Assign multiple roles to a user |
| `Users` | 1 : M `UserClaims` | Store user-specific claims |
| `Roles` | 1 : M `UserRoles` | Allow a role to belong to multiple users |
| `Roles` | 1 : M `RoleClaims` | Store role-specific claims |
| `UserRoles` | M : 1 `Users` | User side of role assignment |
| `UserRoles` | M : 1 `Roles` | Role side of role assignment |
| `UserClaims` | M : 1 `Users` | User-specific authorization data |
| `RoleClaims` | M : 1 `Roles` | Role-specific authorization data |

---

## 8. Why `WithMany()` Was Not Used in the Original Configuration

This configuration is incorrect for an explicit `UserRoles` entity:

```csharp
builder.HasMany(x => x.UserRoles)
       .WithMany(x => x.User);
```

`HasMany().WithMany()` is generally used when EF Core manages the many-to-many join automatically.

In this architecture, `UserRoles` is a real entity:

```text
UserRoles
├── UserId
├── RoleId
└── AssignedAt
```

Therefore, the correct relationship is:

```csharp
builder.HasMany(x => x.UserRoles)
       .WithOne(x => x.User)
       .HasForeignKey(x => x.UserId);
```

---

## 9. Delete Behavior

The relationships use:

```csharp
.OnDelete(DeleteBehavior.Cascade);
```

For example:

```text
User
 │
 ├── UserRole
 ├── UserRole
 └── UserClaim
```

If a user is deleted, the associated `UserRoles` and `UserClaims` records are automatically deleted.

Similarly:

```text
Role
 │
 ├── UserRole
 └── RoleClaim
```

Deleting the role removes its associated `UserRoles` and `RoleClaims`.

> Use cascade delete carefully if your application needs historical or audit records to remain after deleting users or roles.

---

## 10. Password Storage

Because this project does not use ASP.NET Core Identity, password management is handled by the application.

The database should store:

```text
PasswordHash
```

and never:

```text
Password
ConfirmPassword
```

Passwords must be hashed before being stored.

Correct flow:

```text
Register Request
      │
      ├── Password
      └── ConfirmPassword
             │
             ▼
       Password Hashing
             │
             ▼
        PasswordHash
             │
             ▼
          Database
```

`Password` and `ConfirmPassword` should belong to a request DTO rather than the database entity.

---

## 11. JWT Authorization Flow

The configured entities support the following custom JWT authorization flow:

```text
                    Login Request
                         │
                         ▼
                       Users
                         │
                         ▼
                 Verify PasswordHash
                         │
                         ▼
                     UserRoles
                         │
                         ▼
                       Roles
                         │
                ┌────────┴────────┐
                ▼                 ▼
           RoleClaims         UserClaims
                │                 │
                └────────┬────────┘
                         ▼
                  Collect Claims
                         │
                         ▼
                    Generate JWT
                         │
                         ▼
                    Access Token
```

JWT claims can include:

```text
sub          → User ID
name         → User name
email        → User email
role         → Admin
permission   → create_user
permission   → delete_user
```

---

## 12. Configuration Folder Structure

Recommended structure:

```text
AuthModulePL
│
└── Configurations
    │
    ├── AuthModuleUserConfigurations.cs
    ├── AuthModuleRoleConfigurations.cs
    ├── AuthModuleUserRoleConfigurations.cs
    ├── AuthModuleUserClaimConfigurations.cs
    └── AuthModuleRoleClaimConfigurations.cs
```

Each configuration implements:

```csharp
IEntityTypeConfiguration<TEntity>
```

This keeps EF Core configuration separate from the entity classes.

---

## 13. Registering Configurations in DbContext

Configurations can be registered automatically:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.ApplyConfigurationsFromAssembly(
        typeof(AuthModuleDbContext).Assembly);
}
```

If the configuration classes are located in another assembly, use a marker type from that assembly:

```csharp
modelBuilder.ApplyConfigurationsFromAssembly(
    typeof(AuthModuleUserConfigurations).Assembly);
```

EF Core will discover implementations of:

```csharp
IEntityTypeConfiguration<TEntity>
```

automatically.

---

## 14. Final Database Design

Core tables:

```text
┌─────────────────────┐
│       Users         │
├─────────────────────┤
│ Id PK               │
│ FirstName           │
│ LastName             │
│ UserName            │
│ NormalizedUserName  │
│ Email               │
│ NormalizedEmail     │
│ PasswordHash        │
│ SecurityStamp       │
│ PhoneNumber         │
│ ...                 │
└──────────┬──────────┘
           │
     ┌─────┴─────┐
     │           │
     ▼           ▼
UserRoles    UserClaims
     │
     │
     ▼
┌─────────────────────┐
│       Roles         │
├─────────────────────┤
│ Id PK               │
│ Name                │
│ NormalizedName      │
│ Description         │
│ IsActive            │
└──────────┬──────────┘
           │
           ▼
      RoleClaims
```

---

## 15. Design Principles

This configuration follows these principles:

1. No ASP.NET Core Identity dependency.
2. Custom user management.
3. JWT-based authentication.
4. Explicit many-to-many relationship through `UserRoles`.
5. Separate user claims and role claims.
6. Composite key for `UserRoles`.
7. Explicit foreign-key configuration.
8. Explicit delete behavior.
9. Entity configuration separated from domain entities.
10. Password hashes stored instead of plaintext passwords.
11. EF Core configuration implemented through `IEntityTypeConfiguration<T>`.
12. Configurations discovered using `ApplyConfigurationsFromAssembly()`.

---

## Entity Relationship Summary

```text
Users
 │
 ├─────────────── 1 : M ─────────────── UserClaims
 │
 └─────────────── 1 : M ─────────────── UserRoles
                                           │
                                           │ M : 1
                                           ▼
                                         Roles
                                           │
                                           │ 1 : M
                                           ▼
                                      RoleClaims
```

This provides the foundation for a custom, extensible JWT Authentication and Authorization module without depending on ASP.NET Core Identity.
