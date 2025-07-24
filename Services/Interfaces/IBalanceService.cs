using DotNetSandbox.Models.DTOs.Input;
using DotNetSandbox.Models.DTOs.Output;
using DotNetSandbox.Services.CustomResponse;
using Microsoft.AspNetCore.Mvc;

namespace DotNetSandbox.Services.Interfaces
{
    public interface IBalanceService
    {
        Task<ServiceResponse<UserBalanceDTO>> AdjustBalanceAsync(AdjustBalanceRequest req, string operatorName);
        Task<ServiceResponse<UserTransactionDTO>> GetTransactions(TransactionsRequest req, string operatorName);
        Task<ServiceResponse<SoloTransactionRequest>> GetSoloTransactions(SoloTransactionRequest req);
    }
}
