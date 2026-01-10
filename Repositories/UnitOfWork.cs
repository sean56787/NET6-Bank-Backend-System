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
            _currentTransaction = await _context.Database.BeginTransactionAsync(isolationLevel);
        }

        public Task CommitTransactionAsync()
        {
            throw new NotImplementedException();
        }

        public Task RollBackTransactionAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
