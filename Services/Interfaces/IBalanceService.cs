using DotNetSandbox.Models.DTOs.Input;
using DotNetSandbox.Models.DTOs.Output;
using DotNetSandbox.Services.CustomResponse;

namespace DotNetSandbox.Services.Interfaces
{
    public interface IBalanceService
    {
        Task<ServiceResponse<UserBalanceDTO>> AdjustBalanceAsync(AdjustBalanceRequest req, string operatorName);
        Task<ServiceResponse<UserTransactionDTO>> GetTransactions(TransactionsRequest req, string operatorName);
        // Task<ServiceResponse<SoloTransactionRequest>> GetSoloTransactions(SoloTransactionRequest req);
        Task<ServiceResponse<UserBalanceDTO>> DepositAsync(DepositRequest req, string operatorName);
        Task<ServiceResponse<UserBalanceDTO>> WithdrawAsync(WithdrawRequest req, string operatorName);
        Task<ServiceResponse<UserBalanceDTO>> TransferAsync(TransferRequest req, string operatorName);
    }
}
