using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using DotNetSandbox.Data;
using DotNetSandbox.Models.DTOs.Output;
using DotNetSandbox.Services.CustomResponse;
using DotNetSandbox.Models.Data;

namespace DotNetSandbox.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        
        public UserService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public bool Register(string? username, string? password, string? email)
        {
            bool result = _context.Users.Any(u => u.Username == username) || _context.Users.Any(u => u.Email == email); //只負責找
            if (result)
                return false;

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var newUser = new User
            {
                Username = username,
                Password = hashedPassword,
                Email = email,
                IsVerified = false,
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();
            return true;
        }

        public ServiceResponse<UserDTO> Login(string? username, string? password, string? email)
        {
            User? user = null;

            if (!string.IsNullOrWhiteSpace(username))
                user = _context.Users.FirstOrDefault(u => u.Username == username);
            else if (!string.IsNullOrWhiteSpace(email))
                user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                return ServiceResponse<UserDTO>.NotFound(message: "user not found");
            }
            else if(user.IsVerified == false || user.IsActive == false){
                return ServiceResponse<UserDTO>.Error(message: "user not verified or has been frozen", statusCode:401);
            }
            else if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return ServiceResponse<UserDTO>.Error(message: "account & password doesn't match", statusCode:401);
            }

            var userDTO = new UserDTO
            {
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString(),
                IsActive = user.IsActive
            };

            return ServiceResponse<UserDTO>.Ok(data:userDTO, message: "login success");
        }

        public bool Verify(string username)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return false;
            user.IsVerified = true;
            user.IsActive = true;
            _context.SaveChanges(); // 寫入
            return true;
        }
    }
}
