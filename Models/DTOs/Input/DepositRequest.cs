using DotNetSandbox.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DotNetSandbox.Models.DTOs.Input
{
    public class DepositRequest
    {
        [Required(ErrorMessage = "Required UserId")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Required Amount")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Required BalanceType")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BalanceType Type { get; set; } = BalanceType.Default;

        public string? Description { get; set; }

        [Required]
        [StringLength(64)]
        public string RequestKey { get; set; }
    }
}
