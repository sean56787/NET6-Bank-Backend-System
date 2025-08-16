using DotNetSandbox.Models.Data;

namespace DotNetSandbox.Models.UnUsed
{
    public class UserStore
    {
        public static List<User> Users = new()
        {
            new User {UserId = 1, Username = "admin", Password = "123", IsVerified = true},
            new User {UserId = 2, Username = "test", Password = "456",IsVerified = false},
        };

        public static User? FindByUsername(string username) // 找第一筆符合的 找不到 => 回傳 null
        {
            return Users.FirstOrDefault(u => u.Username == username);
        }

        public static void Add(User user)
        {
            user.UserId = Users.Count > 0 ? Users.Max(u => u.UserId) + 1 : 1;
            Users.Add(user);
        }
    }
}
