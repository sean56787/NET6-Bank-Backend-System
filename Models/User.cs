namespace DotNetSandbox.Models
{
    

    public class User
    {
        public enum UserRole
        {
            user,
            admin,
        }

        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public UserRole Role { get; set; } = UserRole.user;
        public bool Isverified { get; set; } = false;
        public string Email { get; set; } = "";
    }
}
