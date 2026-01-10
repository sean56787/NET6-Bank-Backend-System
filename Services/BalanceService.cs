using DotNetSandbox.Services.Interfaces;
using DotNetSandbox.Data;
using DotNetSandbox.Models.Data;
using DotNetSandbox.Models.DTOs.Output;
using DotNetSandbox.Models.DTOs.Input;
using DotNetSandbox.Services.CustomResponse;
using Microsoft.EntityFrameworkCore;
using DotNetSandbox.Repositories.Interfaces;

namespace DotNetSandbox.Services
{
    public class BalanceService : IBalanceService
    {
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _uow;
        private readonly IUserWithdrawCheck _withdrawCheck;
        private readonly IUserDepositCheck _depositCheck;
        private readonly IUserTransferCheck _transferCheck;
        private readonly IBalanceRepository _balanceRepositories;//repo

        public BalanceService(AppDbContext context, 
                    IUnitOfWork uow,
                    IUserWithdrawCheck withdrawCheck,
                    IUserDepositCheck depositCheck,
                    IUserTransferCheck transferCheck,
                    IBalanceRepository balanceRepositories)
        {
            _context = context;
            _withdrawCheck = withdrawCheck;
            _depositCheck = depositCheck;
            _transferCheck = transferCheck;
            _balanceRepositories = balanceRepositories;
            _uow = uow;
        }

        public async Task<SystemResponse<UserBalanceDTO>> C2CTransferAsync(TransferRequest req, string? operatorName)
        {
            var sender = await _uow.Users.GetUserByUserId(req.FromUserId);
            var receiver = await _uow.Users.GetUserByUserId(req.ToUserId);

            var IsValidTransfer = await _transferCheck.IsValidOperation(req, sender, receiver);
            if (!IsValidTransfer.StatusCode.Equals(200))
                return IsValidTransfer;

            await _uow.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);// 建立交易控制
            try
            {
                decimal senderBalanceBefore = sender.Balance;
                decimal receiverBalanceBefore = receiver.Balance;

                decimal senderBalanceAfter = senderBalanceBefore - req.Amount;
                decimal receiverBalanceAfter = receiverBalanceBefore + req.Amount;

                //加入轉帳紀錄
                var transferLog = new UserBalanceTransferLog
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

                await _context.Database.ExecuteSqlRawAsync(
                        $"UPDATE [Users] SET Balance = {sender.Balance - req.Amount} WHERE UserId = {sender.UserId}"
                    );
                await _context.Database.ExecuteSqlRawAsync(
                        $"UPDATE [Users] SET Balance = {receiver.Balance + req.Amount} WHERE UserId = {receiver.UserId}"
                    );
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();                //成功
                return SystemResponse<UserBalanceDTO>.Ok();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();              //取消交易
                throw;
            }
        }

        public async Task<SystemResponse<UserBalanceDTO>> WithdrawAsync(WithdrawRequest req, string? operatorName)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == req.UserId);

            if (user == null)
            {
                return SystemResponse<UserBalanceDTO>.NotFound("user not found");
            }

            var BalanceBefore = user.Balance;
            var IsValidWithdraw = await _withdrawCheck.IsValidOperation(req);

            if (IsValidWithdraw == null)
            {
                return SystemResponse<UserBalanceDTO>.Error(message: "server error", statusCode: 500);
            }
            else if (!IsValidWithdraw.StatusCode.Equals(200))
            {
                return IsValidWithdraw;
            }

            using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable); // 序列化 > 阻擋其他交易的寫入 > 一致性

            try
            {
                decimal UserUpdatedBalance = user.Balance - req.Amount;

                await _context.Database
                    .ExecuteSqlRawAsync(
                         $"UPDATE Users SET Balance = {UserUpdatedBalance} WHERE UserId = {user.UserId}"
                    );
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

                return SystemResponse<UserBalanceDTO>.Ok(result, message: "withdraw successful");
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();              //取消交易
                throw;
            }
        }

        public async Task<SystemResponse<UserBalanceDTO>> DepositAsync(DepositRequest req, string? operatorName)
        {

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == req.UserId);

            if (user == null)
            {
                return SystemResponse<UserBalanceDTO>.NotFound("user not found");
            }
            var BalanceBefore = user.Balance;
            var IsValidDeposit = await _depositCheck.IsValidOperation(req);

            if (IsValidDeposit == null)
            {
                return SystemResponse<UserBalanceDTO>.Error(message: "server error", statusCode: 500);
            }
            else if (!IsValidDeposit.StatusCode.Equals(200))
            {
                return IsValidDeposit;
            }

            using var transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);

            try
            {
                decimal UserUpdatedBalance = user.Balance + req.Amount;
                await _context.Database
                    .ExecuteSqlRawAsync(
                        $"UPDATE Users SET Balance = {UserUpdatedBalance} WHERE UserId = {user.UserId}"
                    );
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

                return SystemResponse<UserBalanceDTO>.Ok(result, message: "deposit successful");
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();              //取消交易
                throw;
            }
        }

        public async Task<SystemResponse<UserBalanceDTO>> AdjustBalanceAsync(AdjustBalanceRequest req, string? operatorName)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == req.UserId);

            if (user == null)
            {
                return SystemResponse<UserBalanceDTO>.NotFound("user not found");
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
                return SystemResponse<UserBalanceDTO>.Ok(result, message: "balance adjusted by admin");
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();              //取消交易
                throw;
            }
        }

        public async Task<SystemResponse<UserTransactionDTO>> GetTransactions(TransactionsRequest req, string? operatorName)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == req.UserId);

            if (user == null)
            {
                return SystemResponse<UserTransactionDTO>.NotFound("user not found");
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

            return SystemResponse<UserTransactionDTO>.Ok(data: result);
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
