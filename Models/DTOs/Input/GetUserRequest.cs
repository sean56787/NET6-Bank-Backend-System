using System.ComponentModel.DataAnnotations;

namespace DotNetSandbox.Models.DTOs.Input
{
    public class GetUserRequest
    {
        [Range(1, 999, ErrorMessage = "UserId should be 1~999")]
        public int? UserId { get; set; }

        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "invalid email format")]
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
