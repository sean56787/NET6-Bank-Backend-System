using DotNetSandbox.Models.DTOs.Input;
using DotNetSandbox.Models.DTOs.Output;
using DotNetSandbox.Services.CustomResponse;

namespace DotNetSandbox.Services.Interfaces
{
    public interface IAdminService
    {
        Task<SystemResponse<UserDTO>> UpdateUser(UpdateUserRequest req);
        Task<SystemResponse<UserDTO>> CreateUser(CreateUserRequest req);
        Task<SystemResponse<UserDTO>> DeleteUser(DeleteUserRequest req);
        Task<SystemResponse<UserDTO>> GetUser(GetUserRequest req);
        Task<SystemResponse<List<UserDTO>>> GetAllUsers();
        Task<SystemResponse<UserDTO>> FrozenUser(FrozenUserRequest req);
    }
}
