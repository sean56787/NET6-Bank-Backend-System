using DotNetSandbox.Models.Data;

namespace DotNetSandbox.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetUserByUserId(int UserId);
        Task<User?> GetUserByUserName(string UserName);
    }
}
