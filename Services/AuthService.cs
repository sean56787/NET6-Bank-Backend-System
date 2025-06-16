using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using DotNetSandbox.Data;
using DotNetSandbox.Models;

namespace DotNetSandbox.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public bool Register(string username, string password)
        {
            if(_context.Users.Any(u => u.Username == username))
            {
                return false;
            }

            var newUser = new User
            {
                Username = username,
                Password = password,
                Isverified = false,
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();
            return true;
        }

        public User Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            return user;
        }

        public bool Verify(string username)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return false;
            user.Isverified = true;
            _context.SaveChanges(); // 寫入
            return true;
        }

        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("this_is_a_very_long_secret_key_123456");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Username == "admin" ? "admin" : "user")
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public List<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }
    }
}
