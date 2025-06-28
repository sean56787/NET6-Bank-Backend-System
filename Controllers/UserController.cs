using Microsoft.AspNetCore.Mvc;     // ControllerBase
using DotNetSandbox.Services;       // register / login / verify
using Microsoft.AspNetCore.Authorization;
using DotNetSandbox.Data;
using DotNetSandbox.Models.DTOs;
using System.Security.Claims;

namespace DotNetSandbox.Controllers
{
    [ApiController]                 // 自動處理 ModelState 錯誤驗證
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

        [Authorize]
        [HttpPut("update-user")]
        public IActionResult UpdateUser([FromBody] UpdateUserRequest req)
        {
            try
            {
                var userDTO = _authService.UpdateUser(req);
                if (userDTO != null)
                    return Ok(new { msg = "user update success", userDTO });
                else
                    return BadRequest();
            } catch (Exception exp)
            {
                return StatusCode(500, exp.Message);
            }
        }

        [Authorize]
        [HttpPost("create-user")]
        public IActionResult CreateUser([FromBody] CreateUserRequest req)
        {
            try
            {
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                if(userRole != "admin")
                {
                    return StatusCode(403, new { error = "you have to permission to use this api" });
                }

                var userDTO = _authService.CreateUser(req.Username, req.Password, req.Email, req.Role);
                if(userDTO != null)
                {
                    return Ok(new { msg = "user created", userDTO });
                }
                else
                {
                    return BadRequest();
                }
            } catch (Exception exp)
            {
                return StatusCode(500, exp.Message);
            }
        }


        [Authorize]
        [HttpGet("get-user")]
        public IActionResult GetUser([FromQuery] GetUserRequest req)
        {
            try
            {
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                if (userRole != "admin")
                {
                    return StatusCode(403, new { error = "you have no permission to use this api" });
                }

                var usersDTO = _authService.GetUser(req.Username, req.Email);
                if (usersDTO != null)
                    return Ok(usersDTO);

                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("get-all-users")]
        public IActionResult GetAllUsers()
        {
            try
            {
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                if(userRole != "admin")
                {
                    return StatusCode(403, new { error = "you have no permission to use this api" });
                }

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
