using DotNetSandbox.Models.DTOs.Input;
using DotNetSandbox.Models.DTOs.Output;
using DotNetSandbox.Services.CustomResponse;

namespace DotNetSandbox.Services.Interfaces
{
    public interface IBalanceService
    {
        Task<SystemResponse<UserBalanceDTO>> AdjustBalanceAsync(AdjustBalanceRequest req, string operatorName);
        Task<SystemResponse<UserTransactionDTO>> GetTransactions(TransactionsRequest req, string operatorName);
        // Task<ServiceResponse<SoloTransactionRequest>> GetSoloTransactions(SoloTransactionRequest req);
        Task<SystemResponse<UserBalanceDTO>> DepositAsync(DepositRequest req, string operatorName);
        Task<SystemResponse<UserBalanceDTO>> WithdrawAsync(WithdrawRequest req, string operatorName);
        Task<SystemResponse<UserBalanceDTO>> C2CTransferAsync(TransferRequest req, string operatorName);
    }
}
