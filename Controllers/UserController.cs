using Microsoft.AspNetCore.Mvc;     // ControllerBase
using DotNetSandbox.Services;       // register / login / verify
using Microsoft.AspNetCore.Authorization;
using DotNetSandbox.Data;
using DotNetSandbox.Models;
namespace DotNetSandbox.Controllers
{
    [ApiController]                 // Web API Controller
    [Route("api/[controller]")]     // api/user
    public class UserController : ControllerBase
    {
        
        private readonly AuthService _authService;

        public UserController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public IActionResult Register(string username, string password)
        {
            var success = _authService.Register(username, password);
            if (!success) return Conflict(new { error = "user already exists" });
            return Ok(new { msg = "user registered" });
        }

        [HttpPost("login")]
        public IActionResult Login(string username, string password)
        {
            var user = _authService.Login(username, password);       //檢查是否為已驗證帳戶
            if (user == null || !user.Isverified) return Conflict(new { error = "invalid account or not verified" });

            var token = _authService.GenerateToken(user);
            return Ok(new {token});
        }

        [HttpPost("verify")]
        public IActionResult Verify(string username)
        {
            var success = _authService.Verify(username);
            if (!success) return Conflict(new { error = "user not found" });

            return Ok(new { msg = "user verified, pls login" });
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _authService.GetAllUsers();
            return Ok(users);
        }
    }
}
