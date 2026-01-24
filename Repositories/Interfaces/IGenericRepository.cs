using System.Linq.Expressions;

namespace DotNetSandbox.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(object id);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        void Update(T entity);
        void Add(T entity);
    }
}
