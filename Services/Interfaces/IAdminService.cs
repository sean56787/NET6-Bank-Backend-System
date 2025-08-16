using DotNetSandbox.Models.DTOs.Input;
using DotNetSandbox.Models.DTOs.Output;
using DotNetSandbox.Services.CustomResponse;

namespace DotNetSandbox.Services.Interfaces
{
    public interface IAdminService
    {
        Task<ServiceResponse<UserDTO>> UpdateUser(UpdateUserRequest req);
        Task<ServiceResponse<UserDTO>> CreateUser(CreateUserRequest req);
        Task<ServiceResponse<UserDTO>> DeleteUser(DeleteUserRequest req);
        Task<ServiceResponse<UserDTO>> GetUser(GetUserRequest req);
        Task<ServiceResponse<List<UserDTO>>> GetAllUsers();
        Task<ServiceResponse<UserDTO>> FrozenUser(FrozenUserRequest req);
    }
}
