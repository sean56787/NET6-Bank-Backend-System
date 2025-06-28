using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DotNetSandbox.Models.DTOs
{
    public class CreateUserRequest
    {
        [Required(ErrorMessage = "username is required")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "username should be 3~20 chars")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "pwd is required")]
        [MinLength(6, ErrorMessage = "pwd should be at least 6 chars")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "email is required")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "invalid email format")]
        public string? Email { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        [Required(ErrorMessage = "role is required")]
        public User.UserRole? Role { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email))
            {
                yield return new ValidationResult(
                    "username and email must be provided",
                    new[] { nameof(Username), nameof(Email) }
                );
            }
        }
    }
}
