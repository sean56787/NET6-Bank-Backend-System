using System.ComponentModel.DataAnnotations;

namespace DotNetSandbox.Models.DTOs.Input
{
    public class LoginRequest : IValidatableObject
    {

        [Range(1, 999, ErrorMessage = "userid should be 1~999")]
        public int? UserId { get; set; }

        [Required(ErrorMessage = "pwd is required")]
        [MinLength(6, ErrorMessage = "pwd should be at least 6 chars")]
        public string? Password { get; set; }

        [MinLength(6, ErrorMessage = "mail should be at least 6 chars")]
        public string? Email { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!UserId.HasValue && string.IsNullOrWhiteSpace(Email))
            {
                yield return new ValidationResult(
                    "username or email must be provided",
                    new[] { nameof(UserId), nameof(Email) }
                );
            }
        }
    }
}
