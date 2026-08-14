using System;
using System.Collections.Generic;
using System.Text;

namespace AuthModuleDM.Entities;

public class UserRoles
{
    public int UserId { get; set; }

    public int RoleId { get; set; }

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties one to many relationship maintains both user & role
    public Users User { get; set; } = null!;

    public Roles Role { get; set; } = null!;
}
