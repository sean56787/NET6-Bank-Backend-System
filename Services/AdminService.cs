using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using DotNetSandbox.Data;
using DotNetSandbox.Models.DTOs.Input;
using DotNetSandbox.Models.DTOs.Output;
using DotNetSandbox.Services.CustomResponse;
using DotNetSandbox.Models.Data;

namespace DotNetSandbox.Services
{
    public class AdminService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AdminService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public UserDTO? UpdateUser(UpdateUserRequest req)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == req.Username || u.Email == req.Email);

            if (user == null)
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

            if (req.IsVerified != null)
                user.IsVerified = (bool)req.IsVerified;

            if (req.IsActive != null)
                user.IsActive = (bool)req.IsActive;

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
        public ServiceResponse<UserDTO> CreateUser(string? username, string? password, string? email, User.UserRole? role) // 錯誤 這裡沒回應 controller -> badreq
        {
            var result = _context.Users.Any(u => u.Username == username || u.Email == email);
            if (result)
                return ServiceResponse<UserDTO>.Error(message:"error: user/email exist", statusCode: 409);

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var newUser = new User
            {
                Username = username,
                Password = hashedPassword,
                Email = email,
                IsVerified = false,
                IsActive = false,
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
            
            if(userDTO != null)
            {
                return ServiceResponse<UserDTO>.Ok(message: "user created success", data:userDTO);
            }
            else
            {
                return ServiceResponse<UserDTO>.Error(message: "process fail when create userDTO", statusCode: 500);
            }
            
        }

        public ServiceResponse<User> DeleteUser(string? username)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return ServiceResponse<User>.NotFound(message:"user not found");
            }
            
            _context.Users.Remove(user);
            _context.SaveChanges();
            return ServiceResponse<User>.Ok(user, message: "user not found"); ;
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
                IsVerified = u.IsVerified
            }).ToList();

            return usersDTO;
        }

        public ServiceResponse<UserDTO> FrozenUser(string? username)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            UserDTO? userDTO = null;

            if (user == null)
            {
                return ServiceResponse<UserDTO>.NotFound(message: "no user to frozen, user not found", statusCode: 404);
            }

            if(user.IsActive == false)
            {
                userDTO = new UserDTO { Username = user.Username, IsActive = user.IsActive };
                return ServiceResponse<UserDTO>.Error(userDTO, message: "user is been frozen, no action", statusCode: 409);
            }

            user.IsActive = false;
            _context.SaveChanges();
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
