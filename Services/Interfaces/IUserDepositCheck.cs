using DotNetSandbox.Models.DTOs.Input;
using DotNetSandbox.Models.DTOs.Output;
using DotNetSandbox.Services.CustomResponse;

namespace DotNetSandbox.Services.Interfaces
{
    public interface IUserDepositCheck
    {
        Task<ServiceResponse<UserBalanceDTO>> IsValidOperation(DepositRequest req);
    }
}
