using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuthModuleDM.Entities
{
    public class Users
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string NormalizedUserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string NormalizedEmail { get; set; } = string.Empty;

        public bool IsEmailConfirmed { get; set; }

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string SecurityStamp { get; set; } = Guid.NewGuid().ToString();

        public string PhoneNumber { get; set; } = string.Empty;

        public bool PhoneNumberConfirmed { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public DateTime? LockoutEnd { get; set; }

        public bool LockoutEnabled { get; set; }

        public int AccessFailedCount { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties

        public ICollection<UserRoles> UserRoles { get; set; } = [];

        public ICollection<UserClaims> UserClaims { get; set; } = [];
    }
}
