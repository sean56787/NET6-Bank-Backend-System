using System.ComponentModel.DataAnnotations;

namespace DotNetSandbox.Models.DTOs.Input
{
    public class FrozenUserRequest
    {
        [Required(ErrorMessage = "UserId is required")]
        [Range(1, 999, ErrorMessage = "UserId should be 1-999")]
        public int? UserId { get; set; }
    }
}
