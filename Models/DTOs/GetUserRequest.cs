using System.ComponentModel.DataAnnotations;

namespace DotNetSandbox.Models.DTOs
{
    public class GetUserRequest
    {
        [StringLength(20, MinimumLength = 3, ErrorMessage = "username should be 3~20 chars")]
        public string? Username { get; set; }

        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "invalid email format")]
        public string? Email { get; set; }

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
