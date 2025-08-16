using DotNetSandbox.Models.Data;
using DotNetSandbox.Models.DTOs.Input;
using DotNetSandbox.Models.DTOs.Output;
using DotNetSandbox.Services.CustomResponse;

namespace DotNetSandbox.Services.Interfaces
{
    public interface IUserTransferCheck
    {
        Task<ServiceResponse<UserBalanceDTO>> IsValidOperation(TransferRequest req, User sender, User receiver);
    }
}
