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
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public BalanceType Type { get; set; }
        public string? Description { get; set; }
        public string? @Operator { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
