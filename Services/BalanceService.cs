using DotNetSandbox.Services.Interfaces;
using DotNetSandbox.Data;
using DotNetSandbox.Models.DTOs.Output;
using DotNetSandbox.Models.DTOs.Input;
using DotNetSandbox.Services.CustomResponse;
using Microsoft.EntityFrameworkCore;

namespace DotNetSandbox.Services
{
    public class BalanceService : IBalanceService
    {
        private readonly AppDbContext _context;

        public BalanceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<UserBalanceDTO>> AdjustBalanceAsync(AdjustBalanceRequest req, string operatorName)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u=>u.Username == req.Username);

            if(user == null)
            {
                return ServiceResponse<UserBalanceDTO>.NotFound("user not found");
            }

            user.Balance += req.Amount;

            _context.BalanceLogs.Add(new Models.BalanceLog
            {
                UserId = user.Id,                           // TODO: 換成User.UserId
                Amount = req.Amount,
                Type = req.Type,
                Description = req.Note ?? "",
                CreatedAt = DateTime.UtcNow,
                Operator = operatorName
            });

            await _context.SaveChangesAsync();

            var result = new UserBalanceDTO
            {
                Username = user.Username,
                Balance = user.Balance
            };

            return ServiceResponse<UserBalanceDTO>.Ok(result, message: "balance adjusted success");
        }
    }
}
