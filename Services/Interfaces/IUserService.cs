using DotNetSandbox.Models.DTOs.Input;
using DotNetSandbox.Models.DTOs.Output;
using DotNetSandbox.Services.CustomResponse;

namespace DotNetSandbox.Services.Interfaces
{
    public interface IUserService
    {
        Task<SystemResponse<UserDTO>> Register(RegisterRequest req);
        Task<SystemResponse<UserDTO>> Login(LoginRequest req);
        Task<SystemResponse<UserDTO>> Verify(string email);
    }
}
