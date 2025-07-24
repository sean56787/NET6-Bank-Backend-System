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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == req.Username);

            if (user == null)
            {
                return ServiceResponse<UserBalanceDTO>.NotFound("user not found");
            }

            user.Balance += req.Amount;

            _context.BalanceLogs.Add(new Models.Data.BalanceLog
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

        public async Task<ServiceResponse<UserTransactionDTO>> GetTransactions(TransactionsRequest req, string operatorName)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == req.UserId);

            if (user == null)
            {
                return ServiceResponse<UserTransactionDTO>.NotFound("user not found");
            }

            var query = _context.BalanceLogs
                .Include(b => b.User)
                .AsQueryable();

            query = query.Where(b => b.UserId == req.UserId);

            if (!string.IsNullOrWhiteSpace(req.Operator))
                query = query.Where(b => b.Operator.Contains(req.Operator));

            if (req.BalanceType.HasValue)
                query = query.Where(b => b.Type == req.BalanceType.Value);

            if (req.StartDate.HasValue)
                query = query.Where(b => b.CreatedAt >= req.StartDate.Value);

            if (req.EndDate.HasValue)
                query = query.Where(b => b.CreatedAt <= req.EndDate.Value);

            var totalCount = await query.CountAsync();

            var logs = await query
                .OrderByDescending(b => b.CreatedAt)
                .Skip((req.Page - 1) * req.PageSize)
                .Take(req.PageSize)
                .ToListAsync();

            var result = new UserTransactionDTO
            {
                UserId = req.UserId,
                Username = user.Username,
                TotalCount = totalCount,
                Transactions = logs.Select(b => new TransactionDTO
                {
                    Id = b.Id,
                    Amount = b.Amount,
                    Type = b.Type,
                    Description = b.Description,
                    Operator = b.Operator,
                    CreatedAt = b.CreatedAt
                }).ToList()
            };

            return ServiceResponse<UserTransactionDTO>.Ok(data: result);
        }
        
        /*
        public async Task<ServiceResponse<SoloTransactionRequest>> GetSoloTransactions(SoloTransactionRequest req)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == req.UserId);

            if(user == null)
                return ServiceResponse<SoloTransactionRequest>.NotFound();

            var query = _context.BalanceLogs

        }
        */
    }
}
