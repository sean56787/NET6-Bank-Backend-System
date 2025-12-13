using DotNetSandbox.Data;
using DotNetSandbox.Models.DTOs.Input;
using DotNetSandbox.Models.DTOs.Output;
using DotNetSandbox.Services.CustomResponse;
using DotNetSandbox.Models.Data;
using Microsoft.EntityFrameworkCore;
using DotNetSandbox.Services.Interfaces;

namespace DotNetSandbox.Services
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AdminService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<ServiceResponse<UserDTO>> UpdateUser(UpdateUserRequest req)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == req.UserId || u.Email == req.Email);

            if (user == null)
            {
                return ServiceResponse<UserDTO>.NotFound("user not found");
            }

            if (!string.IsNullOrWhiteSpace(req.Username))
                user.Username = req.Username;

            if (!string.IsNullOrWhiteSpace(req.Email))
                user.Email = req.Email;

            if (!string.IsNullOrWhiteSpace(req.Password))
            {
                var newHashedPassword = BCrypt.Net.BCrypt.HashPassword(req.Password);
                user.Password = newHashedPassword;
            }

            if (req.Role.HasValue)
                user.Role = (User.UserRole)req.Role;

            if (req.IsVerified.HasValue)
                user.IsVerified = (bool)req.IsVerified;

            if (req.IsActive.HasValue)
                user.IsActive = (bool)req.IsActive;

            await _context.SaveChangesAsync();

            var updatedUserDTO = new UserDTO
            {
                Username = user.Username,
                Id = user.UserId,
                Role = user.Role.ToString(),
                Email = user.Email
            };

            return ServiceResponse<UserDTO>.Ok(data: updatedUserDTO);
        }

        public async Task<ServiceResponse<UserDTO>> CreateUser(CreateUserRequest req)
        {
            var result = await _context.Users.AnyAsync(u => u.Username == req.Username || u.Email == req.Email);
            if (result)
                return ServiceResponse<UserDTO>.Error(message: "error: user/email exist", statusCode: 409);

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(req.Password);

            var newUser = new User
            {
                Username = req.Username,
                Password = hashedPassword,
                Email = req.Email,
                IsVerified = false,
                IsActive = true,
                Role = req.Role,
            };

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            var userDTO = new UserDTO
            {
                Id = newUser.UserId,
                Username = newUser.Username,
                Role = newUser.Role.ToString(),
                Email = newUser.Email
            };

            if (userDTO != null)
            {
                return ServiceResponse<UserDTO>.Ok(message: "user created success", data: userDTO);
            }
            else
            {
                return ServiceResponse<UserDTO>.Error(message: "process fail when create userDTO", statusCode: 500);
            }
        }

        public async Task<ServiceResponse<UserDTO>> DeleteUser(DeleteUserRequest req)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == req.UserId);
            if (user == null)
            {
                return ServiceResponse<UserDTO>.NotFound(message: "user not found");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return ServiceResponse<UserDTO>.Ok(message: "user deleted"); ;
        }

        public async Task<ServiceResponse<UserDTO>> GetUser(GetUserRequest req)
        {
            var userDTO = await _context.Users.
                Where(u => u.UserId == req.UserId || u.Email == req.Email).
                Select(u => new UserDTO
                {
                    Id = u.UserId,
                    Username = u.Username,
                    Role = u.Role.ToString(),
                    Email = u.Email
                }).FirstOrDefaultAsync();

            if (userDTO == null)
                return ServiceResponse<UserDTO>.NotFound(message: $"user not found");

            return ServiceResponse<UserDTO>.Ok(data: userDTO);
        }

        public async Task<ServiceResponse<List<UserDTO>>> GetAllUsers()
        {
            var usersDTO = await _context.Users.Select(u => new UserDTO
            {
                Id = u.UserId,
                Username = u.Username,
                Email = u.Email,
                Role = u.Role.ToString(),
                IsVerified = u.IsVerified
            }).ToListAsync();

            if (usersDTO != null)
                return ServiceResponse<List<UserDTO>>.Ok(data: usersDTO);
            else
                return ServiceResponse<List<UserDTO>>.Error(message: "users not found");
        }

        public async Task<ServiceResponse<UserDTO>> FrozenUser(FrozenUserRequest req)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == req.UserId);
            UserDTO? userDTO = null;

            if (user == null)
            {
                return ServiceResponse<UserDTO>.NotFound(message: "no user to frozen, user not found", statusCode: 404);
            }

            if (user.IsActive == false)
            {
                userDTO = new UserDTO { Username = user.Username, IsActive = user.IsActive };
                return ServiceResponse<UserDTO>.Error(userDTO, message: "user is been frozen, no action", statusCode: 409);
            }

            user.IsActive = false;
            await _context.SaveChangesAsync();

            userDTO = new UserDTO
            {
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString(),
                IsActive = user.IsActive
            };

            return ServiceResponse<UserDTO>.Ok(userDTO, message: "frozen user success");
        }
    }
}
