using DotNetSandbox.Data;
using DotNetSandbox.Models.DTOs.Output;
using DotNetSandbox.Services.CustomResponse;
using DotNetSandbox.Models.Data;
using Microsoft.EntityFrameworkCore;
using DotNetSandbox.Models.DTOs.Input;
using DotNetSandbox.Services.Interfaces;

namespace DotNetSandbox.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public UserService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<ServiceResponse<UserDTO>> Register(RegisterRequest req)
        {

            bool result = await _context.Users.AnyAsync(u => u.Username == req.Username || u.Email == req.Email);
            if (result)
                return ServiceResponse<UserDTO>.Error(message: "username or email already exists", statusCode: 409);

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(req.Password);

            var newUser = new User
            {
                Username = req.Username,
                Password = hashedPassword,
                Email = req.Email,
                IsVerified = false,
                IsActive = true,
            };

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
            return ServiceResponse<UserDTO>.Ok(message: "user register success");
        }

        public async Task<ServiceResponse<UserDTO>> Login(LoginRequest req)
        {
            User? user = null;

            if (req.UserId.HasValue)
                user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == req.UserId);
            else if (!string.IsNullOrWhiteSpace(req.Email))
                user = await _context.Users.FirstOrDefaultAsync(u => u.Email == req.Email);

            if (user == null)
            {
                return ServiceResponse<UserDTO>.NotFound(message: "user not found", statusCode: 404);
            }
            else if (user.IsVerified == false || user.IsActive == false)
            {
                return ServiceResponse<UserDTO>.Error(message: "user not verified or has been frozen", statusCode: 401);
            }
            else if (!BCrypt.Net.BCrypt.Verify(req.Password, user.Password))
            {
                return ServiceResponse<UserDTO>.Error(message: "account & password doesn't match", statusCode: 401);
            }

            var userDTO = new UserDTO
            {
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString(),
                IsActive = user.IsActive
            };

            return ServiceResponse<UserDTO>.Ok(data: userDTO, message: "login success");
        }

        public async Task<ServiceResponse<UserDTO>> Verify(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return ServiceResponse<UserDTO>.NotFound(message: "user not found");
            if (user.IsActive == false)
                return ServiceResponse<UserDTO>.Error(message: "user account has been frozen");

            user.IsVerified = true;
            user.IsActive = true;
            await _context.SaveChangesAsync(); // 寫入
            return ServiceResponse<UserDTO>.Ok(message: "user verify success, pls login");
        }
    }
}
