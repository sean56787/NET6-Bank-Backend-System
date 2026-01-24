using DotNetSandbox.Data;
using DotNetSandbox.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DotNetSandbox.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context; //只允許自己和子類別修改
        protected readonly DbSet<T> _dbSet; //from EntityFrameworkCore

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>(); //EF Core 根據傳入的 T，動態從 DbContext 找到對應的 DbSet<T>
        }

        public async Task<T?> GetByIdAsync(object id) //找主鍵
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<bool> AnyAsync(Expression<Func<T,bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
        
        
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }
    }
}
