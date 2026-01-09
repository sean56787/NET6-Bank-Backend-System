using DotNetSandbox.Data;
using DotNetSandbox.Models.DTOs.Input;
using DotNetSandbox.Models.DTOs.Output;
using DotNetSandbox.Services.CustomResponse;
using DotNetSandbox.Services.Interfaces;

namespace DotNetSandbox.Services.Utility
{
    public class UserDepositCheck : IUserDepositCheck
    {
        private readonly AppDbContext _context;

        public UserDepositCheck(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SystemResponse<UserBalanceDTO>> IsValidOperation(DepositRequest req)
        {
            if(req.Amount <= 0)
            {
                return SystemResponse<UserBalanceDTO>.Error(message: "amount of deposit can not be negative", statusCode: 403);
            }

            if(req.Amount > 100000)
            {
                return SystemResponse<UserBalanceDTO>.Error(message: "can not deposit over $100,000", statusCode: 403);
            }

            return SystemResponse<UserBalanceDTO>.Ok();
        }
    }
}
