using Microsoft.AspNetCore.Mvc;     // ControllerBase
using DotNetSandbox.Services;       // register / login / verify
using Microsoft.AspNetCore.Authorization;
using DotNetSandbox.Data;
using DotNetSandbox.Models.DTOs;

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
        public IActionResult Register([FromBody] RegisterRequest req)
        {
            try
            {
                var success = _authService.Register(req.Username, req.Password, req.Email);
                if (!success) return Conflict(new { error = "user already exists" });
                return Ok(new { msg = "user registered" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            try
            {
                var user = _authService.Login(req.Username, req.Password, req.Email);       //檢查是否為已驗證帳戶
                if (user == null || !user.Isverified) return Conflict(new { error = "invalid account or not verified" });

                var token = _authService.GenerateToken(user);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("verify")]
        public IActionResult Verify(string username)
        {
            try
            {
                var success = _authService.Verify(username);
                if (!success) return Conflict(new { error = "user not found" });
                return Ok(new { msg = "user verified, pls login" });
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            try
            {
                var usersDTO = _authService.GetAllUsers();
                if (usersDTO != null)
                    return Ok(usersDTO);
                else return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
