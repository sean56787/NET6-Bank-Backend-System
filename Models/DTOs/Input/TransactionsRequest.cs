using DotNetSandbox.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DotNetSandbox.Models.DTOs.Input
{
    public class TransactionsRequest
    {
        [Required(ErrorMessage = "Required UserId")]
        public int UserId { get; set; }
        public string? @Operator { get; set; }

        [Required(ErrorMessage = "Required BalanceType")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BalanceType? BalanceType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Page = 1;
        public int PageSize = 10;
    }
}
