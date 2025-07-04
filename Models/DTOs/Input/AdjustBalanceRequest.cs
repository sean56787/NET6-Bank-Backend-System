using DotNetSandbox.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DotNetSandbox.Models.DTOs.Input
{
    public class AdjustBalanceRequest
    {
        [Required(ErrorMessage = "Required Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Required Amount")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Required BalanceType")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BalanceType Type { get; set; }

        public string? Note { get; set; }
    }
}
