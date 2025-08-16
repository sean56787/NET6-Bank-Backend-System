using DotNetSandbox.Models.DTOs.Input;
using DotNetSandbox.Models.DTOs.Output;
using DotNetSandbox.Services.CustomResponse;

namespace DotNetSandbox.Services.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResponse<UserDTO>> Register(RegisterRequest req);
        Task<ServiceResponse<UserDTO>> Login(LoginRequest req);
        Task<ServiceResponse<UserDTO>> Verify(string email);
    }
}
