using System.ComponentModel.DataAnnotations;

namespace DotNetSandbox.Models.Data
{
    public class UserBalanceTransferLog
    {
        [Key]
        public int TransferId { get; set; }
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Description { get; set; } = string.Empty;
        public int? FromBalanceLogId { get; set; }
        public int? ToBalanceLogId { get; set; }
        public BalanceLog? FromBalanceLog { get; set; }
        public BalanceLog? ToBalanceLog { get; set; }
    }
}
