using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using DotNetSandbox.Data;
using DotNetSandbox.Models;
using DotNetSandbox.Models.DTOs;
using DotNetSandbox.Migrations;

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
            bool result = _context.Users.Any(u => u.Username == username) || _context.Users.Any(u => u.Email == email);
            if (result)
                return false;

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var newUser = new User
            {
                Username = username,
                Password = hashedPassword,
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
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password)) return null;
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
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim(ClaimTypes.Email, user.Email)
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

        public UserDTO? UpdateUser(UpdateUserRequest req)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == req.Username || u.Email == req.Email);

            if(user == null)
            {
                return null;
            }

            if (req.Username != null)
                user.Username = req.Username;

            if (req.Email != null)
                user.Email = req.Email;

            if (req.Password != null)
            {
                var newHashedPassword = BCrypt.Net.BCrypt.HashPassword(req.Password);
                user.Password = newHashedPassword;
            }
                
            if (req.Role != null)
                user.Role = (User.UserRole)req.Role;

            if (req.Isverified != null)
                user.Isverified = (bool)req.Isverified;

            _context.SaveChanges();

            var updatedUserDTO = new UserDTO
            {
                Username = user.Username,
                Id = user.Id,
                Role = user.Role.ToString(),
                Email = user.Email
            };

            return updatedUserDTO;
        }
        public UserDTO? CreateUser(string? username, string? password, string? email, User.UserRole? role)
        {
            var result = _context.Users.Any(u => u.Username == username || u.Email == email);
            if (result)
                return null;

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var newUser = new User
            {
                Username = username,
                Password = hashedPassword,
                Email = email,
                Isverified = false,
                Role = (User.UserRole)role,
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            var userDTO = new UserDTO
            {
                Username = newUser.Username,
                Id = newUser.Id,
                Role = newUser.Role.ToString(),
                Email = newUser.Email
            };

            return userDTO;
        }

        public UserDTO? GetUser(string? username, string? email)
        {
            var userDTO = _context.Users.
                Where(u => u.Username == username || u.Email == email).
                Select(u => new UserDTO 
                { 
                    Id = u.Id,
                    Username = u.Username,
                    Role = u.Role.ToString(),
                    Email = u.Email 
                }).FirstOrDefault();

            if (userDTO == null)
                return null;

            return userDTO;
        }

        public List<UserDTO> GetAllUsers()
        {
            var usersDTO = _context.Users.Select(u => new UserDTO
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                Role = u.Role.ToString(),
                IsVerified = u.Isverified
            }).ToList();

            return usersDTO;
        }
    }
}
