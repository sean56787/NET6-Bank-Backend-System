using DotNetSandbox.Data;
using DotNetSandbox.Models.DTOs.Input;
using DotNetSandbox.Models.DTOs.Output;
using DotNetSandbox.Services.CustomResponse;
using DotNetSandbox.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DotNetSandbox.Services.Utility
{
    public class UserWithdrawCheck : IUserWithdrawCheck
    {

        private readonly AppDbContext _context;

        public UserWithdrawCheck(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SystemResponse<UserBalanceDTO>> IsValidOperation(WithdrawRequest req)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == req.UserId);

            if (req.Amount <= 0)
            {
                return SystemResponse<UserBalanceDTO>.Error(message: "amount of Withdraw can not be negative", statusCode: 403);
            }

            if (req.Amount > 50000)
            {
                return SystemResponse<UserBalanceDTO>.Error(message: "can not Withdraw over $50,000", statusCode: 403);
            }

            return SystemResponse<UserBalanceDTO>.Ok();
        }
    }
}
