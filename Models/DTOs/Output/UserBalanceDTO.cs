using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DotNetSandbox.Models.DTOs.Output
{
    public class UserBalanceDTO
    {
        [Required(ErrorMessage = "Username is required")]
        public string? Username { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Balance { get; set; }
    }
}
