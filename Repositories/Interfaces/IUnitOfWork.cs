using DotNetSandbox.Models.Data;
using System.Data;
namespace DotNetSandbox.Repositories.Interfaces
{
    public interface IUnitOfWork :IDisposable
    {
        IUserRepository Users { get; }
        IGenericRepository<BalanceLog> BalanceLogs { get; }
        IGenericRepository<UserBalanceTransferLog> TransferLogs { get; }
        IGenericRepository<ServerLog> ServerLogs { get; }
        IGenericRepository<WebLog> WebLogs { get; }

        Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted);
        Task CommitTransactionAsync();
        Task RollBackTransactionAsync();

        Task<int> CompleteAsync(); // 統一提交事務
    }
}
