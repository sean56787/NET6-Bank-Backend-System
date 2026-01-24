namespace DotNetSandbox.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(object id);
        void Update(T entity);
        void Add(T entity);
    }
}
