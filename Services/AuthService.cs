using DotNetSandbox.Models;

namespace DotNetSandbox.Services
{
    public class AuthService
    {
        public bool Register(string username, string password)
        {
            if(UserStore.FindByUsername(username) != null)
            {
                return false;
            }

            var newUser = new User
            {
                Username = username,
                Password = password,
                Isverified = false,
            };

            UserStore.Add(newUser);
            return true;
        }

        public bool Login(string username, string password)
        {
            var user = UserStore.FindByUsername(username);
            return user != null && user.Password == password && user.Isverified;
        }

        public bool Verify(string username)
        {
            var user = UserStore.FindByUsername(username);
            if (user == null) return false;
            user.Isverified = true;
            return true;
        }
    }
}
