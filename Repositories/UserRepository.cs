using DotNetSandbox.Data;
using DotNetSandbox.Models.Data;
using DotNetSandbox.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DotNetSandbox.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<User?> GetUserByUserId(int userId)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User?> GetUserByUserName(string userName)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Username == userName);
        }
    }
}
