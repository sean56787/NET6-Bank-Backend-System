using DotNetSandbox.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetSandbox.Models.Data
{
    public class BalanceLog
    {
        //主鍵
        [Key]
        public int Id { get; set; }
        // 外鍵
        public decimal Amount { get; set; }
        public BalanceType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Operator { get; set; } = string.Empty;
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
