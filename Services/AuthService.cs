using DotNetSandbox.Data;
using DotNetSandbox.Models;

namespace DotNetSandbox.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public bool Register(string username, string password)
        {
            if(_context.Users.Any(u => u.Username == username))
            {
                return false;
            }

            var newUser = new User
            {
                Username = username,
                Password = password,
                Isverified = false,
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();
            return true;
        }

        public bool Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            return user != null && user.Password == password && user.Isverified;
        }

        public bool Verify(string username)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return false;
            user.Isverified = true;
            _context.SaveChanges(); // 寫入
            return true;
        }
    }
}
