namespace DotNetSandbox.Models
{
    public class UserStore
    {
        public static List<User> Users = new()
        {
            new User {Id = 1, Username = "admin", Password = "123", IsVerified = true},
            new User {Id = 2, Username = "test", Password = "456",IsVerified = false},
        };

        public static User? FindByUsername(string username) // 找第一筆符合的 找不到 => 回傳 null
        {
            return Users.FirstOrDefault(u => u.Username == username);
        }

        public static void Add(User user)
        {
            user.Id = Users.Count > 0 ? Users.Max(u => u.Id) + 1 : 1;
            Users.Add(user);
        }
    }
}
