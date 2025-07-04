using DotNetSandbox.Models.DTOs.Input;
using DotNetSandbox.Models.DTOs.Output;
using DotNetSandbox.Services.CustomResponse;

namespace DotNetSandbox.Services.Interfaces
{
    public interface IBalanceService
    {
        Task<ServiceResponse<UserBalanceDTO>> AdjustBalanceAsync(AdjustBalanceRequest req, string operatorName);
    }
}
