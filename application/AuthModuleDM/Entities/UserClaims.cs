using System;
using System.Collections.Generic;
using System.Text;

namespace AuthModuleDM.Entities;
public class UserClaims
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string ClaimType { get; set; } = string.Empty;

    public string ClaimValue { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public Users User { get; set; } = null!;
}
