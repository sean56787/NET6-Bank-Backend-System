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
        private readonly IWebLogService _webLogService;

        public UserService(AppDbContext context, IConfiguration config, IWebLogService webLogService)
        {
            _context = context;
            _config = config;
            _webLogService = webLogService;
        }

        public async Task<SystemResponse<UserDTO>> Register(RegisterRequest req)
        {

            bool result = await _context.Users.AnyAsync(u => u.Username == req.Username || u.Email == req.Email);
            if (result)
            {
                await _webLogService.WebLogWarnings(
                    "user try to register using email, but it was registed by another account",
                    DateTime.UtcNow,
                    Models.Enums.SecurityLevelType.WARN,
                    StatusCodes.Status409Conflict
                );

                return SystemResponse<UserDTO>.Error(message: "username or email already exists", statusCode: 409);
            }
                

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
            return SystemResponse<UserDTO>.Ok(message: "user register success");
        }

        public async Task<SystemResponse<UserDTO>> Login(LoginRequest req)
        {
            User? user = null;

            if (req.UserId.HasValue)
                user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == req.UserId);
            else if (!string.IsNullOrWhiteSpace(req.Email))
                user = await _context.Users.FirstOrDefaultAsync(u => u.Email == req.Email);

            if (user == null)
            {
                await _webLogService.WebLogWarnings(
                    "user not found",
                    DateTime.UtcNow,
                    Models.Enums.SecurityLevelType.WARN,
                    StatusCodes.Status404NotFound
                );
                return SystemResponse<UserDTO>.NotFound(message: "user not found", statusCode: 404);
            }
            else if (user.IsVerified == false || user.IsActive == false)
            {
                await _webLogService.WebLogWarnings(
                    "user not verified or has been frozen",
                    DateTime.UtcNow,
                    Models.Enums.SecurityLevelType.WARN,
                    StatusCodes.Status403Forbidden
                );
                return SystemResponse<UserDTO>.Error(message: "user not verified or has been frozen", statusCode: 403);
            }
            else if (!BCrypt.Net.BCrypt.Verify(req.Password, user.Password))
            {
                await _webLogService.WebLogWarnings(
                    "account & password doesn't match",
                    DateTime.UtcNow,
                    Models.Enums.SecurityLevelType.WARN,
                    StatusCodes.Status422UnprocessableEntity
                );
                return SystemResponse<UserDTO>.Error(message: "account & password doesn't match", statusCode: 422);
            }

            var userDTO = new UserDTO
            {
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString(),
                IsActive = user.IsActive
            };

            return SystemResponse<UserDTO>.Ok(data: userDTO, message: "login success");
        }

        public async Task<SystemResponse<UserDTO>> Verify(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) 
            {
                await _webLogService.WebLogWarnings(
                    "user not found",
                    DateTime.UtcNow,
                    Models.Enums.SecurityLevelType.WARN,
                    StatusCodes.Status404NotFound
                );
                return SystemResponse<UserDTO>.NotFound(message: "user not found");
            }
                
            if (user.IsActive == false)
            {
                await _webLogService.WebLogWarnings(
                    "user account has been frozen",
                    DateTime.UtcNow,
                    Models.Enums.SecurityLevelType.WARN,
                    StatusCodes.Status403Forbidden
                );
                return SystemResponse<UserDTO>.Error(message: "user account has been frozen", statusCode:403);
            }
                

            user.IsVerified = true;
            user.IsActive = true;
            await _context.SaveChangesAsync(); // 寫入
            return SystemResponse<UserDTO>.Ok(message: "user verify success, pls login");
        }
    }
}
