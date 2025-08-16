using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DotNetSandbox.Models.DTOs.Input
{
    public class DeleteUserRequest
    {
        [Required(ErrorMessage = "UserId is required")]
        [Range(1, 999, ErrorMessage = "UserId should be 1-999")]
        public int? UserId { get; set; }
    }
}
