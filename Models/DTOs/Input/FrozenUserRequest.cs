using System.ComponentModel.DataAnnotations;

namespace DotNetSandbox.Models.DTOs.Input
{
    public class FrozenUserRequest
    {
        [Required(ErrorMessage = "username is required")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "username should be 3~20 chars")]
        public string? Username { get; set; }
    }
}
