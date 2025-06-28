using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DotNetSandbox.Models.DTOs
{
    public class UpdateUserRequest
    {
        [Required(ErrorMessage = "username is required")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "username should be 3~20 chars")]
        public string? Username { get; set; }

        [MinLength(6, ErrorMessage = "pwd should be at least 6 chars")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "email is required")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "invalid email format")]
        public string? Email { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public User.UserRole? Role { get; set; }

        public bool? Isverified { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Username) && string.IsNullOrWhiteSpace(Email))
            {
                yield return new ValidationResult(
                    "username or email must be provided",
                    new[] { nameof(Username), nameof(Email) }
                );
            }
        }
    }
}
