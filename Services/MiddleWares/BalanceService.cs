using DotNetSandbox.Services.Interfaces;
using DotNetSandbox.Data;
using DotNetSandbox.Models.Data;
using DotNetSandbox.Models.DTOs.Output;
using DotNetSandbox.Models.DTOs.Input;
using DotNetSandbox.Services.CustomResponse;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DotNetSandbox.Services.MiddleWares
{
    public class BalanceService : IBalanceService
    {
        private readonly AppDbContext _context;
        private readonly IUserWithdrawCheck _withdrawCheck;
        private readonly IUserDepositCheck _depositCheck;
        private readonly IUserTransferCheck _transferCheck;

        public BalanceService(AppDbContext context, IUserWithdrawCheck withdrawCheck, IUserDepositCheck depositCheck, IUserTransferCheck transferCheck)
        {
            _context = context;
            _withdrawCheck = withdrawCheck;
            _depositCheck = depositCheck;
            _transferCheck = transferCheck;
        }

        public async Task<ServiceResponse<UserBalanceDTO>> TransferAsync(TransferRequest req, string? operatorName)
        {
            var sender = await _context.Users.FirstOrDefaultAsync(u => u.UserId == req.FromUserId);
            var receiver = await _context.Users.FirstOrDefaultAsync(u => u.UserId == req.ToUserId);

            var IsValidTransfer = await _transferCheck.IsValidOperation(req, sender, receiver);
            if (!IsValidTransfer.StatusCode.Equals(200))
                return IsValidTransfer;

            using var transaction = await _context.Database.BeginTransactionAsync(); // 建立交易控制
            try
            {
                decimal senderBalanceBefore = sender.Balance;
                decimal receiverBalanceBefore = receiver.Balance;

                decimal senderBalanceAfter = senderBalanceBefore - req.Amount;
                decimal receiverBalanceAfter = receiverBalanceBefore + req.Amount;

                //加入轉帳紀錄
                var transferLog = new TransferLog
                {
                    FromUserId = sender.UserId,
                    ToUserId = receiver.UserId,
                    Amount = req.Amount,
                    CreatedAt = DateTime.Now,
                    Description = req.Description ?? "",
                };
                _context.TransferLogs.Add(transferLog);
                await _context.SaveChangesAsync();

                //加入轉出方紀錄
                var fromBalanceLog = new BalanceLog
                {
                    UserId = sender.UserId,
                    Amount = -req.Amount,
                    BalanceBefore = senderBalanceBefore,
                    Balance = senderBalanceAfter,
                    Type = Models.Enums.BalanceType.TransferOut,
                    Description = req.Description ?? "",
                    Operator = operatorName ?? sender.Username,
                    CreatedAt = DateTime.Now,
                    TransactionId = transferLog.TransferId,
                };
                _context.BalanceLogs.Add(fromBalanceLog);
                await _context.SaveChangesAsync();

                //加入轉入方紀錄
                var toBalanceLog = new BalanceLog
                {
                    UserId = receiver.UserId,
                    Amount = req.Amount,
                    BalanceBefore = receiverBalanceBefore,
                    Balance = receiverBalanceAfter,
                    Type = Models.Enums.BalanceType.TransferIn,
                    Description = req.Description ?? "",
                    Operator = operatorName ?? sender.Username,
                    CreatedAt = DateTime.Now,
                    TransactionId = transferLog.TransferId,
                };
                _context.BalanceLogs.Add(toBalanceLog);
                await _context.SaveChangesAsync();

                transferLog.FromBalanceLogId = fromBalanceLog.BalanceId;
                transferLog.ToBalanceLogId = toBalanceLog.BalanceId;
                await _context.SaveChangesAsync();

                sender.Balance -= req.Amount;
                receiver.Balance += req.Amount;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();                //成功
                return ServiceResponse<UserBalanceDTO>.Ok();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();              //取消交易
                throw;
            }
        }

        public async Task<ServiceResponse<UserBalanceDTO>> WithdrawAsync(WithdrawRequest req, string? operatorName)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == req.UserId);

            if (user == null)
            {
                return ServiceResponse<UserBalanceDTO>.NotFound("user not found");
            }

            var BalanceBefore = user.Balance;
            var IsValidWithdraw = await _withdrawCheck.IsValidOperation(req);

            if (IsValidWithdraw == null)
            {
                return ServiceResponse<UserBalanceDTO>.Error(message: "server error", statusCode: 500);
            }
            else if (!IsValidWithdraw.StatusCode.Equals(200))
            {
                return IsValidWithdraw;
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                user.Balance -= req.Amount;
                await _context.SaveChangesAsync();

                _context.BalanceLogs.Add(new BalanceLog
                {
                    UserId = user.UserId,
                    Amount = req.Amount,
                    BalanceBefore = BalanceBefore,
                    Balance = user.Balance,
                    Type = Models.Enums.BalanceType.Withdraw,
                    Description = req.Description ?? "",
                    CreatedAt = DateTime.Now,
                    Operator = operatorName ?? user.Username,
                });
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();                //確認提交

                var result = new UserBalanceDTO
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Type = req.Type,
                    BalanceBeforeOperation = BalanceBefore,
                    Balance = user.Balance,
                };

                return ServiceResponse<UserBalanceDTO>.Ok(result, message: "withdraw successful");
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();              //取消交易
                throw;
            }
        }

        public async Task<ServiceResponse<UserBalanceDTO>> DepositAsync(DepositRequest req, string? operatorName)
        {

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == req.UserId);

            if (user == null)
            {
                return ServiceResponse<UserBalanceDTO>.NotFound("user not found");
            }
            var BalanceBefore = user.Balance;
            var IsValidDeposit = await _depositCheck.IsValidOperation(req);

            if (IsValidDeposit == null)
            {
                return ServiceResponse<UserBalanceDTO>.Error(message: "server error", statusCode: 500);
            }
            else if (!IsValidDeposit.StatusCode.Equals(200))
            {
                return IsValidDeposit;
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                user.Balance += req.Amount;
                await _context.SaveChangesAsync();

                _context.BalanceLogs.Add(new BalanceLog
                {
                    UserId = user.UserId,
                    Amount = req.Amount,
                    BalanceBefore = BalanceBefore,
                    Balance = user.Balance,
                    Type = Models.Enums.BalanceType.Deposit,
                    Description = req.Description ?? "",
                    CreatedAt = DateTime.Now,
                    Operator = operatorName ?? user.Username,
                });
                await _context.SaveChangesAsync();              //確認提交

                await transaction.CommitAsync();

                var result = new UserBalanceDTO
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Type = req.Type,
                    BalanceBeforeOperation = BalanceBefore,
                    Balance = user.Balance,
                };

                return ServiceResponse<UserBalanceDTO>.Ok(result, message: "deposit successful");
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();              //取消交易
                throw;
            }
        }

        public async Task<ServiceResponse<UserBalanceDTO>> AdjustBalanceAsync(AdjustBalanceRequest req, string? operatorName)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == req.UserId);

            if (user == null)
            {
                return ServiceResponse<UserBalanceDTO>.NotFound("user not found");
            }

            var BalanceBefore = user.Balance;
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                user.Balance += req.Amount;
                await _context.SaveChangesAsync();

                _context.BalanceLogs.Add(new BalanceLog
                {
                    UserId = user.UserId,
                    Amount = req.Amount,
                    BalanceBefore = BalanceBefore,
                    Balance = user.Balance,
                    Type = req.Type,
                    Description = req.Description ?? "",
                    CreatedAt = DateTime.Now,
                    Operator = operatorName ?? user.Username
                });
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();                //確認提交

                var result = new UserBalanceDTO
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Type = req.Type,
                    BalanceBeforeOperation = BalanceBefore,
                    Balance = user.Balance,
                };
                return ServiceResponse<UserBalanceDTO>.Ok(result, message: "balance adjusted by admin");
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();              //取消交易
                throw;
            }
        }

        public async Task<ServiceResponse<UserTransactionDTO>> GetTransactions(TransactionsRequest req, string? operatorName)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == req.UserId);

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
                    BalanceId = b.BalanceId,
                    Amount = b.Amount,
                    Type = b.Type.ToString(),
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
