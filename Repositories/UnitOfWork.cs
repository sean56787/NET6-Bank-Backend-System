using DotNetSandbox.Data;
using DotNetSandbox.Models.Data;
using DotNetSandbox.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace DotNetSandbox.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _currentTransaction;
        public IUserRepository Users { get; private set; }
        public IGenericRepository<BalanceLog> BalanceLogs { get; private set; }
        public IGenericRepository<UserBalanceTransferLog> TransferLogs { get; private set; }
        public IGenericRepository<ServerLog> ServerLogs { get; private set; }
        public IGenericRepository<WebLog> WebLogs { get; private set; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;

            // 初始化各個倉儲，共用同一個 context
            Users = new UserRepository(_context);
            BalanceLogs = new GenericRepository<BalanceLog>(_context);
            TransferLogs = new GenericRepository<UserBalanceTransferLog>(_context);
            ServerLogs = new GenericRepository<ServerLog>(_context);
            WebLogs = new GenericRepository<WebLog>(_context);
        }

        public async Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
        {
            if (_currentTransaction != null) return;
            _currentTransaction = await _context.Database.BeginTransactionAsync(isolationLevel);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_currentTransaction != null)
                {
                    await _currentTransaction.CommitAsync();
                }
            }
            catch
            {
                await _currentTransaction.RollbackAsync();
            }
            finally
            {
                await DisposeTransactionAsync();
            }
        }

        public async Task RollBackTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync();
                await DisposeTransactionAsync();
            }
        }

        private async Task DisposeTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }
}
