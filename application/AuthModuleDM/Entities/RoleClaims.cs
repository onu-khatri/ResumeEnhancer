using System;
using System.Collections.Generic;
using System.Text;

namespace AuthModuleDM.Entities;
public class RoleClaims
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public string ClaimType { get; set; } = string.Empty;

    public string ClaimValue { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public Roles Role { get; set; } = null!;
}
