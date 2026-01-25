using DotNetSandbox.Services.Interfaces;
using DotNetSandbox.Data;
using DotNetSandbox.Models.Data;
using DotNetSandbox.Models.DTOs.Output;
using DotNetSandbox.Models.DTOs.Input;
using DotNetSandbox.Services.CustomResponse;
using Microsoft.EntityFrameworkCore;
using DotNetSandbox.Repositories.Interfaces;
using EntityFramework.Exceptions.Common;

namespace DotNetSandbox.Services
{
    public class BalanceService : IBalanceService
    {
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _uow;
        private readonly IUserWithdrawCheck _withdrawCheck;
        private readonly IUserDepositCheck _depositCheck;
        private readonly IUserTransferCheck _transferCheck;
        private readonly IServiceCommandCenter _serviceCommandCenter;

        public BalanceService(AppDbContext context, 
                    IUnitOfWork uow,
                    IUserWithdrawCheck withdrawCheck,
                    IUserDepositCheck depositCheck,
                    IUserTransferCheck transferCheck,
                    IBalanceRepository balanceRepositories,
                    IServiceCommandCenter serviceCommandCenter)
        {
            _context = context;
            _withdrawCheck = withdrawCheck;
            _depositCheck = depositCheck;
            _transferCheck = transferCheck;
            _uow = uow;
            _serviceCommandCenter = serviceCommandCenter;
        }

        public async Task<SystemResponse> C2CTransferAsync(TransferRequest req, string? operatorName)
        {
            var RequestKeyCheck = await _serviceCommandCenter.RequestKeyCheck(req.RequestKey);
            if (!RequestKeyCheck.Success)
            {
                return SystemResponse.Error(message: RequestKeyCheck.Message ?? "", statusCode: RequestKeyCheck.StatusCode);
            }

            await _uow.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);      // 建立交易控制-TODO: 將IsolationLevel改更寬鬆並配合rowVersion+TimeStamp
            try
            {
                var userRepo = _uow.Users;

                var firstId = req.FromUserId < req.ToUserId ? req.FromUserId : req.ToUserId;
                var secondId = req.FromUserId < req.ToUserId ? req.ToUserId : req.FromUserId;

                var firstUser = await _uow.Users.GetUserByUserId(firstId);
                var secondUser = await _uow.Users.GetUserByUserId(secondId);

                var sender = firstId == req.FromUserId ? firstUser : secondUser;
                var receiver = firstId == req.ToUserId ? firstUser : secondUser;

                var ValidTransferCheck = await _transferCheck.IsValidOperation(req, sender, receiver);
                if (!ValidTransferCheck.StatusCode.Equals(200))
                {
                    await _uow.RollBackTransactionAsync();
                    return ValidTransferCheck;
                }

                decimal senderBalanceBefore = sender.Balance;
                decimal receiverBalanceBefore = receiver.Balance;

                sender.Balance -= req.Amount;
                receiver.Balance += req.Amount;

                //加入轉帳紀錄
                var transferLog = new UserBalanceTransferLog
                {
                    FromUserId = sender.UserId,
                    ToUserId = receiver.UserId,
                    Amount = req.Amount,
                    CreatedAt = DateTime.Now,
                    Description = req.Description ?? "",
                    RequestKey = req.RequestKey,
                };
                _uow.TransferLogs.Add(transferLog);

                //加入轉出方紀錄
                var fromBalanceLog = new BalanceLog
                {
                    UserId = sender.UserId,
                    Amount = -req.Amount,
                    BalanceBefore = senderBalanceBefore,
                    Balance = sender.Balance,
                    Type = Models.Enums.BalanceType.TransferOut,
                    Description = req.Description ?? "",
                    Operator = operatorName ?? sender.Username,
                    CreatedAt = DateTime.Now,
                    TransactionId = transferLog.TransferId,
                };
                _uow.BalanceLogs.Add(fromBalanceLog);

                //加入轉入方紀錄
                var toBalanceLog = new BalanceLog
                {
                    UserId = receiver.UserId,
                    Amount = req.Amount,
                    BalanceBefore = receiverBalanceBefore,
                    Balance = receiver.Balance,
                    Type = Models.Enums.BalanceType.TransferIn,
                    Description = req.Description ?? "",
                    Operator = operatorName ?? sender.Username,
                    CreatedAt = DateTime.Now,
                    TransactionId = transferLog.TransferId,
                };
                _uow.BalanceLogs.Add(toBalanceLog);

                //轉帳紀錄ID重指定
                transferLog.FromBalanceLogId = fromBalanceLog.BalanceId;
                transferLog.ToBalanceLogId = toBalanceLog.BalanceId;
                _uow.TransferLogs.Update(transferLog);

                //更新用戶帳戶-停用，SaveChangesAsync() 會自動追蹤實體變更
                //sender.Balance = sender.Balance;
                //receiver.Balance = receiver.Balance;
                //_uow.Users.Update(sender);
                //_uow.Users.Update(receiver);

                await _uow.SaveChangesAsync();                      
                await _uow.CommitTransactionAsync();                //實際DB操作
                return SystemResponse.Ok();
            }
            catch(DbUpdateConcurrencyException cccEx)
            {
                await _uow.RollBackTransactionAsync();
                return SystemResponse.Error(message: "system busy, try later", statusCode: 500);
            }
            catch(UniqueConstraintException ucEx)
            {
                await _uow.RollBackTransactionAsync();
                return SystemResponse.Error(message: "請求處理中或已完成", statusCode: 500);
            }
            catch(Exception ex)
            {
                await _uow.RollBackTransactionAsync();             //取消交易
                throw;
            }
        }

        public async Task<SystemResponse> WithdrawAsync(WithdrawRequest req, string? operatorName)
        {
            var RequestKeyCheck = await _serviceCommandCenter.RequestKeyCheck(req.RequestKey);
            if (!RequestKeyCheck.Success)
            {
                return SystemResponse.Error(message: RequestKeyCheck.Message ?? "", statusCode: RequestKeyCheck.StatusCode);
            }

            await _uow.BeginTransactionAsync(System.Data.IsolationLevel.Serializable); // 序列化 > 阻擋其他交易的寫入 > 一致性 > 鎖住整個資料範圍(最嚴格)

            try
            {
                //找User
                var user = await _uow.Users.GetByIdAsync(req.UserId);
                if (user == null)
                {
                    return SystemResponse.NotFound("user not found");
                }

                //withdraw check
                var ValidDrawCheck = await _withdrawCheck.IsValidOperation(req);
                if (ValidDrawCheck == null)
                {
                    return SystemResponse.Error(message: "server error", statusCode: 500);
                }
                else if (!ValidDrawCheck.StatusCode.Equals(200))
                {
                    return ValidDrawCheck;
                }

                //紀錄動作前餘額
                var BalanceBefore = user.Balance;
                //提款-更改餘額
                user.Balance -= req.Amount;
                await _uow.SaveChangesAsync();

                _uow.BalanceLogs.Add(new BalanceLog
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
                
                await _uow.SaveChangesAsync();
                await _uow.CommitTransactionAsync();                //確認提交
                
                return SystemResponse.Ok(message: "withdraw successful");
            }
            catch (DbUpdateConcurrencyException cccEx)
            {
                await _uow.RollBackTransactionAsync();
                return SystemResponse.Error(message: "system busy, try later", statusCode: 500);
            }
            catch (UniqueConstraintException ucEx)
            {
                await _uow.RollBackTransactionAsync();
                return SystemResponse.Error(message: "請求處理中或已完成", statusCode: 500);
            }
            catch (Exception ex)
            {
                await _uow.RollBackTransactionAsync();             //取消交易
                throw;
            }
        }

        public async Task<SystemResponse> DepositAsync(DepositRequest req, string? operatorName)
        {

            var RequestKeyCheck = await _serviceCommandCenter.RequestKeyCheck(req.RequestKey);
            if (!RequestKeyCheck.Success)
            {
                return SystemResponse.Error(message: RequestKeyCheck.Message ?? "", statusCode: RequestKeyCheck.StatusCode);
            }

            await _uow.BeginTransactionAsync(System.Data.IsolationLevel.Serializable); // 序列化 > 阻擋其他交易的寫入 > 一致性 > 鎖住整個資料範圍(最嚴格)

            try
            {
                //找User
                var user = await _uow.Users.GetByIdAsync(req.UserId);
                if (user == null)
                {
                    return SystemResponse.NotFound("user not found");
                }

                //deposit check
                var ValidDepositCheck = await _depositCheck.IsValidOperation(req);
                if (ValidDepositCheck == null)
                {
                    return SystemResponse.Error(message: "server error", statusCode: 500);
                }
                else if (!ValidDepositCheck.StatusCode.Equals(200))
                {
                    return ValidDepositCheck;
                }

                //紀錄動作前餘額
                var BalanceBefore = user.Balance;
                //存款-更改餘額
                user.Balance += req.Amount;
                await _uow.SaveChangesAsync();

                _uow.BalanceLogs.Add(new BalanceLog
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

                await _uow.SaveChangesAsync();
                await _uow.CommitTransactionAsync();                //確認提交

                return SystemResponse.Ok(message: "deposit successful");
            }
            catch (DbUpdateConcurrencyException cccEx)
            {
                await _uow.RollBackTransactionAsync();
                return SystemResponse.Error(message: "system busy, try later", statusCode: 500);
            }
            catch (UniqueConstraintException ucEx)
            {
                await _uow.RollBackTransactionAsync();
                return SystemResponse.Error(message: "請求處理中或已完成", statusCode: 500);
            }
            catch (Exception ex)
            {
                await _uow.RollBackTransactionAsync();             //取消交易
                throw;
            }
        }

        /// <summary>
        /// 後台調整用戶餘額-暫時停用
        /// </summary>
        /// <param name="req"></param>
        /// <param name="operatorName"></param>
        /// <returns></returns>
        /*
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
        */

        public async Task<SystemResponse<UserTransactionDTO>> GetTransactions(TransactionsRequest req, string? operatorName)
        {
            var user = await _uow.Users.GetByIdAsync(req.UserId);

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
