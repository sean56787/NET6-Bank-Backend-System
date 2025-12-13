using Microsoft.AspNetCore.Mvc;     // ControllerBase
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using DotNetSandbox.Models.DTOs.Input;
using DotNetSandbox.Services.Interfaces;

namespace DotNetSandbox.Controllers
{
    [ApiController]                 // 自動處理 ModelState 錯誤驗證
    [Route("api/[controller]")]     // api/user
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        public UserController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        [Authorize(Roles = "admin, user")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            try
            {
                var result = await _userService.Register(req);

                if (!result.Success) 
                    return StatusCode(statusCode:result.StatusCode, result.Message);

                return Ok(new { message = result.Message });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = $"server error: {e.Message}"});
            }
        }

        [Authorize(Roles = "admin, user")]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            try
            {
                var result = await _userService.Login(req);       //檢查是否為已驗證帳戶

                if (result == null)
                {
                    return StatusCode(500, new { message = "server error: server no response" });
                }
                else if (!result.Success)
                {
                    return StatusCode(result.StatusCode, result.Message);
                }

                if (result?.Data != null)
                {
                    var tokenResult = _authService.GenerateToken(result.Data);
                    return Ok(new { message = tokenResult.Message, token = tokenResult.Data });
                }
                else
                {
                    return StatusCode(500, new {message = "server error: can not generate Json web token"});
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = $"server error: {e.Message}" });
            }
        }

        [Authorize(Roles = "admin, user")]
        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromQuery] string email)
        {
            try
            {
                var result = await _userService.Verify(email);
                if (result == null)
                {
                    return StatusCode(500, new { message = "server error: server no response" });
                }
                else if (!result.Success)
                {
                    return StatusCode(result.StatusCode, result.Message);
                }
                return Ok(new { message = result.Message });
            } catch (Exception e)
            {
                return StatusCode(500, new { message = $"server error: {e.Message}" });
            }
        }

        [Authorize(Roles = "admin, user")]
        [HttpGet("who-am-i")]
        public async Task<IActionResult> WhoAmI()
        {
            try
            {
                var username = User.Identity.Name;
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                var email = User.FindFirst(ClaimTypes.Email)?.Value;

                if(string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(role) || string.IsNullOrWhiteSpace(email))
                    return Unauthorized();
                
                return Ok(new
                {
                    message = $"Hello, {username}",
                    data = new 
                    {
                        Role = role,
                        Email = email
                    }
                });
                
            } catch (Exception e)
            {
                return StatusCode(500, new { message = $"server error: {e.Message}" });
            }
        }
    }
}
