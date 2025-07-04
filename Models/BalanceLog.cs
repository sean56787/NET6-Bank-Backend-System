using DotNetSandbox.Models.Enums;

namespace DotNetSandbox.Models
{
    public class BalanceLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public BalanceType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Operator { get; set; } = string.Empty;
    }
}
