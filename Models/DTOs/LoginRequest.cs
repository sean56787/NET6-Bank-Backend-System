using System.ComponentModel.DataAnnotations;

namespace DotNetSandbox.Models.DTOs
{
    public class LoginRequest : IValidatableObject
    {

        [StringLength(20, MinimumLength = 3, ErrorMessage = "username should be 3~20 chars")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "pwd is required")]
        [MinLength(6, ErrorMessage = "pwd should be at least 6 chars")]
        public string? Password { get; set; }

        [MinLength(6, ErrorMessage = "mail should be at least 6 chars")]
        public string? Email { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(string.IsNullOrWhiteSpace(Username) && string.IsNullOrWhiteSpace(Email))
            {
                yield return new ValidationResult(
                    "username or email must be provided",
                    new[] { nameof(Username), nameof(Email) }
                );
            }
        }
    }
}
