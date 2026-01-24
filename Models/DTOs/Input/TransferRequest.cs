using DotNetSandbox.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DotNetSandbox.Models.DTOs.Input
{
    public class TransferRequest
    {
        [Required(ErrorMessage = "Required FromUserName")]
        public int FromUserId { get; set; }

        [Required(ErrorMessage = "Required ToUserName")]
        public int ToUserId { get; set; }

        [Required(ErrorMessage = "Required Amount")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Required BalanceType")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BalanceType Type { get; set; } = BalanceType.Default;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Required RequestKey")]
        public string RequestKey { get; set; }
    }
}
