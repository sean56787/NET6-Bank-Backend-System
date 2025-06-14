//using System.Collections.Generic;
//using System.Linq;
namespace DotNetSandbox.Models
{
    public class UserStore
    {
        public static List<User> Users = new()
        {
            new User {Id = 1, Username = "admin", Password = "123", Isverified = true},
            new User {Id = 2, Username = "test", Password = "456",Isverified = false},
        };

        public static User? FindByUsername(string username)
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
