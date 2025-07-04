using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetSandbox.Models
{
    

    public class User
    {
        public enum UserRole
        {
            user,
            admin,
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.user;
        public string Email { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }
        public bool IsVerified { get; set; } = false;
        public bool IsActive { get; set; } = false;
    }
}
