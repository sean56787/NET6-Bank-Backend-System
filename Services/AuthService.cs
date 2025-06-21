using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using DotNetSandbox.Data;
using DotNetSandbox.Models;
using DotNetSandbox.Models.DTOs;

namespace DotNetSandbox.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public bool Register(string? username, string? password, string? email)
        {
            if(_context.Users.Any(u => u.Username == username))
            {
                return false;
            }

            var newUser = new User
            {
                Username = username,
                Password = password,
                Email = email,
                Isverified = false,
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();
            return true;
        }

        public User? Login(string? username, string? password, string? email)
        {
            User? user = null;

            if (!string.IsNullOrWhiteSpace(username))
                user = _context.Users.FirstOrDefault(u => u.Username == username);
            else if (!string.IsNullOrWhiteSpace(email))
                user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null) return null;
            if (user.Password != password) return null;
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
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }

        public List<UserDTO> GetAllUsers()
        {
            var allUsers = _context.Users.Select(u => new UserDTO
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                Role = u.Role.ToString(),
                IsVerified = u.Isverified
            }).ToList();

            return allUsers;
        }
    }
}
