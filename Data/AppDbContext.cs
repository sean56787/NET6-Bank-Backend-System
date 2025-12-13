using Microsoft.EntityFrameworkCore;
using DotNetSandbox.Models.Data;

namespace DotNetSandbox.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<BalanceLog> BalanceLogs { get; set; }
        public DbSet<TransferLog> TransferLogs { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) //modelBuilder => Entity Framework Core 自定義資料模型設定
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().Property(u => u.Role).HasConversion<string>(); // 後續所有_context.add() 在遇到 User.Role都會用字串儲存
            modelBuilder.Entity<BalanceLog>().Property(b => b.Type).HasConversion<string>();

            modelBuilder.Entity<BalanceLog>()
                .HasOne(b => b.User)
                .WithMany(u => u.BalanceLogs)
                .HasForeignKey(b => b.UserId) //外鍵
                .HasPrincipalKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade); //父表不存在時連同刪除子表
        }
    }
}
