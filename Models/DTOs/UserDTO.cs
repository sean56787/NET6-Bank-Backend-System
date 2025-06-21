namespace DotNetSandbox.Models.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string Role { get; set; } = "";
        public string Email { get; set; } = "";
        public bool IsVerified { get; set; }
    }
}
