using DotNetSandbox.Data;
using DotNetSandbox.Models.Data;
using DotNetSandbox.Models.DTOs.Input;
using DotNetSandbox.Models.DTOs.Output;
using DotNetSandbox.Services.CustomResponse;
using DotNetSandbox.Services.Interfaces;

namespace DotNetSandbox.Services.Utility
{
    public class UserTransferCheck : IUserTransferCheck
    {
        private readonly AppDbContext _context;

        public UserTransferCheck(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<UserBalanceDTO>> IsValidOperation(TransferRequest req, User sender, User receiver)
        {
            if (sender == null) return ServiceResponse<UserBalanceDTO>.NotFound(message: "sender not found", statusCode: 404);
            if (receiver == null) return ServiceResponse<UserBalanceDTO>.NotFound(message: "receiver not found", statusCode: 404);

            if (string.Equals(req.FromUserId, req.ToUserId))
                return ServiceResponse<UserBalanceDTO>.Error(message: "you can not transfer to yourself", statusCode: 400);

            if(req.Amount <= 0)
                return ServiceResponse<UserBalanceDTO>.Error(message: "the amount of transfer can not equal/below 0$", statusCode: 400);

            if (sender.Balance < req.Amount) 
                return ServiceResponse<UserBalanceDTO>.Error(message: "your balance is not sufficient to transfer, pls deposit first", statusCode: 402);

            return ServiceResponse<UserBalanceDTO>.Ok();
        }
    }
}
