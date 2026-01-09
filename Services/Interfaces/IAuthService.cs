using DotNetSandbox.Models.DTOs.Output;
using DotNetSandbox.Services.CustomResponse;

namespace DotNetSandbox.Services.Interfaces
{
    public interface IAuthService
    {
        SystemResponse<string> GenerateToken(UserDTO user);
    }
}
