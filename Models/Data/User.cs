using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetSandbox.Models.Data
{
    public class User
    {
        public enum UserRole
        {
            user,
            admin,
        }

        [Key]
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.user;
        public string Email { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; } = decimal.Zero;
        public bool IsVerified { get; set; } = false;
        public bool IsActive { get; set; } = true;

        //navigate
        public List<BalanceLog> BalanceLogs { get; set; } = new ();
        public List<UserBalanceTransferLog> TransferLog { get; set; } = new();
    }
}
