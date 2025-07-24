using System.ComponentModel.DataAnnotations;

namespace DotNetSandbox.Models.DTOs.Input
{
    public class SoloTransactionRequest
    {
        [Required(ErrorMessage = "Required UserId")]
        public int UserId { get; set; }

        [Required(ErrorMessage ="Required LogId")]
        public int BalanceLogId { get; set; }
    }
}
