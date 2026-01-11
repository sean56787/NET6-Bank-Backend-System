using DotNetSandbox.Models.Data;
using System.Data;
namespace DotNetSandbox.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        IGenericRepository<BalanceLog> BalanceLogs { get; }
        IGenericRepository<UserBalanceTransferLog> TransferLogs { get; }
        IGenericRepository<ServerLog> ServerLogs { get; }
        IGenericRepository<WebLog> WebLogs { get; }

        Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync();
        Task RollBackTransactionAsync();
    }
}
