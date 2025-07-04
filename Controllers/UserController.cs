using Microsoft.AspNetCore.Mvc;     // ControllerBase
using Microsoft.AspNetCore.Authorization;
using DotNetSandbox.Services;       // register / login / verify
using System.Security.Claims;
using DotNetSandbox.Models.DTOs.Input;

namespace DotNetSandbox.Controllers
{
    [ApiController]                 // 自動處理 ModelState 錯誤驗證
    [Route("api/[controller]")]     // api/user
    public class UserController : ControllerBase
    {
        
        private readonly UserService _userService;
        private readonly AuthService _authService;
        public UserController(UserService userService, AuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest req)
        {
            try
            {
                var success = _userService.Register(req.Username, req.Password, req.Email);
                if (!success) return Conflict(new { error = "user || email already exists" });
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
                var result = _userService.Login(req.Username, req.Password, req.Email);       //檢查是否為已驗證帳戶

                if (!result.Success)
                {
                    return StatusCode(result.StatusCode, result.Message);
                }

                if (result?.Data != null)
                {
                    var token = _authService.GenerateToken(result.Data);
                    return Ok(new { token });
                }
                else
                {
                    return StatusCode(500, new {message = "server error: can not generate Json web token"});
                }
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
                var success = _userService.Verify(username);
                if (!success) return Conflict(new { error = "user not found" });
                return Ok(new { msg = "user verified, pls login" });
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("who-am-i")]
        public IActionResult WhoAmI()
        {
            try
            {
                if (User?.Identity?.IsAuthenticated != true) // JWT 是否驗證通過
                {
                    return Unauthorized();
                }

                var username = User.Identity.Name;
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                var email = User.FindFirst(ClaimTypes.Email)?.Value;

                if(string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(role) || string.IsNullOrWhiteSpace(email))
                    return Unauthorized();
                
                return Ok(new
                {
                    msg = $"Hello, {username}",
                    data = new 
                    {
                        Role = role,
                        Email = email
                    }
                });
                
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
