using DotNetSandbox.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace DotNetSandbox.Models.DTOs.Output
{
    public class UserTransactionDTO
    {
        [Required(ErrorMessage = "UserId is required")]
        public int UserId { get; set; }
        public string? Username { get; set; }
        public int TotalCount { get; set; }
        public List<TransactionDTO> Transactions { get; set; } = new();
    }
    public class TransactionDTO
    {
        public int BalanceId { get; set; }
        public decimal Amount { get; set; }
        public string? Type { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? @Operator { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
