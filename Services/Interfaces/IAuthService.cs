using DotNetSandbox.Models.DTOs.Output;
using DotNetSandbox.Services.CustomResponse;

namespace DotNetSandbox.Services.Interfaces
{
    public interface IAuthService
    {
        ServiceResponse<string> GenerateToken(UserDTO user);
    }
}
