using DotNetSandbox.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetSandbox.Models.Data
{
    public class BalanceLog
    {
        [Key]
        public int BalanceId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public decimal? BalanceBefore { get; set; }
        public decimal Balance {get;set;}
        public BalanceType Type { get; set; } = BalanceType.Default;
        public string? Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Operator { get; set; } = string.Empty;
        public int? TransactionId { get; set; }

        //navigate
        public User User { get; set; }
    }
}
