using Microsoft.EntityFrameworkCore;
using DotNetSandbox.Models;

namespace DotNetSandbox.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

    }
}
