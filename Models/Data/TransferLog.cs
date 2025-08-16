using DotNetSandbox.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace DotNetSandbox.Models.Data
{
    public class TransferLog
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

        //navigate
        //public User FromUser { get; set; }

        //public User ToUser { get; set; }

        public BalanceLog? FromBalanceLog { get; set; }

        public BalanceLog? ToBalanceLog { get; set; }
    }
}
