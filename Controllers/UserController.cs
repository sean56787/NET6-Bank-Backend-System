using Microsoft.AspNetCore.Mvc;
using DotNetSandbox.Services;

namespace DotNetSandbox.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AuthService _authService;

        public UserController()
        {
            _authService = new AuthService();
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
            var success = _authService.Login(username, password);
            if (!success) return Conflict(new { error = "invalid account or not verified" });

            return Ok(new { msg = "login success" });
        }

        [HttpPost("verify")]
        public IActionResult Verify(string username)
        {
            var success = _authService.Verify(username);
            if (!success) return Conflict(new { error = "user not found" });

            return Ok(new { msg = "user verified, pls login" });
        }
    }
}
